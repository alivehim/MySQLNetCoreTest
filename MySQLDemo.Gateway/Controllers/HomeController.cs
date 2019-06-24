using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySQLDemo.Gateway.Controllers
{
    public class HomeController:Controller
    {
        public ActionResult Index()
        {
            return Content(string.Format("IdentityServer . API - {0:yyyy-MM-dd HH:mm:ss.fff}", DateTime.Now));
        }
    }
}
