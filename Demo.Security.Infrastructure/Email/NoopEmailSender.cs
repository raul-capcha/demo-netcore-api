using Demo.Security.Infrastructure.Abstractions;
using Microsoft.Extensions.Logging;

namespace Demo.Security.Infrastructure.Email
{
    public sealed class NoopEmailSender : IEmailSender
    {
        private readonly ILogger<NoopEmailSender> _logger;
        public NoopEmailSender(ILogger<NoopEmailSender> logger) => _logger = logger;

        public Task SendAsync(string to, string subject, string htmlBody, CancellationToken ct = default)
        {
            _logger.LogInformation("Simulando envío de email a {To}. Asunto: {Subject}. Body:\n{Body}", to, subject, htmlBody);
            return Task.CompletedTask;
        }
    }
}
