using IdentityServer4.Hosting;
using IdentityServer4.ResponseHandling;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MySQLDemo.Core;
using MySQLDemo.Core.Extensions;
using MySQLDemo.Core.Model;
using MySQLDemo.Core.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySQLDemo.Extension
{
    public static class IdentityServerMyBuilderExtensions
    {
        public static IIdentityServerBuilder AddMyIdentityServer(
           this IIdentityServerBuilder builder)
        {
            //builder.Services.AddSingleton<ITokenResponseGenerator, MyTokenResponseGenerator>();
            builder.Services.TryGetDescriptors(typeof(Endpoint), out var descriptors);

            var services = builder.Services;

            var descriptor = descriptors.FirstOrDefault(p => ((Endpoint)p.ImplementationInstance).Name == "Token");
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            var descriptorIntrospection = descriptors.FirstOrDefault(p => ((Endpoint)p.ImplementationInstance).Name == "Introspection");
            if (descriptorIntrospection != null)
            {
                services.Remove(descriptorIntrospection);
            }

            services.TryGetDescriptors(typeof(IEndpointRouter), out var descriptorsx);
            foreach (var item in descriptorsx)
            {
                var index = services.IndexOf(item);

                //services.Insert(index, item.WithImplementationType(implementationType));

                services.Remove(item);
            }

            builder.Services.AddTransient<IEndpointRouter, MyEndpointRouter>();

            builder.AddEndpoint<MyEndpointHandler>("Token", ProtocolRoutePaths.Token.EnsureLeadingSlash());
            //builder.AddEndpoint<MyIntrospectionEndpoint>("Introspection", ProtocolRoutePaths.Introspection.EnsureLeadingSlash());

            builder.AddLoginHandler<PasswordLoginHandler>(LoginType.ByPassword);
            builder.AddLoginHandler<PhoneLoginHandler>(LoginType.ByPhone);
            builder.AddLoginHandler<TouristLoginHandler>(LoginType.ByTourist);


            //builder.Services.AddSingleton<IEndpointHandler, MyEndpointHandler>();
            return builder;
        }

        public static IIdentityServerBuilder AddLoginHandler<T>(this IIdentityServerBuilder builder, string loginType)
          where T : class, ILoginHandler
        {
            builder.Services.AddTransient<T>();
            builder.Services.AddSingleton(new LoginHandler(loginType, typeof(T)));

            return builder;
        }

        private static bool TryGetDescriptors(this IServiceCollection services, Type serviceType, out ICollection<ServiceDescriptor> descriptors)
        {
            return (descriptors = services.Where(service => service.ServiceType == serviceType).ToArray()).Any();
        }
    }
}
