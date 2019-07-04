using MySQLDemo.Framework.Infrastructure;
using SaasKit.Multitenancy.Modle;

namespace SaasKit.Multitenancy
{
    public interface ITentantRedisResolver
    {
        TentantChooseResult Find();
    }
}