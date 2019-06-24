using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityServer4.Validation;
using MySQLDemo.Core.Model;

namespace MySQLDemo.Core.Service
{
    public class TouristLoginHandler : ILoginHandler
    {
        public Task ProcessAsync(ResourceOwnerPasswordValidationContext context)
        {
            throw new NotImplementedException();
        }
    }
}
