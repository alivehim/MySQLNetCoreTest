using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaasKit.Multitenancy.Modle
{
    public class TentantChooseResult
    {
        public long KeyId { get; set; }
        public AppTenant Tenant { get; set; }
    }
}
