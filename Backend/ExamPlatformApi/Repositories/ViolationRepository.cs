using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ExamPlatform.API.Helpers;
using ExamPlatform.API.Models;

namespace ExamPlatform.API.Repositories;

public class ViolationRepository : IViolationRepository
{
    private readonly IMongoCollection<Violation> _violations;

    public ViolationRepository(IMongoDatabase database)
    {
        _violations = database.GetCollection<Violation>("violations");
    }

    public ViolationRepository(IOptions<MongoDbSettings> settings) : this(GetDatabase(settings))
    {
    }

    private static IMongoDatabase GetDatabase(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        return client.GetDatabase(settings.Value.DatabaseName);
    }

    public async Task CreateAsync(Violation violation)
    {
        await _violations.InsertOneAsync(violation);
    }

    public async Task<List<Violation>> GetByExamAndStudentAsync(string examId, string studentId)
    {
        return await _violations.Find(v => v.ExamId == examId && v.StudentId == studentId).ToListAsync();
    }

    public async Task<int> GetCountByExamAndStudentAsync(string examId, string studentId)
    {
        return (int)await _violations.CountDocumentsAsync(v => v.ExamId == examId && v.StudentId == studentId);
    }

    public async Task<List<Violation>> GetByExamIdAsync(string examId)
    {
        return await _violations.Find(v => v.ExamId == examId).ToListAsync();
    }
}
