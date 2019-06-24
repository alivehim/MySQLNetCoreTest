using IdentityServer4.Hosting;
using IdentityServer4.ResponseHandling;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySQLDemo.Core.Model
{
    public class TokenResult : IEndpointResult
    {
        public TokenResponse Response { get; set; }

        public TokenResult(TokenResponse response)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));

            Response = response;
        }

        public async Task ExecuteAsync(HttpContext context)
        {
            context.Response.SetNoCache();

            var dto = new ResultDto
            {
                //id_token = Response.IdentityToken,
                //access_token = Response.AccessToken,
                //refresh_token = Response.RefreshToken,
                code = "1",
                message = "success",
                Data = new ResultDto.innerData
                {
                    access_token = Response.AccessToken,
                    refresh_token = Response.RefreshToken,
                    nickname = "hello",
                    openid = "abc"
                }

            };

            await context.Response.WriteJsonAsync(dto);
        }

        internal class ResultDto
        {
            public string code { get; set; }
            public string message { get; set; }

            public innerData Data { get; set; }
            internal class innerData
            {
                public string access_token { get; set; }
                public string refresh_token { get; set; }
                public string nickname { get; set; }
                public string openid { get; set; }
            }
        }
    }
}
