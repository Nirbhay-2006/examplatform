using ExamPlatform.API.Models;

namespace ExamPlatform.API.Repositories;

public interface IViolationRepository
{
    Task CreateAsync(Violation violation);
    Task<List<Violation>> GetByExamAndStudentAsync(string examId, string studentId);
    Task<int> GetCountByExamAndStudentAsync(string examId, string studentId);
    Task<List<Violation>> GetByExamIdAsync(string examId);
}
