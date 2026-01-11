using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ExamPlatform.API.Helpers;
using ExamPlatform.API.Models;

namespace ExamPlatform.API.Repositories;

public class ResponseRepository : IResponseRepository
{
    private readonly IMongoCollection<Response> _responses;

    public ResponseRepository(IMongoDatabase database)
    {
        _responses = database.GetCollection<Response>("responses");
    }

    public ResponseRepository(IOptions<MongoDbSettings> settings) : this(GetDatabase(settings))
    {
    }

    private static IMongoDatabase GetDatabase(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        return client.GetDatabase(settings.Value.DatabaseName);
    }

    public async Task<Response?> GetByIdAsync(string id)
    {
        return await _responses.Find(r => r.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Response?> GetByExamAndStudentAsync(string examId, string studentId)
    {
        return await _responses.Find(r => r.ExamId == examId && r.StudentId == studentId).FirstOrDefaultAsync();
    }

    public async Task<List<Response>> GetByExamIdAsync(string examId)
    {
        return await _responses.Find(r => r.ExamId == examId).ToListAsync();
    }

    public async Task CreateAsync(Response response)
    {
        await _responses.InsertOneAsync(response);
    }

    public async Task UpdateAsync(Response response)
    {
        await _responses.ReplaceOneAsync(r => r.Id == response.Id, response);
    }
}
