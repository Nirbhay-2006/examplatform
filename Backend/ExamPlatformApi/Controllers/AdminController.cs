using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExamPlatform.API.DTOs;
using ExamPlatform.API.Repositories;
using ExamPlatform.API.Services;

namespace ExamPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin")]
public class AdminController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IExamRepository _examRepository;
    // private readonly IPaymentRepository _paymentRepository;
    private readonly ISystemConfigRepository _configRepository;
    private readonly IAuditService _auditService;

    public AdminController(
        IUserRepository userRepository, 
        IExamRepository examRepository, 
        // IPaymentRepository paymentRepository,
        ISystemConfigRepository configRepository,
        IAuditService auditService)
    {
        _userRepository = userRepository;
        _examRepository = examRepository;
        // _paymentRepository = paymentRepository;
        _configRepository = configRepository;
        _auditService = auditService;
    }

    [HttpGet("users")]
    public async Task<ActionResult<ApiResponse<List<Models.User>>>> GetAllUsers()
    {
        var users = await _userRepository.GetAllAsync();
        return Ok(new ApiResponse<List<Models.User>> { Success = true, Data = users });
    }

    [HttpPut("users/{userId}/activate")]
    public async Task<ActionResult<ApiResponse<object>>> ActivateUser(string userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return NotFound(new ApiResponse<object> { Success = false, Message = "User not found" });

        user.IsActive = true;
        await _userRepository.UpdateAsync(user);
        return Ok(new ApiResponse<object> { Success = true, Message = "User activated" });
    }

    [HttpPut("users/{userId}/deactivate")]
    public async Task<ActionResult<ApiResponse<object>>> DeactivateUser(string userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return NotFound(new ApiResponse<object> { Success = false, Message = "User not found" });

        user.IsActive = false;
        await _userRepository.UpdateAsync(user);
        return Ok(new ApiResponse<object> { Success = true, Message = "User deactivated" });
    }

    [HttpPut("users/{userId}/role")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateUserRole(string userId, [FromBody] string role)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return NotFound(new ApiResponse<object> { Success = false, Message = "User not found" });

        user.Role = role;
        await _userRepository.UpdateAsync(user);
        return Ok(new ApiResponse<object> { Success = true, Message = "Role updated" });
    }

    [HttpGet("payments")]
    public async Task<ActionResult<ApiResponse<object>>> GetAllPayments()
    {
        // This would need to fetch all payments - simplified for now
        return Ok(new ApiResponse<object> { Success = true, Message = "Payment logs" });
    }

    [HttpGet("dashboard")]
    public async Task<ActionResult<ApiResponse<object>>> GetDashboard()
    {
        var users = await _userRepository.GetAllAsync();
        var exams = await _examRepository.GetAllAsync();

        var stats = new
        {
            totalUsers = users.Count,
            totalStudents = users.Count(u => u.Role == "student"),
            totalTeachers = users.Count(u => u.Role == "teacher"),
            totalExams = exams.Count,
            paidUsers = users.Count(u => u.SubscriptionType == "paid")
        };

        return Ok(new ApiResponse<object> { Success = true, Data = stats });
    }

    [HttpGet("config")]
    public async Task<ActionResult<ApiResponse<Models.SystemConfig>>> GetSystemConfig()
    {
        var config = await _configRepository.GetConfigAsync();
        return Ok(new ApiResponse<Models.SystemConfig> { Success = true, Data = config });
    }

    [HttpPut("config")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateSystemConfig([FromBody] Models.SystemConfig config)
    {
        // Ensure ID is preserved or handled by repo
        await _configRepository.UpdateConfigAsync(config);
        
        // Audit log
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "admin";
        await _auditService.LogAsync("UPDATE_CONFIG", userId, "Updated system configuration", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown");

        return Ok(new ApiResponse<object> { Success = true, Message = "Configuration updated successfully" });
    }

    [HttpGet("audit-logs")]
    public async Task<ActionResult<ApiResponse<List<Models.AuditLog>>>> GetAuditLogs([FromQuery] int limit = 100)
    {
        var logs = await _auditService.GetLogsAsync(limit);
        return Ok(new ApiResponse<List<Models.AuditLog>> { Success = true, Data = logs });
    }
}
