using Demo.Security.Infrastructure.Abstractions;
using System.Text;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Demo.Security.Infrastructure.Email
{
    public sealed class SmtpEmailSender : IEmailSender
    {
        private readonly SmtpOptions _opt;
        private readonly ILogger<SmtpEmailSender> _logger;

        public SmtpEmailSender(IOptions<SmtpOptions> opt, ILogger<SmtpEmailSender> logger)
        {
            _opt = opt.Value;
            _logger = logger;
        }

        public async Task SendAsync(string to, string subject, string htmlBody, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(_opt.Host))
                throw new InvalidOperationException("SMTP: Host no configurado.");

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_opt.FromName, _opt.FromEmail));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject ?? "";

            // Fallback de texto plano a partir del HTML (muy básico)
            var plainText = StripHtml(htmlBody);

            var alternative = new Multipart("alternative")
        {
            new TextPart("plain") { Text = plainText },
            new TextPart("html")  { Text = htmlBody }
        };

            message.Body = alternative;

            using var client = new SmtpClient();

            // Opcional: ignorar validación de certificado (solo DEV)
            if (_opt.SkipCertificateValidation)
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
            }

            client.Timeout = _opt.TimeoutSeconds * 1000;

            // Conexión
            SecureSocketOptions sslOpt = SecureSocketOptions.Auto; // Auto negocia
            if (_opt.UseSsl) sslOpt = SecureSocketOptions.SslOnConnect;       // 465
            else if (_opt.UseStartTls) sslOpt = SecureSocketOptions.StartTls; // 587
            else sslOpt = SecureSocketOptions.None;                            // 25 (sin TLS)

            await client.ConnectAsync(_opt.Host, _opt.Port, sslOpt, ct);

            // Autenticación (si hay credenciales)
            if (!string.IsNullOrWhiteSpace(_opt.Username))
            {
                await client.AuthenticateAsync(_opt.Username, _opt.Password ?? "", ct);
            }

            await client.SendAsync(message, ct);
            await client.DisconnectAsync(true, ct);

            _logger.LogInformation("Email enviado a {To} con asunto \"{Subject}\"", to, subject);
        }

        private static string StripHtml(string html)
        {
            if (string.IsNullOrWhiteSpace(html)) return string.Empty;
            var sb = new StringBuilder(html.Length);
            bool inside = false;

            foreach (var ch in html)
            {
                if (ch == '<') { inside = true; continue; }
                if (ch == '>') { inside = false; continue; }
                if (!inside) sb.Append(ch);
            }

            // Normaliza saltos y espacios
            return System.Text.RegularExpressions.Regex.Replace(sb.ToString(), @"\s+\n", "\n")
                                                      .Replace("&nbsp;", " ")
                                                      .Trim();
        }
    }
}
