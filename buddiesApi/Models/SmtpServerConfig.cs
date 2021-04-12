namespace buddiesApi.Middlewares.AuthenticationManager {
    public interface ISmtpServerConfig {
        public string ServerAddress { get; set; }
        public int Portnumber { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
    }

    public class SmtServerConfig : ISmtpServerConfig {
        public string ServerAddress { get; set; }
        public int Portnumber { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
    }
}
