using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MySQLDemo.Core.DB;
using MySQLDemo.Core.Domain;

namespace MySQLDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        public TestDbContext _dbContext;
        IRepository<User> userRepository;
        public ValuesController(IRepository<User> user)
        {
            this.userRepository = user;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<string> Get()
        {
            //var dbUser= _dbContext.User.FirstOrDefault();
            //if (dbUser != null)
            //    return dbUser.Name;
            //else

            var entity = userRepository.Table.FirstOrDefault();
            return entity?.Name;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
