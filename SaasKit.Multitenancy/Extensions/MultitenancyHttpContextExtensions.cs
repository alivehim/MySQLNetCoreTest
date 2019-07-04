using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaasKit.Multitenancy
{
    public static class MultitenancyHttpContextExtensions
    {
        private const string TenantContextKey = "saaskit.TenantContext";

        public static void SetTenantContext<TTenant>(this HttpContext context, TenantContext<TTenant> tenantContext)
        {
            Ensure.Argument.NotNull(context, nameof(context));
            Ensure.Argument.NotNull(tenantContext, nameof(tenantContext));

            context.Items[TenantContextKey] = tenantContext;
        }

        /// <summary>
        /// Read Parameter from http post form data
        /// </summary>
        /// <typeparam name="TTenant"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public static TenantContext<TTenant> GetTenantContext<TTenant>(this HttpContext context)
        {
            Ensure.Argument.NotNull(context, nameof(context));

            object tenantContext;
            if (context.Items.TryGetValue(TenantContextKey, out tenantContext))
            {
                return tenantContext as TenantContext<TTenant>;
            }

            return null;
        }

        public static TTenant GetTenant<TTenant>(this HttpContext context)
        {
            Ensure.Argument.NotNull(context, nameof(context));

            var tenantContext = GetTenantContext<TTenant>(context);

            if (tenantContext != null)
            {
                return tenantContext.Tenant;
            }

            return default(TTenant);
        }
    }
}
