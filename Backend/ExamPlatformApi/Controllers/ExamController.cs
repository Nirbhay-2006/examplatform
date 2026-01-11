using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Linq;
using ExamPlatform.API.DTOs;
using ExamPlatform.API.Models;
using ExamPlatform.API.Repositories;
using ExamPlatform.API.Services;
using ExamPlatform.API.Helpers;

namespace ExamPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExamController : ControllerBase
{
    private readonly IExamRepository _examRepository;
    private readonly IQuestionRepository _questionRepository;
    private readonly IResponseRepository _responseRepository;
    private readonly IViolationRepository _violationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IFileParserService _fileParserService;
    private readonly ILogger<ExamController> _logger;

    public ExamController(
        IExamRepository examRepository,
        IQuestionRepository questionRepository,
        IResponseRepository responseRepository,
        IViolationRepository violationRepository,
        IUserRepository userRepository,
        IFileParserService fileParserService,
        ILogger<ExamController> logger)
    {
        _examRepository = examRepository;
        _questionRepository = questionRepository;
        _responseRepository = responseRepository;
        _violationRepository = violationRepository;
        _userRepository = userRepository;
        _fileParserService = fileParserService;
        _logger = logger;
    }

    private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
    private string GetUserRole() => User.FindFirst(ClaimTypes.Role)?.Value ?? "";
    private string GetSubscriptionType() => User.FindFirst("subscriptionType")?.Value ?? "free";

