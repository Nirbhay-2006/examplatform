using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ExamPlatform.API.Helpers;
using ExamPlatform.API.Models;

namespace ExamPlatform.API.Repositories;

public class QuestionRepository : IQuestionRepository
{
    private readonly IMongoCollection<Question> _questions;

    public QuestionRepository(IMongoDatabase database)
    {
        _questions = database.GetCollection<Question>("questions");
    }

    public QuestionRepository(IOptions<MongoDbSettings> settings) : this(GetDatabase(settings))
    {
    }

    private static IMongoDatabase GetDatabase(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        return client.GetDatabase(settings.Value.DatabaseName);
    }

    public async Task<Question?> GetByIdAsync(string id)
    {
        return await _questions.Find(q => q.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<Question>> GetByExamIdAsync(string examId)
    {
        return await _questions.Find(q => q.ExamId == examId).SortBy(q => q.Order).ToListAsync();
    }

    public async Task CreateAsync(Question question)
    {
        await _questions.InsertOneAsync(question);
    }

    public async Task UpdateAsync(Question question)
    {
        await _questions.ReplaceOneAsync(q => q.Id == question.Id, question);
    }

    public async Task DeleteAsync(string id)
    {
        await _questions.DeleteOneAsync(q => q.Id == id);
    }
}
