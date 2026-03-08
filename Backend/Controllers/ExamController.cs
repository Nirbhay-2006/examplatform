using ExamNest.Models.DTOs;
using ExamNest.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExamNest.Controllers
{
    [ApiController]
    [Route("api/exam")]
    public class ExamController : ControllerBase
    {
        private readonly IExamAntiCheatingService _antiCheatingService;

        public ExamController(IExamAntiCheatingService antiCheatingService)
        {
            _antiCheatingService = antiCheatingService;
        }

        [HttpPost("start-session")]
        public async Task<IActionResult> StartSession([FromBody] StartExamSessionRequestDto request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _antiCheatingService.StartSessionAsync(request, cancellationToken);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPost("log-violation")]
        public async Task<IActionResult> LogViolation([FromBody] LogViolationRequestDto request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _antiCheatingService.LogViolationAsync(request, cancellationToken);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("auto-submit")]
        public async Task<IActionResult> AutoSubmit([FromBody] AutoSubmitRequestDto request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _antiCheatingService.AutoSubmitAsync(request, cancellationToken);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("monitoring-event")]
        public async Task<IActionResult> MonitoringEvent([FromBody] MonitoringEventRequestDto request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _antiCheatingService.LogMonitoringEventAsync(request, cancellationToken);
                return Ok(new { logged = true });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("session/{sessionId:guid}")]
        public async Task<IActionResult> GetSession([FromRoute] Guid sessionId, CancellationToken cancellationToken)
        {
            var session = await _antiCheatingService.GetSessionAsync(sessionId, cancellationToken);
            if (session == null)
            {
                return NotFound(new { message = "Session not found." });
            }

            return Ok(session);
        }

        [HttpGet("violations")]
        public async Task<IActionResult> GetViolations(
            [FromQuery(Name = "exam_id")] int? examId,
            [FromQuery(Name = "user_id")] int? userId,
            [FromQuery] DateTime? date,
            [FromQuery] int page = 1,
            [FromQuery(Name = "page_size")] int pageSize = 50,
            CancellationToken cancellationToken = default)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 200);

            var logs = await _antiCheatingService.GetViolationsAsync(examId, userId, date, page, pageSize, cancellationToken);
            return Ok(logs);
        }
    }
}
