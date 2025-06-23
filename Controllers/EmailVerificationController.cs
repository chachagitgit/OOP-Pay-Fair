using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OOP_Fair_Fare.Models;
using System.Net.Mail;
using System.Net;

namespace OOP_Fair_Fare.Controllers
{
    /// <summary>
    /// Controller responsible for handling email verification during user registration.
    /// This includes generating OTP (One-Time Password) codes, sending verification emails,
    /// and validating OTP codes entered by users.
    /// </summary>
    [Route("api")]
    [ApiController]
    public class EmailVerificationController : ControllerBase
    {
        // Database context for accessing and manipulating data
        private readonly AppDbContext _context;
        
        // Random number generator for OTP creation
        private readonly Random _random;
        
        // Configuration service to access email settings from appsettings.json
        private readonly IConfiguration _configuration;

        public EmailVerificationController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _random = new Random();
            _configuration = configuration;
        }        /// <summary>
        /// Endpoint for sending OTP verification codes via email.
        /// This method is called when a user signs up and needs to verify their email.
        /// The process:
        /// 1. Generates a random 6-digit OTP
        /// 2. Saves the OTP to the database with a 5-minute expiration
        /// 3. Sends the OTP to the user's email using SMTP
        /// </summary>
        /// <param name="request">Contains the user's email address</param>
        /// <returns>Success response if email sent, error response if failed</returns>
        [HttpPost("send/otp")]
        public async Task<IActionResult> SendOTP([FromBody] EmailRequest request)
        {
            try
            {
                var smtpServer = _configuration["Email:SmtpServer"] ?? throw new InvalidOperationException("SMTP server not configured");
                var smtpPort = _configuration["Email:SmtpPort"] ?? throw new InvalidOperationException("SMTP port not configured");
                var fromEmail = _configuration["Email:FromEmail"] ?? throw new InvalidOperationException("From email not configured");
                var password = _configuration["Email:Password"] ?? throw new InvalidOperationException("Email password not configured");

                // Generate 6-digit OTP
                string otp = _random.Next(100000, 999999).ToString();

                // Save OTP to database
                var verification = new EmailVerification
                {
                    Email = request.Email,
                    OTP = otp,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                    IsUsed = false
                };

                _context.EmailVerifications.Add(verification);
                await _context.SaveChangesAsync();

                using (var client = new SmtpClient(smtpServer, int.Parse(smtpPort)))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(fromEmail, password);

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(fromEmail),
                        Subject = "Fair Fare - Email Verification",
                        Body = $"Your verification code is: {otp}\nThis code will expire in 5 minutes.",
                        IsBodyHtml = false
                    };
                    mailMessage.To.Add(request.Email);

                    await client.SendMailAsync(mailMessage);
                }

                return Ok(new { success = true });
            }            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed to send OTP: {ex.Message}");
                Console.Error.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { success = false, message = $"Failed to send OTP: {ex.Message}" });
            }
        }        /// <summary>
        /// Endpoint for verifying the OTP code entered by the user.
        /// This method is called when a user submits the OTP code from the verification modal.
        /// The verification process:
        /// 1. Finds the most recent unused OTP for the email
        /// 2. Checks if the OTP exists and hasn't expired
        /// 3. Validates the entered code against the stored OTP
        /// 4. Marks the OTP as used if validation succeeds
        /// </summary>
        /// <param name="request">Contains the email and OTP code entered by user</param>
        /// <returns>
        /// - Success response if OTP is valid
        /// - Bad request if OTP is invalid, expired, or not found
        /// </returns>
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOTP([FromBody] OTPVerificationRequest request)
        {
            var verification = _context.EmailVerifications
                .Where(v => v.Email == request.Email && !v.IsUsed)
                .OrderByDescending(v => v.CreatedAt)
                .FirstOrDefault();

            if (verification == null)
            {
                return BadRequest(new { success = false, message = "No verification code found" });
            }

            if (DateTime.UtcNow > verification.ExpiresAt)
            {
                return BadRequest(new { success = false, message = "Verification code has expired" });
            }

            if (verification.OTP != request.OTP)
            {
                return BadRequest(new { success = false, message = "Invalid verification code" });
            }

            verification.IsUsed = true;
            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }
    }    public class EmailRequest
    {
        public required string Email { get; set; }
    }

    public class OTPVerificationRequest
    {
        public required string Email { get; set; }
        public required string OTP { get; set; }
    }
}
