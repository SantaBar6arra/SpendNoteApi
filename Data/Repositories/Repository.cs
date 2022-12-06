using Data.Context;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly SpendNoteContext _context;

        public Repository(SpendNoteContext context)
        {
            _context = context;
        }

        public bool Remove(T entity)
        {
            try
            {
                _context.Set<T>().Remove(entity);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<T> GetById(int id) => 
            await _context.Set<T>().FindAsync(id);

        public async Task<IEnumerable<T>> GetAll() =>
            await _context.Set<T>().ToListAsync();

        public bool Upsert(T entity)
        {
            try
            {
                _context.Set<T>().AddOrUpdate<T>(entity);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
