using Data.Context;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.EntityRepositories
{
    public class ListRepository : Repository<List>, IListRepository
    {
        public ListRepository(SpendNoteContext context) : base(context)
        {
        }

        public virtual async Task<IEnumerable<List>> GetAll(int userId) =>
            (await GetAll()).Where(i => i.User.Id == userId);
    }
}
