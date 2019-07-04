using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySQLDemo.Core.DB;
using MySQLDemo.Core.Domain;
using MySQLDemo.Core.Service;
using MySQLDemo.Framework.Infrastructure;
using MySQLDemo.Resource.Model;
using SaasKit.Multitenancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySQLDemo.Resource.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        public UserController(IHttpContextAccessor httpContext)
        {
        }

        [Route("~/Register")]
        [HttpPost]
        public IActionResult Register([FromBody]RegisterModel model,
            [FromServices]IRedisRepository redisRepository,
            [FromServices]ITentantRedisResolver tentantResolver)
        {

            var result = tentantResolver.Find();

            using (var context = new TenantDbContext(result.Tenant))
            {
                var repository = new EfRepository<User>(context);
                repository.Insert(new User { Name = "unkown", Password = "xxxyyy" });
            }

            //返回ID
            return Ok();
        }

        [HttpGet]
        [Route("~/FindByName")]
        public IActionResult FindByName([FromServices]IUserService userService)
        {
            var val = userService.FindByName("hello");
            return Ok(val);
        }
    }
}
