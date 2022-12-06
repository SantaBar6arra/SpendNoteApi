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
    public interface IUnitOfWork
    {
        public UserRepository UserRepository { get; }
        public IncomeCategoryRepository IncomeCategoryRepository { get; }
        public IncomeRepository IncomeRepository { get; }
        public ProductCategoryRepository ProductCategoryRepository { get; }
        public ProductRepository ProductRepository { get; }
        public ListRepository ListRepository { get; }
        public VerificationCodeRepository VerificationCodeRepository { get; }
        public Task<int> Complete();
    }
}
