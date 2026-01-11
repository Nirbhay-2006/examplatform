using ExamPlatform.API.Models;

namespace ExamPlatform.API.Repositories;

public interface IResultRepository
{
    Task CreateAsync(Result result);
    Task<Result?> GetByExamAndStudentAsync(string examId, string studentId);
    Task<List<Result>> GetByStudentIdAsync(string studentId);
}
