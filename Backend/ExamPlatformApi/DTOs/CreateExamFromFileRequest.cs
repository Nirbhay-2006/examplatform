using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ExamPlatform.API.DTOs;

public class CreateExamFromFileRequest
{
    [Required]
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [Required]
    public IFormFile File { get; set; } = null!;

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    public int Duration { get; set; } // In minutes

    public int PassingMarks { get; set; }

    public bool IsPaid { get; set; } = false;

    public int MaxViolations { get; set; } = 3;

    public int? QuestionCount { get; set; } // Optional: Select only N questions randomly
}
