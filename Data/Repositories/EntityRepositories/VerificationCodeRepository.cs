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
    public class VerificationCodeRepository : Repository<VerificationCode>, IVerificationCodeRepository
    {
        public VerificationCodeRepository(SpendNoteContext context) : base(context)
        {
        }

        public async Task<VerificationCode> GetByGuid(string guid) =>
            await _context.VerificationCodes.SingleOrDefaultAsync(vc => vc.Code == guid);

        public async Task<VerificationCode> GetByUserId(int userId) =>
            await _context.VerificationCodes.SingleOrDefaultAsync(vc => vc.User.Id == userId);
    }
}
