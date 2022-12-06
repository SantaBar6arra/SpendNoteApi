using System.Security.Cryptography;
using System.Text;

namespace WebApi.Services
{
    public class PasswordHashService
    {
        public static void CreatePasswordHash(string password, out string hash, out string salt)
        {
            using (var hmac = new HMACSHA512())
            {
                salt = Convert.ToBase64String(hmac.Key);
                hash = Convert.ToBase64String(hmac.ComputeHash(Encoding.ASCII.GetBytes(password)));
            }
        }

        public static bool VerifyPasswordHash(string password, string hash, string salt)
        {
            using (var hmac = new HMACSHA512(Convert.FromBase64String(salt)))
            {
                string computedHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
                return string.Equals(computedHash, hash);
            }
        }
    }
}
