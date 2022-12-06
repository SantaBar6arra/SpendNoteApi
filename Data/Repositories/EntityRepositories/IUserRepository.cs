using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.EntityRepositories
{
    public interface IUserRepository : IRepository<User>
    {
        public Task<User> GetByName(string name);
    }
}
