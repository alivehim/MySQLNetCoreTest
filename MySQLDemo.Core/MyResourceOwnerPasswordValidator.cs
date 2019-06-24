using IdentityModel;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using MySQLDemo.Core.Model;
using MySQLDemo.Core.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySQLDemo.Core
{
    public class MyResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        //private readonly IUserService _userService;
        private readonly IEnumerable<LoginHandler> _loginHandlers;
        private IHttpContextAccessor _context;
        public MyResourceOwnerPasswordValidator(IEnumerable<LoginHandler> loginHandlers, IHttpContextAccessor context)
        {
            _loginHandlers = loginHandlers;
            _context = context;
        }

        private ILoginHandler Find(ResourceOwnerPasswordValidationContext context)
        {
            //判断登录方式
            var loginType = context.Request.Raw["logintype"];
            foreach (var item in _loginHandlers)
            {

                if (item.LoginType == loginType)
                {
                    var handler = _context.HttpContext.RequestServices.GetService(item.Handler) as ILoginHandler;
                    return handler;
                }
            }

            //_logger.LogTrace("No endpoint entry found for request path: {path}", context.Request.Path);

            return null;

        }
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {

            var handler = Find(context);
            await handler.ProcessAsync(context);

            //var user = _userService.FindByName(context.UserName);
            //if (user != null)
            //{
            //    context.Result = new GrantValidationResult(
            //        user.Id.ToString(),
            //        OidcConstants.AuthenticationMethods.Password,
            //        DateTime.UtcNow,
            //        null);
            //}
            return;
        }
    }
}
