using ExamPlatform.API.Models;

namespace ExamPlatform.API.Repositories;

public interface IResponseRepository
{
    Task<Response?> GetByIdAsync(string id);
    Task<Response?> GetByExamAndStudentAsync(string examId, string studentId);
    Task<List<Response>> GetByExamIdAsync(string examId);
    Task CreateAsync(Response response);
    Task UpdateAsync(Response response);
}
