using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using buddiesApi.Models;
using MailKit.Net.Smtp;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using static buddiesApi.Helpers.SecurityManager;

namespace buddiesApi.Middlewares.AuthenticationManager {
    public class AuthenticationManager : IAuthenticationManager{
        private readonly AuthenticationConfig config;

        private EmailPayload GetEmailPayload(
            VerificationType type,
            string verificationLink
        ) {
            bool verifyEmail = type == VerificationType.Email;
            return new EmailPayload {
                Subject = verifyEmail ? "E-Mail Bestätigung" : "Verifikation",
                Body = new TextPart("html"){
                    Text = type == VerificationType.Email
                        ? GetEmailVerificationBody(verificationLink)
                        : GetIdentityVerificationBody(verificationLink)                            
                }
            };
        }

        private string GetEmailVerificationBody(string link) {
            return "<p>Hallo,</p><p>bitte bestätige deine E-Mail Adresse, indem du den folgenden Link aufrufst:<p>" +
                "<a href=\"" + link + "\">hier bestätigen</a><p>Danke,</p><p>" + config.Hostname + "</p>";
        }

        private string GetIdentityVerificationBody(string link) {
            return "<p>Hallo,</p><p>dein Passwort wurde drei Mal hintereinander falsch eingegeben. Wir haben deinen Account aus sicherheitsgründen gesperrt, bitte gehe auf folgenden Link um deinen Account wieder freizuschalten<p>" +
                "<a href=\"" + link + "\">hier aufrufen</a><p>Danke,</p><p>" + config.Hostname + "</p>";
        }

        public AuthenticationManager(AuthenticationConfig config) {
            this.config = config;
        }

        public string Authenticate(UserCred userCred, User user) {
            if (user == null || !IsCorrectPassword(userCred.Password, user)) {
                return null;
            }
            return CreateBearerToken(user);
        }

        public string CreateBearerToken(User user) {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(config.Key);

            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
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

        public bool IsCorrectPassword(string password, User user) {
            var verifyingSecret = ComputeHash(password, ConvertStringSalt(user.Salt));
            return verifyingSecret.HashedSaltedPassword == user.Password;
        }

        public void SendVerificationEmail(
            VerificationType type,
            string varificationLink,
            string email
        ) {
            var emailPayload = GetEmailPayload(type, varificationLink);
            var message = new MimeMessage {
                From = { config.AdminMailboxAddress },
                To = { new MailboxAddress("", email) },
                Subject = emailPayload.Subject,
                Body = emailPayload.Body
            };

            using var client = new SmtpClient();
            client.Connect(
                config.SmtpServerConfig.ServerAddress,
                config.SmtpServerConfig.Portnumber,
                false);

            client.Authenticate(
                config.SmtpServerConfig.EmailAddress,
                config.SmtpServerConfig.Password);

            client.Send(message);

            client.Disconnect(true);
        }
    }
}
