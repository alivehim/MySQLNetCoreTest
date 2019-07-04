using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaasKit.Multitenancy
{
    /// <summary>
    /// ITenant wrapper that returns the tenant instance.
    /// </summary>
    /// <typeparam name="TTenant"></typeparam>	
    public class TenantWrapper<TTenant> : ITenant<TTenant>
    {
        /// <summary>
        /// Intializes the wrapper with the tenant instance to return.
        /// </summary>
        /// <param name="tenant">The tenant instance to return.</param>		
        public TenantWrapper(TTenant tenant)
        {
            Value = tenant;
        }

        /// <summary>
        /// The tenant instance.
        /// </summary>
		public TTenant Value { get; }
    }

}
