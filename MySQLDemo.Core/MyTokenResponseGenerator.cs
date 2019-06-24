using IdentityModel;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySQLDemo.Core
{
    public class MyTokenResponseGenerator: TokenResponseGenerator
    {
        //private readonly DapperStoreOptions _config;
        //private readonly ICache<CzarToken> _cache;
        public MyTokenResponseGenerator(ISystemClock clock, ITokenService tokenService, IRefreshTokenService refreshTokenService, IResourceStore resources, IClientStore clients, ILogger<TokenResponseGenerator> logger) : base(clock, tokenService, refreshTokenService, resources, clients, logger)
        {
            //_config = config;
            //_cache = cache;
        }

        public override async Task<TokenResponse> ProcessAsync(TokenRequestValidationResult request)
        {
            var result = new TokenResponse();
            switch (request.ValidatedRequest.GrantType)
            {
                case OidcConstants.GrantTypes.ClientCredentials:
                    result = await ProcessClientCredentialsRequestAsync(request);
                    break;
                case OidcConstants.GrantTypes.Password:
                    result = await ProcessPasswordRequestAsync(request);
                    break;
                case OidcConstants.GrantTypes.AuthorizationCode:
                    result = await ProcessAuthorizationCodeRequestAsync(request);
                    break;
                case OidcConstants.GrantTypes.RefreshToken:
                    result = await ProcessRefreshTokenRequestAsync(request);
                    break;
                default:
                    result = await ProcessExtensionGrantRequestAsync(request);
                    break;
            }
            //if (result != null)
            //{
            //    result.Custom = new Dictionary<string, object> { };
            //    result.Custom.Add("Code", 1);
            //}
           
            return result;
        }

    }
}
