using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Validation;
using MySQLDemo.Core.Model;

namespace MySQLDemo.Core.Service
{
    public class PasswordLoginHandler : ILoginHandler
    {
        private readonly IUserService _userService;
        public PasswordLoginHandler(IUserService userService)
        {
            _userService = userService;
        }

        public Task ProcessAsync(ResourceOwnerPasswordValidationContext context)
        {
            var user = _userService.FindByName(context.UserName);
            if (user != null)
            {
                context.Result = new GrantValidationResult(
                    user.Id.ToString(),
                    OidcConstants.AuthenticationMethods.Password,
                    DateTime.UtcNow,
                    null);
            }
            return Task.CompletedTask;
        }
    }
}
