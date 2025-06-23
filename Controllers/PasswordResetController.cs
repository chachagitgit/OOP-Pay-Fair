using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OOP_Fair_Fare.Models;
using OOP_Fair_Fare.Services;
using System.Security.Cryptography;
using System.Text;

namespace OOP_Fair_Fare.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PasswordResetController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PasswordResetController> _logger;

        public PasswordResetController(
            AppDbContext context, 
            IEmailService emailService, 
            IConfiguration configuration,
            ILogger<PasswordResetController> logger)
        {
            _context = context;
            _emailService = emailService;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("request")]
        public async Task<IActionResult> RequestReset([FromForm] string email)
        {
            _logger.LogInformation($"Password reset requested for email: {email}");
              var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
            if (user == null)
            {
                _logger.LogWarning($"User not found for email: {email}");
                return BadRequest(new { message = "This email is not registered in our system. Please check your email or create a new account." });
            }

            _logger.LogInformation($"User found with ID: {user.Id}");

            try
            {
                // Generate a random token
                var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
                var expiryDate = DateTime.UtcNow.AddHours(24);

                var resetToken = new PasswordResetToken
                {
                    UserId = user.Id,
                    Token = token,
                    ExpiryDate = expiryDate,
                    IsUsed = false
                };

                _context.PasswordResetTokens.Add(resetToken);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Reset token created for user ID: {user.Id}");

                var baseUrl = _configuration["AppUrl"] ?? "https://localhost:5001";
                var resetLink = $"{baseUrl}/setNewPassword?token={Uri.EscapeDataString(token)}";
                var emailBody = $@"
                    <h2>Password Reset Request</h2>
                    <p>We received a request to reset your password. Click the link below to set a new password:</p>
                    <p><a href='{resetLink}'>Reset Password</a></p>
                    <p>This link will expire in 24 hours.</p>
                    <p>If you didn't request this password reset, please ignore this email.</p>";                _logger.LogInformation($"Attempting to send reset email to: {user.Email}");
                try
                {
                    await _emailService.SendEmailAsync(user.Email, "Password Reset Request", emailBody);
                    _logger.LogInformation($"Reset email sent successfully to: {user.Email}");
                    return Ok(new { message = "If the email exists, a reset link has been sent." });
                }
                catch (Exception emailEx)
                {
                    _logger.LogError(emailEx, $"Failed to send email to {user.Email}");
                    return StatusCode(500, new { message = $"Failed to send email: {emailEx.Message}" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing password reset for user ID: {user.Id}");
                return StatusCode(500, new { message = "An error occurred while processing your request." });
            }
        }

        [HttpPost("reset")]
        public async Task<IActionResult> ResetPassword([FromForm] string token, [FromForm] string newPassword)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(newPassword))
            {
                return BadRequest(new { message = "Token and new password are required." });
            }

            var resetToken = await _context.PasswordResetTokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Token == token && !t.IsUsed && t.ExpiryDate > DateTime.UtcNow);

            if (resetToken == null)
            {
                return BadRequest(new { message = "Invalid or expired reset token." });
            }            if (resetToken.User == null)
            {
                return BadRequest(new { message = "Associated user not found." });
            }

            // Hash the new password using SHA256 (same as login)
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(newPassword));
            resetToken.User.HashedPassword = BitConverter.ToString(bytes).Replace("-", "").ToLower();

            // Mark token as used
            resetToken.IsUsed = true;
            
            await _context.SaveChangesAsync();

            return Ok(new { message = "Password has been successfully reset." });
        }
    }
}
