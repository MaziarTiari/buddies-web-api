using System;
using System.Linq;
using System.Security.Cryptography;
using buddiesApi.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace buddiesApi.Helpers
{
    public static class SecurityManager
    {
        public class SecuredPassword {
            public string HashedSaltedPassword { get; set; }
            public string Salt { get; set; }

        }

        private static class Pbkdf2Config
        {
            public static readonly int iterationCount = 10000;
            public static readonly int numBytesRequested = 256 / 8;
        }

        public static SecuredPassword ComputeHash(string password, byte[] salt)
        {
            
            SecuredPassword sp = new SecuredPassword();
            sp.Salt = Convert.ToBase64String(salt);
            sp.HashedSaltedPassword = Convert.ToBase64String(
                KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: Pbkdf2Config.iterationCount,
                    numBytesRequested: Pbkdf2Config.numBytesRequested
                )
            );
            return sp;
        }

        public static string RandomString(int length) {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789.-_~|^%{}[] ";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static byte[] ConvertStringSalt(string salt)
        {
            return Convert.FromBase64String(salt);
        }

        public static User EncodeUsersPassword(User user)
        {
            SecuredPassword secPass = ComputeHash(user.Password, GenerateRandomByte());
            user.Password = secPass.HashedSaltedPassword;
            user.Salt = secPass.Salt;
            return user;
        }

        public static byte[] GenerateRandomByte()
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }
    }
}
