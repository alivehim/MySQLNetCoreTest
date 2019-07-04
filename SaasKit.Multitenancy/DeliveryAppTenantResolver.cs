using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySQLDemo.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaasKit.Multitenancy
{
    public class DeliveryAppTenantResolve : MemoryCacheTenantResolver<AppTenant>
    {
        private readonly string CustomerKey = "cId";
        private readonly IEnumerable<AppTenant> tenants;

        public DeliveryAppTenantResolve(IMemoryCache cache, ILoggerFactory loggerFactory, IOptions<MultitenancyOptions> options)
            : base(cache, loggerFactory)
        {
            this.tenants = options.Value.Tenants;
        }

        protected override string GetContextIdentifier(HttpContext context)
        {
            if (context.Request.Headers.ContainsKey(CustomerKey))
            {
                return (context.Request.Headers[CustomerKey].ToString().ToLong() % tenants.Count()).ToString();
            }
            return null;
        }

        protected override IEnumerable<string> GetTenantIdentifiers(TenantContext<AppTenant> context)
        {
            return context.Tenant.Hostnames;
        }

        protected override Task<TenantContext<AppTenant>> ResolveAsync(HttpContext context)
        {
            TenantContext<AppTenant> tenantContext = null;

            //var tenant = tenants.FirstOrDefault(t =>
            //    t.Hostnames.Any(h => h.Equals(context.Request.Host.Value.ToLower())));

            //if (tenant != null)
            //{
            //    tenantContext = new TenantContext<AppTenant>(tenant);
            //}

            if (!context.Request.Headers.ContainsKey(CustomerKey))
            {
                var customerId = long.Parse(context.Request.Headers[CustomerKey].ToString());
                var tenant = tenants.ElementAt((int)(customerId % tenants.Count()));
                tenantContext = new TenantContext<AppTenant>(tenant);
            }

            return Task.FromResult(tenantContext);
        }

        protected override MemoryCacheEntryOptions CreateCacheEntryOptions()
        {
            return base.CreateCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5));
        }
    }
}
