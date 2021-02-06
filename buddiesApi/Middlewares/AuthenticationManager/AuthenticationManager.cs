using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using buddiesApi.Models;
using buddiesApi.Services;
using Microsoft.IdentityModel.Tokens;
using static buddiesApi.Helpers.SecurityManager;

namespace buddiesApi.Middlewares.AuthenticationManager {
    public class AuthenticationManager : IAuthenticationManager{
        private readonly string key;

        public AuthenticationManager(string key) {
            this.key = key;
        }

        public string Authenticate(UserCred userCred, User user) {
            if (user == null || !IsCorrectPassword(userCred.Password, user)) {
                return null;
            }
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenKey = Encoding.ASCII.GetBytes(key);

            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.NameIdentifier, user.Id)
                }),

                Expires = DateTime.UtcNow.AddHours(1),

                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private bool IsCorrectPassword(string password, User user) {
            var verifyingSecret = ComputeHash(password, ConvertStringSalt(user.Salt));
            return verifyingSecret.HashedSaltedPassword == user.Password;
        }
    }
}
