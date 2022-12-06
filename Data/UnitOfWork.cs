using Data.Context;
using Data.Models;
using Data.Repositories;
using Data.Repositories.EntityRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SpendNoteContext _context;

        public UserRepository UserRepository { get; private set; }

        public IncomeCategoryRepository IncomeCategoryRepository { get; private set; }

        public IncomeRepository IncomeRepository { get; private set; }

        public ProductCategoryRepository ProductCategoryRepository { get; private set; }

        public ProductRepository ProductRepository { get; private set; }

        public ListRepository ListRepository { get; private set; }

        public VerificationCodeRepository VerificationCodeRepository { get; private set; }

        public UnitOfWork(SpendNoteContext context)
        {
            _context = context;

            UserRepository = new(_context);
            IncomeCategoryRepository = new(_context);
            IncomeRepository = new(_context);
            ProductCategoryRepository = new(_context);
            ProductRepository = new(_context);
            ListRepository = new(_context);
            VerificationCodeRepository = new(_context);
        }

        public async Task<int> Complete() => await _context.SaveChangesAsync();
    }
}
