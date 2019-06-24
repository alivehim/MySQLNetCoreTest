using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySQLDemo.Core.DB;
using MySQLDemo.Core.Domain;

namespace MySQLDemo.Core.Service
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> userRepository;
        public UserService(IRepository<User> userRepository)
        {
            this.userRepository = userRepository;
        }

        public User FindByName(string name)
        {
            return userRepository.Table.FirstOrDefault(p => p.Name == name);
        }
    }
}