    /// <summary>
    /// Create a new exam (Teacher/Admin only)
    /// </summary>
    /// <param name="request">Exam creation request</param>
    /// <returns>Created exam details</returns>
    [HttpPost]
    [Authorize(Roles = "teacher,admin")]
    public async Task<ActionResult<ApiResponse<Exam>>> CreateExam([FromBody] CreateExamRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<Exam> 
                { 
                    Success = false, 
                    Message = "Invalid input data",
                    Data = null
                });
            }

            _logger.LogInformation("Creating exam: {Title} by user: {UserId}", request.Title, GetUserId());

        var exam = new Exam
        {
            Title = request.Title,
            Description = request.Description,
            TeacherId = GetUserId(),
            Duration = request.Duration,
            TotalMarks = request.TotalMarks,
            PassingMarks = request.PassingMarks,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            IsPaid = request.IsPaid,
            MaxViolations = request.MaxViolations
        };

        await _examRepository.CreateAsync(exam);
            
            _logger.LogInformation("Exam created successfully: {ExamId}, Title: {Title}", exam.Id, exam.Title);
            
        return Ok(new ApiResponse<Exam> { Success = true, Message = "Exam created successfully", Data = exam });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating exam: {Title}", request.Title);
            throw;
        }
    }

    // Teacher: Add Questions to Exam
    [HttpPost("{examId}/questions")]
    [Authorize(Roles = "teacher,admin")]
    public async Task<ActionResult<ApiResponse<Question>>> AddQuestion(string examId, [FromBody] CreateQuestionRequest request)
    {
        var exam = await _examRepository.GetByIdAsync(examId);
        if (exam == null)
            return NotFound(new ApiResponse<Question> { Success = false, Message = "Exam not found" });

        if (exam.TeacherId != GetUserId() && GetUserRole() != "admin")
            return Forbid();

        var questions = await _questionRepository.GetByExamIdAsync(examId);
        var question = new Question
        {
            ExamId = examId,
            QuestionText = request.QuestionText,
            QuestionType = request.QuestionType,
            Options = request.Options,
            CorrectAnswer = request.CorrectAnswer,
            Marks = request.Marks,
            Order = questions.Count + 1
        };

        await _questionRepository.CreateAsync(question);
        return Ok(new ApiResponse<Question> { Success = true, Message = "Question added successfully", Data = question });
    }

    /// <summary>
    /// Upload questions from PDF or Excel file (Teacher/Admin only)
    /// </summary>
    [HttpPost("{examId}/upload-questions")]
    [Authorize(Roles = "teacher,admin")]
    public async Task<ActionResult<ApiResponse<object>>> UploadQuestions(
        string examId, 
        IFormFile file,
        [FromForm] int? maxQuestions)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new ApiResponse<object> 
                { 
                    Success = false, 
                    Message = "No file uploaded" 
                });
            }

            var exam = await _examRepository.GetByIdAsync(examId);
            if (exam == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Exam not found" });

            if (exam.TeacherId != GetUserId() && GetUserRole() != "admin")
                return Forbid();

            // Validate file type
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (extension != ".pdf" && extension != ".xlsx" && extension != ".xls")
            {
                return BadRequest(new ApiResponse<object> 
                { 
                    Success = false, 
                    Message = "Invalid file type. Please upload PDF or Excel files (.pdf, .xlsx, .xls)" 
                });
            }

            // Validate file size (max 10MB)
            if (file.Length > 10 * 1024 * 1024)
            {
                return BadRequest(new ApiResponse<object> 
                { 
                    Success = false, 
                    Message = "File size exceeds 10MB limit" 
                });
            }

            _logger.LogInformation("Parsing questions from file: {FileName} for exam: {ExamId}", file.FileName, examId);

            // Parse questions from file
            var questions = await _fileParserService.ParseQuestionsFromFileAsync(file, examId);

            // Limit number of questions if requested
            if (maxQuestions.HasValue && maxQuestions.Value > 0)
            {
                questions = questions.Take(maxQuestions.Value).ToList();
            }

            if (questions.Count == 0)
            {
                return BadRequest(new ApiResponse<object> 
                { 
                    Success = false, 
                    Message = "No questions found in the file. Please check the file format." 
                });
            }

            // Get existing questions to determine starting order
            var existingQuestions = await _questionRepository.GetByExamIdAsync(examId);
            int startOrder = existingQuestions.Count + 1;

            // Save questions to database
            foreach (var question in questions)
            {
                question.Order = startOrder++;
                await _questionRepository.CreateAsync(question);
            }

            _logger.LogInformation("Successfully uploaded {Count} questions from file: {FileName}", questions.Count, file.FileName);

            return Ok(new ApiResponse<object> 
            { 
                Success = true, 
                Message = $"Successfully uploaded {questions.Count} questions from file",
                Data = new { questionCount = questions.Count }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading questions from file: {FileName}", file?.FileName);
            return BadRequest(new ApiResponse<object> 
            { 
                Success = false, 
                Message = $"Error processing file: {ex.Message}" 
            });
        }
    }

    // Teacher: Get My Exams
    [HttpGet("teacher/my-exams")]
    [Authorize(Roles = "teacher,admin")]
    public async Task<ActionResult<ApiResponse<List<Exam>>>> GetMyExams()
    {
        var exams = await _examRepository.GetByTeacherIdAsync(GetUserId());
        return Ok(new ApiResponse<List<Exam>> { Success = true, Data = exams });
    }

    // Teacher: Assign Exam to Students
    [HttpPost("{examId}/assign")]
    [Authorize(Roles = "teacher,admin")]
    public async Task<ActionResult<ApiResponse<object>>> AssignExam(string examId, [FromBody] List<string> studentIds)
    {
        var exam = await _examRepository.GetByIdAsync(examId);
        if (exam == null)
            return NotFound(new ApiResponse<object> { Success = false, Message = "Exam not found" });

        if (exam.TeacherId != GetUserId() && GetUserRole() != "admin")
            return Forbid();

        exam.AssignedStudents.AddRange(studentIds);
        await _examRepository.UpdateAsync(exam);
        return Ok(new ApiResponse<object> { Success = true, Message = "Exam assigned successfully" });
    }

    // Student: Get My Exams
    [HttpGet("student/my-exams")]
    [Authorize(Roles = "student")]
    public async Task<ActionResult<ApiResponse<List<Exam>>>> GetStudentExams()
    {
        var exams = await _examRepository.GetByStudentIdAsync(GetUserId());
        var subscriptionType = GetSubscriptionType();

        // Filter based on subscription
        var filteredExams = exams.Where(e => !e.IsPaid || subscriptionType == "paid").ToList();
        return Ok(new ApiResponse<List<Exam>> { Success = true, Data = filteredExams });
    }

    // Student: Start Exam
    [HttpPost("{examId}/start")]
    [Authorize(Roles = "student")]
    public async Task<ActionResult<ApiResponse<Response>>> StartExam(string examId)
    {
        var exam = await _examRepository.GetByIdAsync(examId);
        if (exam == null)
            return NotFound(new ApiResponse<Response> { Success = false, Message = "Exam not found" });

        if (!exam.AssignedStudents.Contains(GetUserId()))
            return Forbid();

        if (exam.IsPaid && GetSubscriptionType() != "paid")
            return BadRequest(new ApiResponse<Response> { Success = false, Message = "Subscription required" });

        var existingResponse = await _responseRepository.GetByExamAndStudentAsync(examId, GetUserId());
        if (existingResponse != null)
            return BadRequest(new ApiResponse<Response> { Success = false, Message = "Exam already attempted" });

        var response = new Response
        {
            ExamId = examId,
            StudentId = GetUserId()
        };

        await _responseRepository.CreateAsync(response);
        return Ok(new ApiResponse<Response> { Success = true, Message = "Exam started", Data = response });
    }

    /// <summary>
    /// Validate exam session and heartbeat (Student only)
    /// </summary>
    [HttpPost("{examId}/validate-session")]
    [Authorize(Roles = "student")]
    public async Task<ActionResult<ApiResponse<object>>> ValidateSession(string examId, [FromBody] SessionValidationRequest? request)
    {
        try
        {
            var response = await _responseRepository.GetByExamAndStudentAsync(examId, GetUserId());
            if (response == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Exam not started" });

            // Check if exam is still valid
            var exam = await _examRepository.GetByIdAsync(examId);
            if (exam == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Exam not found" });

            // Validate time constraints
            if (DateTime.UtcNow < exam.StartTime)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Exam has not started yet" });

            if (DateTime.UtcNow > exam.EndTime)
            {
                response.SubmittedAt = DateTime.UtcNow;
                response.Status = "submitted";
                await _responseRepository.UpdateAsync(response);
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Exam time has expired" });
            }

            // Check violation count
            if (response.ViolationCount >= exam.MaxViolations)
            {
                if (response.Status != "submitted")
                {
                    response.SubmittedAt = DateTime.UtcNow;
                    response.Status = "submitted";
                    response.IsAutoSubmitted = true;
                    await _responseRepository.UpdateAsync(response);
                }
                return BadRequest(new ApiResponse<object> 
                { 
                    Success = false, 
                    Message = "Maximum violations reached. Exam auto-submitted.",
                    Data = new { autoSubmitted = true }
                });
            }

            return Ok(new ApiResponse<object> 
            { 
                Success = true, 
                Message = "Session valid",
                Data = new 
                { 
                    violationCount = response.ViolationCount,
                    maxViolations = exam.MaxViolations,
                    timeRemaining = (int)(exam.EndTime - DateTime.UtcNow).TotalSeconds
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating session: ExamId={ExamId}, StudentId={StudentId}", examId, GetUserId());
            return BadRequest(new ApiResponse<object> { Success = false, Message = "Error validating session" });
        }
    }

    // Student: Get Questions (Randomized)
    [HttpGet("{examId}/questions")]
    [Authorize(Roles = "student")]
    public async Task<ActionResult<ApiResponse<object>>> GetQuestions(string examId)
    {
        var questions = await _questionRepository.GetByExamIdAsync(examId);
        
        // Randomize questions and options with mapping
        var (randomizedQuestions, optionMappings) = QuestionRandomizer.RandomizeQuestionsWithMapping(questions);
        
        // Update correct answers after option randomization using mappings
        foreach (var randomizedQuestion in randomizedQuestions)
        {
            if (randomizedQuestion.Id != null && 
                optionMappings.TryGetValue(randomizedQuestion.Id, out var mapping) &&
                randomizedQuestion.Options.Count > 0)
            {
                var originalQuestion = questions.FirstOrDefault(q => q.Id == randomizedQuestion.Id);
                if (originalQuestion != null)
                {
                    randomizedQuestion.CorrectAnswer = QuestionRandomizer.UpdateCorrectAnswerAfterRandomization(
                        originalQuestion.CorrectAnswer,
                        originalQuestion.Options,
                        randomizedQuestion.Options,
                        mapping
                    );
                }
            }
        }

        // Return questions without correct answers for students
        var questionsWithoutAnswers = randomizedQuestions.Select(q => new
        {
            q.Id,
            q.QuestionText,
            q.QuestionType,
            q.Options,
            q.Marks,
            q.Order
        }).ToList();

        return Ok(new ApiResponse<object> { Success = true, Data = questionsWithoutAnswers });
    }

    // Student: Submit Answer (Autosave)
    [HttpPost("{examId}/answer")]
    [Authorize(Roles = "student")]
    public async Task<ActionResult<ApiResponse<object>>> SubmitAnswer(string examId, [FromBody] SubmitAnswerRequest request)
    {
        var response = await _responseRepository.GetByExamAndStudentAsync(examId, GetUserId());
        if (response == null)
            return NotFound(new ApiResponse<object> { Success = false, Message = "Exam not started" });

        var existingAnswer = response.Answers.FirstOrDefault(a => a.QuestionId == request.QuestionId);
        if (existingAnswer != null)
        {
            existingAnswer.AnswerText = request.Answer;
        }
        else
        {
            response.Answers.Add(new Answer
            {
                QuestionId = request.QuestionId,
                AnswerText = request.Answer
            });
        }

        await _responseRepository.UpdateAsync(response);
        return Ok(new ApiResponse<object> { Success = true, Message = "Answer saved" });
    }

    // Student: Submit Exam
    [HttpPost("{examId}/submit")]
    [Authorize(Roles = "student")]
    public async Task<ActionResult<ApiResponse<object>>> SubmitExam(string examId)
    {
        var response = await _responseRepository.GetByExamAndStudentAsync(examId, GetUserId());
        if (response == null)
            return NotFound(new ApiResponse<object> { Success = false, Message = "Exam not started" });

        response.SubmittedAt = DateTime.UtcNow;
        response.Status = "submitted";

        // Auto-evaluate MCQs
        var questions = await _questionRepository.GetByExamIdAsync(examId);
        foreach (var answer in response.Answers)
        {
            var question = questions.FirstOrDefault(q => q.Id == answer.QuestionId);
            if (question != null && question.QuestionType == "mcq")
            {
                answer.IsCorrect = answer.AnswerText.Trim().Equals(question.CorrectAnswer.Trim(), StringComparison.OrdinalIgnoreCase);
                answer.MarksObtained = answer.IsCorrect == true ? question.Marks : 0;
            }
        }

        await _responseRepository.UpdateAsync(response);
        return Ok(new ApiResponse<object> { Success = true, Message = "Exam submitted successfully" });
    }

    /// <summary>
    /// Report a violation during exam (Student only)
    /// </summary>
    /// <param name="examId">Exam ID</param>
    /// <param name="request">Violation details</param>
    /// <returns>Violation count and auto-submit status</returns>
    [HttpPost("{examId}/violation")]
    [Authorize(Roles = "student")]
    public async Task<ActionResult<ApiResponse<object>>> ReportViolation(string examId, [FromBody] ViolationRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object> 
                { 
                    Success = false, 
                    Message = "Invalid input data"
                });
            }

            _logger.LogWarning("Violation reported: ExamId={ExamId}, StudentId={StudentId}, Type={ViolationType}", 
                examId, GetUserId(), request.ViolationType);

        var response = await _responseRepository.GetByExamAndStudentAsync(examId, GetUserId());
        if (response == null)
            {
                _logger.LogWarning("Violation report failed: Exam not started - ExamId={ExamId}, StudentId={StudentId}", 
                    examId, GetUserId());
            return NotFound(new ApiResponse<object> { Success = false, Message = "Exam not started" });
            }

        var violation = new Violation
        {
            ExamId = examId,
            StudentId = GetUserId(),
            ViolationType = request.ViolationType
        };

        await _violationRepository.CreateAsync(violation);

        response.ViolationCount++;
        var exam = await _examRepository.GetByIdAsync(examId);
        if (exam != null && response.ViolationCount >= exam.MaxViolations)
        {
            response.SubmittedAt = DateTime.UtcNow;
            response.Status = "submitted";
            response.IsAutoSubmitted = true;
                
                _logger.LogWarning("Exam auto-submitted due to max violations: ExamId={ExamId}, StudentId={StudentId}, Violations={Count}", 
                    examId, GetUserId(), response.ViolationCount);
        }

        await _responseRepository.UpdateAsync(response);
            
            return Ok(new ApiResponse<object> 
            { 
                Success = true, 
                Message = "Violation recorded", 
                Data = new { violationCount = response.ViolationCount, autoSubmitted = response.IsAutoSubmitted } 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reporting violation: ExamId={ExamId}, StudentId={StudentId}", 
                examId, GetUserId());
            throw;
        }
    }

    // Admin/Teacher: Get All Exams
    [HttpGet]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse<List<Exam>>>> GetAllExams()
    {
        var exams = await _examRepository.GetAllAsync();
        return Ok(new ApiResponse<List<Exam>> { Success = true, Data = exams });
    }
}
