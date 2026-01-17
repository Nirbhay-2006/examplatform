using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ExamPlatform.API.Helpers;
using ExamPlatform.API.Models;

namespace ExamPlatform.API.Repositories;

public class ResultRepository : IResultRepository
{
    private readonly IMongoCollection<Result> _results;

    public ResultRepository(IMongoDatabase database)
    {
        _results = database.GetCollection<Result>("results");
    }

    public ResultRepository(IOptions<MongoDbSettings> settings) : this(GetDatabase(settings))
    {
    }

    private static IMongoDatabase GetDatabase(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        return client.GetDatabase(settings.Value.DatabaseName);
    }

    public async Task CreateAsync(Result result)
    {
        await _results.InsertOneAsync(result);
    }

    public async Task<Result?> GetByExamAndStudentAsync(string examId, string studentId)
    {
        return await _results.Find(r => r.ExamId == examId && r.StudentId == studentId).FirstOrDefaultAsync();
    }

    public async Task<List<Result>> GetByStudentIdAsync(string studentId)
    {
        return await _results.Find(r => r.StudentId == studentId).ToListAsync();
    }

    public async Task<List<Result>> GetByExamIdAsync(string examId)
    {
        return await _results.Find(r => r.ExamId == examId).ToListAsync();
    }
}
