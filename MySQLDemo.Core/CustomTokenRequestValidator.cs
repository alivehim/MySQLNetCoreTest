using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using MySQLDemo.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySQLDemo.Core
{
    public class CustomTokenRequestValidator : ICustomTokenRequestValidator
    {
        private readonly ILogger _logger;

        public CustomTokenRequestValidator(ILogger<CustomTokenRequestValidator> logger)
        {
            _logger = logger;
        }

        public Task ValidateAsync(CustomTokenRequestValidationContext context)
        {
            var parameters = context.Result.ValidatedRequest.Raw;


            //var grantType = parameters.Get("ip");
            //if (grantType.IsMissing())
            //{
            //    SetError(context, "ip address is missing");
            //}

            //var loginType = parameters.Get("LoginType");
            //if (loginType.IsMissing())
            //{
            //    //范围验证
            //    SetError(context, "loginType is missing");
            //}

            return Task.CompletedTask;
        }

        private void SetError(CustomTokenRequestValidationContext context,string errorMessage)
        {
            LogError("errorMessage");
            context.Result.IsError = true;
            context.Result.Error = errorMessage;
        }

        private void LogError(string message = null, object values = null)
        {
            LogWithRequestDetails(LogLevel.Error, message, values);
        }

        private void LogWarning(string message = null, object values = null)
        {
            LogWithRequestDetails(LogLevel.Warning, message, values);
        }

        private void LogInformation(string message = null, object values = null)
        {
            LogWithRequestDetails(LogLevel.Information, message, values);
        }

        private void LogWithRequestDetails(LogLevel logLevel, string message = null, object values = null)
        {
            _logger.Log(logLevel, message);
        }
    }
}
