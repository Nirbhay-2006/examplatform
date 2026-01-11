using ExamPlatform.API.Models;

namespace ExamPlatform.API.Services;

public interface IFileParserService
{
    Task<List<Question>> ParseQuestionsFromFileAsync(IFormFile file, string examId);
}

