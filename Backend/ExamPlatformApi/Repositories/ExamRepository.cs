using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ExamPlatform.API.Helpers;
using ExamPlatform.API.Models;

namespace ExamPlatform.API.Repositories;

public class ExamRepository : IExamRepository
{
    private readonly IMongoCollection<Exam> _exams;

    public ExamRepository(IMongoDatabase database)
    {
        _exams = database.GetCollection<Exam>("exams");
    }

    // Keep old constructor for backwards compatibility
    public ExamRepository(IOptions<MongoDbSettings> settings) : this(GetDatabase(settings))
    {
    }

    private static IMongoDatabase GetDatabase(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        return client.GetDatabase(settings.Value.DatabaseName);
    }

    public async Task<Exam?> GetByIdAsync(string id)
    {
        return await _exams.Find(e => e.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<Exam>> GetAllAsync()
    {
        return await _exams.Find(_ => true).ToListAsync();
    }

    public async Task<List<Exam>> GetByTeacherIdAsync(string teacherId)
    {
        return await _exams.Find(e => e.TeacherId == teacherId).ToListAsync();
    }

    public async Task<List<Exam>> GetByStudentIdAsync(string studentId)
    {
        return await _exams.Find(e => e.AssignedStudents.Contains(studentId)).ToListAsync();
    }

    public async Task CreateAsync(Exam exam)
    {
        await _exams.InsertOneAsync(exam);
    }

    public async Task UpdateAsync(Exam exam)
    {
        await _exams.ReplaceOneAsync(e => e.Id == exam.Id, exam);
    }

    public async Task DeleteAsync(string id)
    {
        await _exams.DeleteOneAsync(e => e.Id == id);
    }
}
