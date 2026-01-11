using System.ComponentModel.DataAnnotations;

namespace ExamPlatform.API.DTOs;

/// <summary>
/// Request model for creating an exam
/// </summary>
public class CreateExamRequest
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 200 characters")]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Description must not exceed 1000 characters")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Duration is required")]
    [Range(1, 600, ErrorMessage = "Duration must be between 1 and 600 minutes")]
    public int Duration { get; set; }

    [Required(ErrorMessage = "Total marks is required")]
    [Range(1, 1000, ErrorMessage = "Total marks must be between 1 and 1000")]
    public int TotalMarks { get; set; }

    [Required(ErrorMessage = "Passing marks is required")]
    [Range(0, 1000, ErrorMessage = "Passing marks must be between 0 and 1000")]
    public int PassingMarks { get; set; }

    [Required(ErrorMessage = "Start time is required")]
    public DateTime StartTime { get; set; }

    [Required(ErrorMessage = "End time is required")]
    public DateTime EndTime { get; set; }

    public bool IsPaid { get; set; }

    [Range(1, 10, ErrorMessage = "Max violations must be between 1 and 10")]
    public int MaxViolations { get; set; } = 3;
}

/// <summary>
/// Request model for creating a question
/// </summary>
public class CreateQuestionRequest
{
    [Required(ErrorMessage = "Question text is required")]
    [StringLength(2000, MinimumLength = 5, ErrorMessage = "Question text must be between 5 and 2000 characters")]
    public string QuestionText { get; set; } = string.Empty;

    [Required(ErrorMessage = "Question type is required")]
    [RegularExpression("^(mcq|text)$", ErrorMessage = "Question type must be either 'mcq' or 'text'")]
    public string QuestionType { get; set; } = "mcq";

    public List<string> Options { get; set; } = new();

    [StringLength(500, ErrorMessage = "Correct answer must not exceed 500 characters")]
    public string CorrectAnswer { get; set; } = string.Empty;

    [Required(ErrorMessage = "Marks is required")]
    [Range(1, 100, ErrorMessage = "Marks must be between 1 and 100")]
    public int Marks { get; set; }
}

/// <summary>
/// Request model for submitting an answer
/// </summary>
public class SubmitAnswerRequest
{
    [Required(ErrorMessage = "Question ID is required")]
    public string QuestionId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Answer is required")]
    [StringLength(2000, ErrorMessage = "Answer must not exceed 2000 characters")]
    public string Answer { get; set; } = string.Empty;
}

/// <summary>
/// Request model for reporting a violation
/// </summary>
public class ViolationRequest
{
    [Required(ErrorMessage = "Exam ID is required")]
    public string ExamId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Violation type is required")]
    [StringLength(50, ErrorMessage = "Violation type must not exceed 50 characters")]
    public string ViolationType { get; set; } = string.Empty;
}

/// <summary>
/// Request model for verifying payment
/// </summary>
public class VerifyPaymentRequest
{
    [Required(ErrorMessage = "Order ID is required")]
    public string OrderId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Payment ID is required")]
    public string PaymentId { get; set; } = string.Empty;
}

/// <summary>
/// Request model for session validation
/// </summary>
public class SessionValidationRequest
{
    public string? BrowserFingerprint { get; set; }
    public string? UserAgent { get; set; }
    public DateTime? Timestamp { get; set; }
}
