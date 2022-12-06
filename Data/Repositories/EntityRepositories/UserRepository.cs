using Data.Context;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.EntityRepositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(SpendNoteContext context) : base(context)
        {
        }

        public async Task<User> GetByName(string name) =>
            await _context.Users.SingleOrDefaultAsync(u => u.Name == name);
    }
}
