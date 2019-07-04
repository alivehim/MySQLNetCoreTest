using DataRunner.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySQLDemo.Framework.Config;
using MySQLDemo.Framework.Infrastructure;
using System;
using System.Collections;
using System.IO;

namespace DataRunner
{
    class Program
    {
        static void Main(string[] args)
        {



            var builder = new ConfigurationBuilder()
.SetBasePath(Directory.GetCurrentDirectory())
.AddJsonFile("appsettings.json");

            var config = builder.Build();

            var serviceProvider = new ServiceCollection()
                .Configure<RedisConfig>(config.GetSection("RedisConfig"))
                .AddLogging()
                .AddSingleton<IRedisRepository, RedisRepository>()
                .AddSingleton<IConcurencyRunner, ConcurencyRunner>()
                .BuildServiceProvider();

            //var test = serviceProvider.GetService<IOptions<RedisConfig>>();    //获取注入的配置数据对象

            //serviceProvider
            //    .GetService<ILoggerFactory>().AddConsole();

            var logger = serviceProvider.GetService<ILoggerFactory>()
                .CreateLogger<Program>();
            logger.LogDebug("Starting application");

            var counter = serviceProvider.GetService<IConcurencyRunner>();
            counter.Do();

            Console.ReadKey();

            counter.Stop();

            logger.LogDebug("All done!");
        }
    }
}
