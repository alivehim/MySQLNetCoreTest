using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySQLDemo.Framework.Extensions
{
    public static class StringExtension
    {
       public static long ToLong(this string @this)
        {
            return long.Parse(@this);
        }
    }
}
