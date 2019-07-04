using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaasKit.Multitenancy
{
    /// <summary>
    /// Used to retreive configured TTenant instances.
    /// </summary>
    /// <typeparam name="TTenant">The type of tenant being requested.</typeparam>
    public interface ITenant<out TTenant>
    {
        TTenant Value { get; }
    }

}
