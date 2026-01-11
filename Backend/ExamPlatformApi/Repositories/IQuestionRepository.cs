using ExamPlatform.API.Models;

namespace ExamPlatform.API.Repositories;

public interface IQuestionRepository
{
    Task<Question?> GetByIdAsync(string id);
    Task<List<Question>> GetByExamIdAsync(string examId);
    Task CreateAsync(Question question);
    Task UpdateAsync(Question question);
    Task DeleteAsync(string id);
}
