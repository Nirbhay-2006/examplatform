using ExamPlatform.API.Models;

namespace ExamPlatform.API.Repositories;

public interface IExamRepository
{
    Task<Exam?> GetByIdAsync(string id);
    Task<List<Exam>> GetAllAsync();
    Task<List<Exam>> GetByTeacherIdAsync(string teacherId);
    Task<List<Exam>> GetByStudentIdAsync(string studentId);
    Task CreateAsync(Exam exam);
    Task UpdateAsync(Exam exam);
    Task DeleteAsync(string id);
}
