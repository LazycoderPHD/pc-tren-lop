using System;
using System.Security.Cryptography;

namespace TaskBoard.Helpers
{
    // Mật khẩu không bao giờ được lưu dạng văn bản thô.
    // Mỗi tài khoản có một chuỗi salt riêng, mật khẩu được băm bằng PBKDF2 (Rfc2898DeriveBytes).
    public static class PasswordHelper
    {
        private const int Iterations = 10000;

        public static string GenerateSalt()
        {
            byte[] saltBytes = new byte[16];
            using (RNGCryptoServiceProvider generator = new RNGCryptoServiceProvider())
            {
                generator.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        public static string HashPassword(string password, string salt)
        {
            byte[] saltBytes = Convert.FromBase64String(salt);
            using (Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, Iterations))
            {
                byte[] hashBytes = pbkdf2.GetBytes(20);
                return Convert.ToBase64String(hashBytes);
            }
        }

        public static bool Verify(string password, string salt, string expectedHash)
        {
            string actualHash = HashPassword(password, salt);
            return actualHash == expectedHash;
        }
    }
}
