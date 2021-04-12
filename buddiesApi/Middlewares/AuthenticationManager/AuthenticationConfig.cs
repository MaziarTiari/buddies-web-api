using System;
using MimeKit;

namespace buddiesApi.Middlewares.AuthenticationManager {
    public class AuthenticationConfig {
        public string Key { get; set; }
        public string Hostname { get; set; }
        public MailboxAddress AdminMailboxAddress { get; set; }
        public SmtServerConfig SmtpServerConfig { get; set; } = new SmtServerConfig();
    }

    public enum VerificationType {
        Email,
        Identity,
    }

    internal class EmailPayload {
        public string Subject { get; set; }
        public TextPart Body { get; set; } = new TextPart("plain");
    }
}
