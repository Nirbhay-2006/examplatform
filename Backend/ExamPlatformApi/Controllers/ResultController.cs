using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ExamPlatform.API.DTOs;
using ExamPlatform.API.Models;
using ExamPlatform.API.Repositories;

namespace ExamPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ResultController : ControllerBase
{
    private readonly IResultRepository _resultRepository;
    private readonly IResponseRepository _responseRepository;
    private readonly IExamRepository _examRepository;
    private readonly IQuestionRepository _questionRepository;

    public ResultController(
        IResultRepository resultRepository,
        IResponseRepository responseRepository,
        IExamRepository examRepository,
        IQuestionRepository questionRepository)
    {
        _resultRepository = resultRepository;
        _responseRepository = responseRepository;
        _examRepository = examRepository;
        _questionRepository = questionRepository;
    }

    private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
    private string GetUserRole() => User.FindFirst(ClaimTypes.Role)?.Value ?? "";

    // Teacher: Publish Result
    [HttpPost("{examId}/publish")]
    [Authorize(Roles = "teacher,admin")]
    public async Task<ActionResult<ApiResponse<object>>> PublishResult(string examId)
    {
        var exam = await _examRepository.GetByIdAsync(examId);
        if (exam == null)
            return NotFound(new ApiResponse<object> { Success = false, Message = "Exam not found" });

        if (exam.TeacherId != GetUserId() && GetUserRole() != "admin")
            return Forbid();

        var responses = await _responseRepository.GetByExamIdAsync(examId);
        var questions = await _questionRepository.GetByExamIdAsync(examId);

        foreach (var response in responses.Where(r => r.Status == "submitted"))
        {
            var totalMarks = 0;
            foreach (var answer in response.Answers)
            {
                totalMarks += answer.MarksObtained;
            }

            var percentage = (double)totalMarks / exam.TotalMarks * 100;
            var result = new Result
            {
                ExamId = examId,
                StudentId = response.StudentId,
                ResponseId = response.Id!,
                TotalMarks = exam.TotalMarks,
                MarksObtained = totalMarks,
                Percentage = percentage,
                IsPassed = totalMarks >= exam.PassingMarks
            };

            await _resultRepository.CreateAsync(result);
            response.Status = "evaluated";
            await _responseRepository.UpdateAsync(response);
        }

        exam.IsPublished = true;
        await _examRepository.UpdateAsync(exam);
        return Ok(new ApiResponse<object> { Success = true, Message = "Results published successfully" });
    }

    // Student: Get My Results
    [HttpGet("my-results")]
    [Authorize(Roles = "student")]
    public async Task<ActionResult<ApiResponse<List<Result>>>> GetMyResults()
    {
        var results = await _resultRepository.GetByStudentIdAsync(GetUserId());
        return Ok(new ApiResponse<List<Result>> { Success = true, Data = results });
    }

    // Student: Get Result by Exam
    [HttpGet("exam/{examId}")]
    [Authorize(Roles = "student")]
    public async Task<ActionResult<ApiResponse<Result>>> GetResultByExam(string examId)
    {
        var result = await _resultRepository.GetByExamAndStudentAsync(examId, GetUserId());
        if (result == null)
            return NotFound(new ApiResponse<Result> { Success = false, Message = "Result not found" });

        return Ok(new ApiResponse<Result> { Success = true, Data = result });
    }

    // Teacher: Evaluate Subjective Answer
    [HttpPost("evaluate")]
    [Authorize(Roles = "teacher,admin")]
    public async Task<ActionResult<ApiResponse<object>>> EvaluateAnswer([FromBody] EvaluateAnswerRequest request)
    {
        var response = await _responseRepository.GetByIdAsync(request.ResponseId);
        if (response == null)
            return NotFound(new ApiResponse<object> { Success = false, Message = "Response not found" });

        var answer = response.Answers.FirstOrDefault(a => a.QuestionId == request.QuestionId);
        if (answer == null)
            return NotFound(new ApiResponse<object> { Success = false, Message = "Answer not found" });

        answer.MarksObtained = request.MarksObtained;
        await _responseRepository.UpdateAsync(response);
        return Ok(new ApiResponse<object> { Success = true, Message = "Answer evaluated" });
    }
}

public class EvaluateAnswerRequest
{
    public string ResponseId { get; set; } = string.Empty;
    public string QuestionId { get; set; } = string.Empty;
    public int MarksObtained { get; set; }
}
