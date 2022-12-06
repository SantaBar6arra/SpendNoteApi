using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetById(int id);
        Task<IEnumerable<T>> GetAll();
        bool Upsert(T entity);
        bool Remove(T entity);
    }
}
