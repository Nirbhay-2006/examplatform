using ExamNest.Data;
using ExamNest.Models.DTOs.Student;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ExamNest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private static readonly string[] ValidOrderStatuses = { "SUCCESS", "PAID", "COMPLETED" };
        private static readonly string[] ValidPaymentStatuses = { "SUCCESS", "PAID", "CAPTURED", "COMPLETED" };

        private readonly AppDbContext _context;

        public StudentController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("published-courses")]
        public async Task<IActionResult> GetPublishedCourses()
        {
            var courses = await _context.Courses
                .Where(c => c.IsPublished)
                .Select(c => new CourseCardStudentDTO
                {
                    CourseId = c.CourseId,
                    Title = c.Title,
                    ThumbnailUrl = c.ThumbailUrl,
                    Fees = c.Fees,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate
                })
                .ToListAsync();

            return Ok(courses);
        }

        [HttpGet("my-subscribed-courses")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMySubscribedCourses()
        {
            var studentId = GetCurrentUserId();
            if (studentId == null) return Unauthorized("Invalid token.");

            var now = DateTime.UtcNow;
            var subscriptions = await _context.Subscriptions
                .Where(s =>
                    s.UserId == studentId.Value &&
                    (s.IsActive ?? false) &&
                    (s.EndDate == null || s.EndDate >= now))
                .ToListAsync();

            if (subscriptions.Count == 0) return Ok(new List<CourseCardStudentDTO>());

            var validOrderIds = await _context.Orders
                .Where(o =>
                    o.UserId == studentId.Value &&
                    o.CourseId != null &&
                    subscriptions.Select(s => s.OrderId).Contains(o.OrderId) &&
                    ValidOrderStatuses.Contains((o.Status ?? string.Empty).ToUpper()))
                .Select(o => o.OrderId)
                .Distinct()
                .ToListAsync();

            var paidOrderIds = await _context.Payments
                .Where(p =>
                    validOrderIds.Contains(p.OrderId) &&
                    ValidPaymentStatuses.Contains((p.Status ?? string.Empty).ToUpper()))
                .Select(p => p.OrderId)
                .Distinct()
                .ToListAsync();

            var allowedCourseIds = subscriptions
                .Where(s => paidOrderIds.Contains(s.OrderId))
                .Select(s => s.CourseId)
                .Distinct()
                .ToList();

            var courses = await _context.Courses
                .Where(c => allowedCourseIds.Contains(c.CourseId))
                .Select(c => new CourseCardStudentDTO
                {
                    CourseId = c.CourseId,
                    Title = c.Title,
                    ThumbnailUrl = c.ThumbailUrl,
                    Fees = c.Fees,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate
                })
                .ToListAsync();

            return Ok(courses);
        }

        [HttpGet("subscribed-course/{courseId:int}/details")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetSubscribedCourseDetails(int courseId)
        {
            var studentId = GetCurrentUserId();
            if (studentId == null) return Unauthorized("Invalid token.");

            var isAllowed = await HasValidSubscriptionAccess(studentId.Value, courseId);
            if (!isAllowed) return Forbid("No valid paid subscription found for this course.");

            var course = await _context.Courses
                .Where(c => c.CourseId == courseId)
                .Select(c => new
                {
                    c.CourseId,
                    c.Title,
                    c.Description,
                    c.StartDate,
                    c.EndDate,
                    c.Fees,
                    c.ThumbailUrl,
                    c.IsPublished,
                    Videos = _context.CourseMedias
                        .Where(m => m.CourseId == c.CourseId)
                        .Select(m => new
                        {
                            m.CourseMediaId,
                            m.FileName,
                            m.FileType,
                            m.FilePath
                        })
                        .ToList(),
                    Exams = _context.CourseExams
                        .Where(e => e.CourseId == c.CourseId && e.IsPublished)
                        .OrderBy(e => e.StartTime)
                        .Select(e => new
                        {
                            e.CourseExamId,
                            e.Title,
                            e.Description,
                            e.StartTime,
                            e.EndTime,
                            e.DurationMinutes
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (course == null) return NotFound("Course not found.");
            return Ok(course);
        }

        [HttpGet("subscribed-course/{courseId:int}/exams")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetSubscribedCourseExams(int courseId)
        {
            var studentId = GetCurrentUserId();
            if (studentId == null) return Unauthorized("Invalid token.");

            var isAllowed = await HasValidSubscriptionAccess(studentId.Value, courseId);
            if (!isAllowed) return Forbid("No valid paid subscription found for this course.");

            var exams = await _context.CourseExams
                .Where(e => e.CourseId == courseId && e.IsPublished)
                .OrderBy(e => e.StartTime)
                .Select(e => new
                {
                    e.CourseExamId,
                    e.Title,
                    e.Description,
                    e.StartTime,
                    e.EndTime,
                    e.DurationMinutes
                })
                .ToListAsync();

            return Ok(exams);
        }

        private async Task<bool> HasValidSubscriptionAccess(int studentId, int courseId)
        {
            var now = DateTime.UtcNow;
            var validSubscriptionOrderIds = await _context.Subscriptions
                .Where(s =>
                    s.UserId == studentId &&
                    s.CourseId == courseId &&
                    (s.IsActive ?? false) &&
                    (s.EndDate == null || s.EndDate >= now))
                .Select(s => s.OrderId)
                .Distinct()
                .ToListAsync();

            if (validSubscriptionOrderIds.Count == 0) return false;

            var validOrderIds = await _context.Orders
                .Where(o =>
                    validSubscriptionOrderIds.Contains(o.OrderId) &&
                    o.UserId == studentId &&
                    o.CourseId == courseId &&
                    ValidOrderStatuses.Contains((o.Status ?? string.Empty).ToUpper()))
                .Select(o => o.OrderId)
                .Distinct()
                .ToListAsync();

            if (validOrderIds.Count == 0) return false;

            var hasPaidPayment = await _context.Payments.AnyAsync(p =>
                validOrderIds.Contains(p.OrderId) &&
                ValidPaymentStatuses.Contains((p.Status ?? string.Empty).ToUpper()));

            return hasPaidPayment;
        }

        private int? GetCurrentUserId()
        {
            var claimValue =
                User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
                User.FindFirstValue("sub");

            return int.TryParse(claimValue, out var id) ? id : null;
        }
    }
}
