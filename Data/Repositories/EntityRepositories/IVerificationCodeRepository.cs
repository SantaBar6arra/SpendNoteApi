using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.EntityRepositories
{
    public interface IVerificationCodeRepository : IRepository<VerificationCode>
    {
        public Task<VerificationCode> GetByGuid(string guid);
        public Task<VerificationCode> GetByUserId(int userId);
    }
}
