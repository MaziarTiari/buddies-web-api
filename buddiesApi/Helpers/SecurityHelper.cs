using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace buddiesApi.Helpers
{
    public static class SecurityHelper
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

        public static byte[] GenerateSalt()
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
