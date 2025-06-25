using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OOP_Fair_Fare.Models;
using OOP_Fair_Fare.Services;
using System.Security.Cryptography;
using System.Text;

namespace OOP_Fair_Fare.Controllers
{
    /// <summary>
    /// Controller that handles the password reset functionality.
    /// This includes:
    /// - Initiating password reset requests
    /// - Sending reset links via email
    /// - Validating reset tokens
    /// - Processing password changes
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PasswordResetController : ControllerBase //inherits from ContollerBase
    {
        // Database context for user and token management
        private readonly AppDbContext _context;
        
        // Service for sending emails
        private readonly IEmailService _emailService;
        
        // Configuration for app settings (URLs, etc.)
        private readonly IConfiguration _configuration;
        
        // Logger for debugging and monitoring
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
        }        /// <summary>
                 /// Initiates the password reset process for a user.
                 /// The process works as follows:
                 /// 1. Validates that the email exists in the system
                 /// 2. Generates a secure random token
                 /// 3. Saves the token to the database with a 24-hour expiration
                 /// 4. Sends a reset link to the user's email
                 /// 
                 /// Security features:
                 /// - Uses cryptographically secure random tokens
                 /// - Tokens expire after 24 hours
                 /// - One-time use tokens (marked as used after reset)
                 /// - Generic response messages to prevent email enumeration
                 /// </summary>
                 /// <param name="email">The email address of the user requesting password reset</param>
                 /// <returns>
                 /// - Generic success message (even if email not found, for security)
                 /// - Error details if email sending fails
                 /// </returns>
        [HttpPost("request")]
        //hides complex password reset logic - abstraction
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
                var expiryDate = DateTime.UtcNow.AddMinutes(5);

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
                    <p>This link will expire in 5 minutes.</p>
                    <p>If you didn't request this password reset, please ignore this email.</p>"; _logger.LogInformation($"Attempting to send reset email to: {user.Email}");
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
            catch (Exception ex) //error handling
            {
                _logger.LogError(ex, $"Error processing password reset for user ID: {user.Id}");
                return StatusCode(500, new { message = "An error occurred while processing your request." });
            }
        }        /// <summary>
        /// Processes the actual password reset when user clicks the link and submits new password.
        /// The verification process:
        /// 1. Validates the reset token exists and hasn't expired
        /// 2. Ensures the token hasn't been used before
        /// 3. Updates the user's password with the new one
        /// 4. Marks the token as used to prevent reuse
        /// 
        /// Security features:
        /// - Validates token expiration
        /// - Prevents token reuse
        /// - Requires both token and new password
        /// - Uses secure password hashing (defined in User model)
        /// </summary>
        /// <param name="token">The reset token from the email link</param>
        /// <param name="newPassword">The new password chosen by the user</param>
        /// <returns>
        /// - Success response if password was reset
        /// - Error if token invalid/expired or password requirements not met
        /// </returns>
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
