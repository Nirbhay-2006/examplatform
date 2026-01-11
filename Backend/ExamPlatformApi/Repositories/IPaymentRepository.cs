using ExamPlatform.API.Models;

namespace ExamPlatform.API.Repositories;

public interface IPaymentRepository
{
    Task CreateAsync(Payment payment);
    Task<Payment?> GetByOrderIdAsync(string orderId);
    Task UpdateAsync(Payment payment);
    Task<List<Payment>> GetByUserIdAsync(string userId);
}
