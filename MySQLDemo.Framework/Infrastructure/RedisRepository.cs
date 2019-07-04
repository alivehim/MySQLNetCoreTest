using Microsoft.Extensions.Options;
using MySQLDemo.Framework.Config;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySQLDemo.Framework.Infrastructure
{
    public class RedisRepository : IRedisRepository
    {

        //IConfiguration _configuration;

        RedisConfig _config;
    
        private static  string ConnectionString = string.Empty;
        private static object _locker = new Object();

        private static ConnectionMultiplexer _instance = null;

        public static ConnectionMultiplexer Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_locker)
                    {
                        if (_instance == null || !_instance.IsConnected)
                        {
                            _instance = ConnectionMultiplexer.Connect(ConnectionString);
                        }
                    }
                }

                return _instance;
            }
        }


        public RedisRepository(IOptions<RedisConfig> configuration)
        {
            _config = configuration.Value;
            //_configuration = configuration;
            ConnectionString = _config.Url;
        }

        public static IDatabase GetDatabase()
        {
            return Instance.GetDatabase();
        }


        public async Task<long> Incr(string key)
        {
            return await GetDatabase().StringIncrementAsync(key);
        }
    }
}
