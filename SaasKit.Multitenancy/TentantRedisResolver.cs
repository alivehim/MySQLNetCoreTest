using Microsoft.Extensions.Options;
using MySQLDemo.Framework.Infrastructure;
using SaasKit.Multitenancy.Modle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaasKit.Multitenancy
{
    public class TentantRedisResolver : ITentantRedisResolver
    {
        IRedisRepository redisRepository;
        MultitenancyOptions options;
        public TentantRedisResolver(IRedisRepository repository, IOptions<MultitenancyOptions> options)
        {
            redisRepository = repository;
            this.options = options.Value;
        }

        public TentantChooseResult Find()
        {
            var keyId = redisRepository.Incr("hello").ConfigureAwait(false).GetAwaiter().GetResult();

            var index =(int) (keyId % options.Tenants.Count());

            return new TentantChooseResult { KeyId=index, Tenant= options.Tenants.ElementAt(index) };

        }
    }
}
