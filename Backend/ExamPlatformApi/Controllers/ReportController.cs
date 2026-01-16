using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExamPlatform.API.DTOs;
using ExamPlatform.API.Repositories;
using ExamPlatform.API.Models;

namespace ExamPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin,teacher")]
public class ReportController : ControllerBase
{
    private readonly IResultRepository _resultRepository;
    private readonly IExamRepository _examRepository;
    private readonly IViolationRepository _violationRepository;
    private readonly IUserRepository _userRepository;

    public ReportController(
        IResultRepository resultRepository,
        IExamRepository examRepository,
        IViolationRepository violationRepository,
        IUserRepository userRepository)
    {
        _resultRepository = resultRepository;
        _examRepository = examRepository;
        _violationRepository = violationRepository;
        _userRepository = userRepository;
    }

    /// <summary>
    /// Get performance report for a specific exam
    /// </summary>
    [HttpGet("exam/{examId}/performance")]
    public async Task<ActionResult<ApiResponse<object>>> GetExamPerformanceReport(string examId)
    {
        var exam = await _examRepository.GetByIdAsync(examId);
        if (exam == null) return NotFound(new ApiResponse<object> { Success = false, Message = "Exam not found" });

        var results = await _resultRepository.GetByExamIdAsync(examId);
        if (results.Count == 0) return Ok(new ApiResponse<object> { Success = true, Data = new { message = "No results found" } });

        var stats = new
        {
            ExamTitle = exam.Title,
            TotalStudents = results.Count,
            PassedStudents = results.Count(r => r.IsPassed),
            FailedStudents = results.Count(r => !r.IsPassed),
            AverageScore = results.Average(r => r.Percentage),
            HighestScore = results.Max(r => r.MarksObtained),
            LowestScore = results.Min(r => r.MarksObtained)
        };

        return Ok(new ApiResponse<object> { Success = true, Data = stats });
    }

    /// <summary>
    /// Get violation summary for an exam
    /// </summary>
    [HttpGet("exam/{examId}/violations")]
    public async Task<ActionResult<ApiResponse<object>>> GetViolationReport(string examId)
    {
        var violations = await _violationRepository.GetByExamIdAsync(examId);
        
        var summary = violations
            .GroupBy(v => v.ViolationType)
            .Select(g => new { Type = g.Key, Count = g.Count() })
            .ToList();

        var studentViolations = violations
            .GroupBy(v => v.StudentId)
            .Select(g => new { StudentId = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(10) // Top 10 violators
            .ToList();

        return Ok(new ApiResponse<object> 
        { 
            Success = true, 
            Data = new 
            { 
                TotalViolations = violations.Count, 
                ByType = summary, 
                TopViolators = studentViolations 
            } 
        });
    }

    /// <summary>
    /// Get system-wide subscription usage report (Admin only)
    /// </summary>
    [HttpGet("system/subscriptions")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse<object>>> GetSubscriptionReport()
    {
        var users = await _userRepository.GetAllAsync();
        
        var stats = new
        {
            TotalUsers = users.Count,
            FreePlanUsers = users.Count(u => u.SubscriptionType == "free"),
            PaidPlanUsers = users.Count(u => u.SubscriptionType == "paid"),
            RevenueEstimate = users.Count(u => u.SubscriptionType == "paid") * 9.99 // Assuming $9.99 price
        };

        return Ok(new ApiResponse<object> { Success = true, Data = stats });
    }
}
