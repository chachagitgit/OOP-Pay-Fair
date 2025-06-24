using System;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace OOP_Fair_Fare.Services
{
    public interface IEmailService //hides implementation details - abstraction
    {
        Task SendEmailAsync(string to, string subject, string body);
    }

    public class EmailService : IEmailService //inherits IEmailService interface
    {
        //encap
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _fromEmail;
        private readonly string _password;
        private readonly ILogger<EmailService> _logger;        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
            try
            {
                _logger.LogInformation("Initializing EmailService");
                
                _smtpServer = configuration["Email:SmtpServer"] ?? throw new ArgumentException("SMTP server not configured");
                _smtpPort = int.Parse(configuration["Email:SmtpPort"] ?? throw new ArgumentException("SMTP port not configured"));
                _fromEmail = configuration["Email:FromEmail"] ?? throw new ArgumentException("From email not configured");
                _password = configuration["Email:Password"] ?? throw new ArgumentException("Email password not configured");
                
                _logger.LogInformation($"EmailService initialized with SMTP server: {_smtpServer}, port: {_smtpPort}, from: {_fromEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize EmailService");
                throw;
            }
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                _logger.LogInformation($"Creating SMTP client for {_smtpServer}:{_smtpPort}");
                  using var client = new SmtpClient(_smtpServer, _smtpPort)
                {
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_fromEmail, _password),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Timeout = 30000 // 30 seconds timeout
                };
                
                // Log connection details (without password)
                _logger.LogInformation($"SMTP Client configured with: Server={_smtpServer}, Port={_smtpPort}, SSL=enabled, Username={_fromEmail}");

                _logger.LogInformation("Creating mail message");
                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(_fromEmail),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                
                mailMessage.To.Add(to);

                _logger.LogInformation($"Attempting to send email via {_smtpServer} to {to}");
                try
                {
                    await client.SendMailAsync(mailMessage);
                    _logger.LogInformation($"Email sent successfully to {to}");
                }
                catch (SmtpException smtpEx)
                {
                    _logger.LogError(smtpEx, $"SMTP error while sending email to {to}. Status code: {smtpEx.StatusCode}, Message: {smtpEx.Message}");
                    throw new Exception($"Failed to send email. SMTP error: {smtpEx.Message}", smtpEx);
                }            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {to}. Error: {ex.Message}");
                throw new Exception("Failed to send email. Please try again.", ex);
            }
        }
    }
}
