using System.Threading.Tasks;

namespace MySQLDemo.Framework.Infrastructure
{
    public interface IRedisRepository
    {
        Task<long> Incr(string key);
    }
}