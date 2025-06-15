using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;

namespace APiTurboSetup.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _fromEmail;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            _smtpServer = _configuration["Email:SmtpServer"] ?? "smtp.gmail.com";
            _smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
            _smtpUsername = _configuration["Email:Username"] ?? "";
            _smtpPassword = _configuration["Email:Password"] ?? "";
            _fromEmail = _configuration["Email:FromEmail"] ?? "";
        }

        public async Task EnviarEmailAsync(string to, string subject, string body)
        {
            using var client = new SmtpClient(_smtpServer, _smtpPort)
            {
                Credentials = new NetworkCredential(_smtpUsername, _smtpPassword),
                EnableSsl = true
            };

            var message = new MailMessage
            {
                From = new MailAddress(_fromEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            message.To.Add(to);

            await client.SendMailAsync(message);
        }

        public async Task EnviarCodigoVerificacaoAsync(string email, string codigo)
        {
            var subject = "Código de Verificação - Troca de Email";
            var body = $@"
                <h2>Código de Verificação</h2>
                <p>Seu código de verificação para troca de email é: <strong>{codigo}</strong></p>
                <p>Este código é válido por 1 minuto.</p>
                <p>Se você não solicitou esta troca, por favor ignore este email.</p>";

            await EnviarEmailAsync(email, subject, body);
        }
    }
} 