using Microsoft.AspNetCore.Mvc;
using ExamPlatform.API.DTOs;
using ExamPlatform.API.Models;
using ExamPlatform.API.Repositories;
using ExamPlatform.API.Services;
using ExamPlatform.API.Helpers;
using BCrypt.Net;

namespace ExamPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly IJwtService _jwtService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IUserRepository userRepository, 
        IEmailService emailService, 
        IJwtService jwtService,
        ILogger<AuthController> logger)
    {
        _userRepository = userRepository;
        _emailService = emailService;
        _jwtService = jwtService;
        _logger = logger;
    }

    /// <summary>
    /// Register a new user (student or teacher)
    /// </summary>
    /// <param name="request">Registration request containing user details</param>
    /// <returns>Success response with verification message</returns>
    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<object>>> Register([FromBody] RegisterRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object> 
                { 
                    Success = false, 
                    Message = "Invalid input data",
                    Data = ModelState
                });
            }

            _logger.LogInformation("Registration attempt for email: {Email}", request.Email);

            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
            {
                _logger.LogWarning("Registration failed: Email already exists - {Email}", request.Email);
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Email already registered" });
            }

            var otpCode = OtpHelper.GenerateOtp();
            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = request.Role,
                OtpCode = otpCode,
                OtpExpiry = DateTime.UtcNow.AddMinutes(10)
            };

            await _userRepository.CreateAsync(user);
            await _emailService.SendOtpEmailAsync(user.Email, user.Name, otpCode);

            _logger.LogInformation("User registered successfully: {Email}, Role: {Role}", user.Email, user.Role);

            return Ok(new ApiResponse<object> { Success = true, Message = "Registration successful. Please verify your email." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for email: {Email}", request.Email);
            throw;
        }
    }

    /// <summary>
    /// Verify OTP code sent to user's email
    /// </summary>
    /// <param name="request">OTP verification request</param>
    /// <returns>Success response if OTP is valid</returns>
    [HttpPost("verify-otp")]
    public async Task<ActionResult<ApiResponse<object>>> VerifyOtp([FromBody] VerifyOtpRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object> 
                { 
                    Success = false, 
                    Message = "Invalid input data",
                    Data = ModelState
                });
            }

            _logger.LogInformation("OTP verification attempt for email: {Email}", request.Email);

            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
            {
                _logger.LogWarning("OTP verification failed: User not found - {Email}", request.Email);
                return NotFound(new ApiResponse<object> { Success = false, Message = "User not found" });
            }

            if (user.OtpCode != request.OtpCode || user.OtpExpiry < DateTime.UtcNow)
            {
                _logger.LogWarning("OTP verification failed: Invalid or expired OTP for email: {Email}", request.Email);
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Invalid or expired OTP" });
            }

            user.IsVerified = true;
            user.OtpCode = null;
            user.OtpExpiry = null;
            await _userRepository.UpdateAsync(user);

            _logger.LogInformation("Email verified successfully: {Email}", request.Email);

            return Ok(new ApiResponse<object> { Success = true, Message = "Email verified successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during OTP verification for email: {Email}", request.Email);
            throw;
        }
    }

    /// <summary>
    /// Login user and return JWT token
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>JWT token and user information</returns>
    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<LoginResponse> 
                { 
                    Success = false, 
                    Message = "Invalid input data",
                    Data = null
                });
            }

            _logger.LogInformation("Login attempt for email: {Email}", request.Email);

            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                _logger.LogWarning("Login failed: Invalid credentials for email: {Email}", request.Email);
                return Unauthorized(new ApiResponse<LoginResponse> { Success = false, Message = "Invalid credentials" });
            }

            if (!user.IsVerified)
            {
                _logger.LogWarning("Login failed: Email not verified for email: {Email}", request.Email);
                return BadRequest(new ApiResponse<LoginResponse> { Success = false, Message = "Please verify your email first" });
            }

            if (!user.IsActive)
            {
                _logger.LogWarning("Login failed: Account inactive for email: {Email}", request.Email);
                return BadRequest(new ApiResponse<LoginResponse> { Success = false, Message = "Account is inactive" });
            }

            var token = _jwtService.GenerateToken(user);
            var response = new LoginResponse
            {
                Token = token,
                UserId = user.Id!,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                SubscriptionType = user.SubscriptionType
            };

            _logger.LogInformation("Login successful for email: {Email}, Role: {Role}", user.Email, user.Role);

            return Ok(new ApiResponse<LoginResponse> { Success = true, Message = "Login successful", Data = response });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email: {Email}", request.Email);
            throw;
        }
    }

    /// <summary>
    /// Resend OTP code to user's email
    /// </summary>
    /// <param name="email">User's email address</param>
    /// <returns>Success response if OTP is sent</returns>
    [HttpPost("resend-otp")]
    public async Task<ActionResult<ApiResponse<object>>> ResendOtp([FromBody] string email)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Email is required" });
            }

            _logger.LogInformation("Resend OTP request for email: {Email}", email);

            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning("Resend OTP failed: User not found - {Email}", email);
                return NotFound(new ApiResponse<object> { Success = false, Message = "User not found" });
            }

            var otpCode = OtpHelper.GenerateOtp();
            user.OtpCode = otpCode;
            user.OtpExpiry = DateTime.UtcNow.AddMinutes(10);
            await _userRepository.UpdateAsync(user);
            await _emailService.SendOtpEmailAsync(user.Email, user.Name, otpCode);

            _logger.LogInformation("OTP resent successfully to email: {Email}", email);

            return Ok(new ApiResponse<object> { Success = true, Message = "OTP sent successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during resend OTP for email: {Email}", email);
            throw;
        }
    }

    /// <summary>
    /// Initiate forgot password flow by sending an OTP to user's email
    /// </summary>
    [HttpPost("forgot-password")]
    public async Task<ActionResult<ApiResponse<object>>> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid input data",
                    Data = ModelState
                });
            }

            _logger.LogInformation("Forgot password request for email: {Email}", request.Email);

            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
            {
                // Do not reveal whether the email exists
                _logger.LogWarning("Forgot password requested for non-existing email: {Email}", request.Email);
                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "If an account exists for this email, a reset code has been sent."
                });
            }

            var otpCode = OtpHelper.GenerateOtp();
            user.OtpCode = otpCode;
            user.OtpExpiry = DateTime.UtcNow.AddMinutes(10);
            await _userRepository.UpdateAsync(user);

            await _emailService.SendPasswordResetEmailAsync(user.Email, user.Name, otpCode);

            _logger.LogInformation("Password reset OTP generated for email: {Email}", request.Email);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "If an account exists for this email, a reset code has been sent."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during forgot password for email: {Email}", request.Email);
            throw;
        }
    }

    /// <summary>
    /// Complete password reset using OTP code
    /// </summary>
    [HttpPost("reset-password")]
    public async Task<ActionResult<ApiResponse<object>>> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid input data",
                    Data = ModelState
                });
            }

            _logger.LogInformation("Reset password attempt for email: {Email}", request.Email);

            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
            {
                _logger.LogWarning("Reset password failed: User not found - {Email}", request.Email);
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Invalid reset code or email" });
            }

            if (user.OtpCode != request.OtpCode || user.OtpExpiry < DateTime.UtcNow)
            {
                _logger.LogWarning("Reset password failed: Invalid or expired OTP for email: {Email}", request.Email);
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Invalid or expired reset code" });
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.OtpCode = null;
            user.OtpExpiry = null;
            await _userRepository.UpdateAsync(user);

            _logger.LogInformation("Password reset successful for email: {Email}", request.Email);

            return Ok(new ApiResponse<object> { Success = true, Message = "Password has been reset successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during reset password for email: {Email}", request.Email);
            throw;
        }
    }
}
