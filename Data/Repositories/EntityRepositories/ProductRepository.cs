using Data.Context;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.EntityRepositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(SpendNoteContext context) : base(context)
        {
        }

        public virtual async Task<IEnumerable<Product>> GetAll(int listId) =>
            (await GetAll()).Where(i => i.List.Id == listId);
    }
}
