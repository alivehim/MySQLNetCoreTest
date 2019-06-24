using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySQLDemo.Resource.Extension
{
    public static class AppExtension
    {
        public static void RegisterToConsul(this IApplicationBuilder app, IConfiguration configuration, IApplicationLifetime lifetime)
        {
            lifetime.ApplicationStarted.Register(() =>
            {
                string serviceName = configuration.GetValue<string>("consulConfig:serviceName");
                string serviceIP = configuration.GetValue<string>("consulConfig:serviceIP");
                string consulClientUrl = configuration.GetValue<string>("consulConfig:consulClientUrl");
                string healthCheckRelativeUrl = configuration.GetValue<string>("consulConfig:healthCheckRelativeUrl");
                int healthCheckIntervalInSecond = configuration.GetValue<int>("consulConfig:healthCheckIntervalInSecond");
                ICollection<string> listenUrls = app.ServerFeatures.Get<IServerAddressesFeature>().Addresses;


                if (string.IsNullOrWhiteSpace(serviceName))
                {
                    throw new Exception("Please use --serviceName=yourServiceName to set serviceName");
                }
                if (string.IsNullOrEmpty(consulClientUrl))
                {
                    consulClientUrl = "http://127.0.0.1:8500";
                }
                if (string.IsNullOrWhiteSpace(healthCheckRelativeUrl))
                {
                    healthCheckRelativeUrl = "health";
                }
                healthCheckRelativeUrl = healthCheckRelativeUrl.TrimStart('/');
                if (healthCheckIntervalInSecond <= 0)
                {
                    healthCheckIntervalInSecond = 1;
                }


                string protocol;
                int servicePort = 0;
                if (!TryGetServiceUrl(listenUrls, out protocol, ref serviceIP, out servicePort, out var errorMsg))
                {
                    throw new Exception(errorMsg);
                }

                var consulClient = new ConsulClient(ConsulClientConfiguration => ConsulClientConfiguration.Address = new Uri(consulClientUrl));

                var httpCheck = new AgentServiceCheck()
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(10),//服务启动多久后注册
                    Interval = TimeSpan.FromSeconds(healthCheckIntervalInSecond),
                    HTTP = $"{protocol}://{serviceIP}:{servicePort}/{healthCheckRelativeUrl}",
                    Timeout = TimeSpan.FromSeconds(2)
                };

                // 生成注册请求
                var registration = new AgentServiceRegistration()
                {
                    Checks = new[] { httpCheck },
                    ID = Guid.NewGuid().ToString(),
                    Name = serviceName,
                    Address = serviceIP,
                    Port = servicePort,
                    Meta = new Dictionary<string, string>() { ["Protocol"] = protocol },
                    Tags = new[] { $"{protocol}" }
                };
                consulClient.Agent.ServiceRegister(registration).Wait();

                //服务停止时, 主动发出注销
                lifetime.ApplicationStopping.Register(() =>
                {
                    try
                    {
                        consulClient.Agent.ServiceDeregister(registration.ID).Wait();
                    }
                    catch
                    { }
                });
            });
        }


        private static bool TryGetServiceUrl(ICollection<string> listenUrls, out string protocol, ref string serviceIP, out int port, out string errorMsg)
        {
            protocol = null;
            port = 0;
            errorMsg = null;
            if (!string.IsNullOrWhiteSpace(serviceIP)) // 如果提供了对外服务的IP, 只需要检测是否在listenUrls里面即可
            {
                foreach (var listenUrl in listenUrls)
                {
                    Uri uri = new Uri(listenUrl);
                    protocol = uri.Scheme;
                    var ipAddress = uri.Host;
                    port = uri.Port;

                    if (ipAddress == serviceIP || ipAddress == "0.0.0.0" || ipAddress == "[::]")
                    {
                        return true;
                    }
                }
                errorMsg = $"The serviceIP that you provide is not in urls={string.Join(',', listenUrls)}";
                return false;
            }
            else // 没有提供对外服务的IP, 需要查找本机所有的可用IP, 看看有没有在 listenUrls 里面的
            {
                var allIPAddressOfCurrentMachine = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()
                        .Select(p => p.GetIPProperties())
                        .SelectMany(p => p.UnicastAddresses)
                        // 这里排除了 127.0.0.1 loopback 地址
                        .Where(p => p.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && !System.Net.IPAddress.IsLoopback(p.Address))
                        .Select(p => p.Address.ToString()).ToArray();
                var uris = listenUrls.Select(listenUrl => new Uri(listenUrl)).ToArray();
                // 本机所有可用IP与listenUrls进行匹配, 如果listenUrl是"0.0.0.0"或"[::]", 则任意IP都符合匹配
                var matches = allIPAddressOfCurrentMachine.SelectMany(ip =>
                        uris.Where(uri => ip == uri.Host || uri.Host == "0.0.0.0" || uri.Host == "[::]")
                        .Select(uri => new { Protocol = uri.Scheme, ServiceIP = ip, Port = uri.Port })
                ).ToList();

                if (matches.Count == 0)
                {
                    errorMsg = $"This machine has IP address=[{string.Join(',', allIPAddressOfCurrentMachine)}], urls={string.Join(',', listenUrls)}, none match.";
                    return false;
                }
                else if (matches.Count == 1)
                {
                    protocol = matches[0].Protocol;
                    serviceIP = matches[0].ServiceIP;
                    port = matches[0].Port;
                    return true;
                }
                else
                {
                    errorMsg = $"Please use --serviceIP=yourChosenIP to specify one IP, which one provide service: {string.Join(",", matches)}.";
                    return false;
                }
            }
        }
    }
}
