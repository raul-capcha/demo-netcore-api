namespace Demo.Security.Infrastructure.Email
{
    public sealed class SmtpOptions
    {
        public string Host { get; set; } = "";
        public int Port { get; set; } = 587;            // 587 STARTTLS (recomendado), 465 SSL
        public bool UseStartTls { get; set; } = true;   // true => STARTTLS; false => depende de UseSsl
        public bool UseSsl { get; set; } = false;       // true => SSL explícito (465)
        public bool SkipCertificateValidation { get; set; } = false; // solo dev

        public string FromName { get; set; } = "Back Office";
        public string FromEmail { get; set; } = "no-reply@tu-dominio.com";

        public string? Username { get; set; }           // null ⇒ sin autenticación
        public string? Password { get; set; }

        public int TimeoutSeconds { get; set; } = 30;
    }
}
