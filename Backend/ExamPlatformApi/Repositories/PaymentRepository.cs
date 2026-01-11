using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ExamPlatform.API.Helpers;
using ExamPlatform.API.Models;

namespace ExamPlatform.API.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly IMongoCollection<Payment> _payments;

    public PaymentRepository(IMongoDatabase database)
    {
        _payments = database.GetCollection<Payment>("payments");
    }

    public PaymentRepository(IOptions<MongoDbSettings> settings) : this(GetDatabase(settings))
    {
    }

    private static IMongoDatabase GetDatabase(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        return client.GetDatabase(settings.Value.DatabaseName);
    }

    public async Task CreateAsync(Payment payment)
    {
        await _payments.InsertOneAsync(payment);
    }

    public async Task<Payment?> GetByOrderIdAsync(string orderId)
    {
        return await _payments.Find(p => p.OrderId == orderId).FirstOrDefaultAsync();
    }

    public async Task UpdateAsync(Payment payment)
    {
        await _payments.ReplaceOneAsync(p => p.Id == payment.Id, payment);
    }

    public async Task<List<Payment>> GetByUserIdAsync(string userId)
    {
        return await _payments.Find(p => p.UserId == userId).ToListAsync();
    }
}
