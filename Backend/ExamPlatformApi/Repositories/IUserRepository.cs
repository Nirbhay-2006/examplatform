using ExamPlatform.API.Models;

namespace ExamPlatform.API.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(string id);
    Task CreateAsync(User user);
    Task UpdateAsync(User user);
    Task<List<User>> GetAllAsync();
}
