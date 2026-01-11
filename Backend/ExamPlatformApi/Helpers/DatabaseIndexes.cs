using MongoDB.Driver;
using ExamPlatform.API.Models;

namespace ExamPlatform.API.Helpers;

/// <summary>
/// Helper class to create database indexes for improved query performance
/// </summary>
public static class DatabaseIndexes
{
    /// <summary>
    /// Creates all necessary indexes for optimal query performance
    /// </summary>
    public static void CreateIndexes(IMongoDatabase database)
    {
        // Users collection indexes
        var usersCollection = database.GetCollection<User>("users");
        usersCollection.Indexes.CreateOne(
            new CreateIndexModel<User>(Builders<User>.IndexKeys.Ascending(u => u.Email),
            new CreateIndexOptions { Unique = true, Name = "email_unique" }));
        usersCollection.Indexes.CreateOne(
            new CreateIndexModel<User>(Builders<User>.IndexKeys.Ascending(u => u.Role),
            new CreateIndexOptions { Name = "role_index" }));

        // Exams collection indexes
        var examsCollection = database.GetCollection<Exam>("exams");
        examsCollection.Indexes.CreateOne(
            new CreateIndexModel<Exam>(Builders<Exam>.IndexKeys.Ascending(e => e.TeacherId),
            new CreateIndexOptions { Name = "teacherId_index" }));
        examsCollection.Indexes.CreateOne(
            new CreateIndexModel<Exam>(Builders<Exam>.IndexKeys.Ascending(e => e.AssignedStudents),
            new CreateIndexOptions { Name = "assignedStudents_index" }));
        examsCollection.Indexes.CreateOne(
            new CreateIndexModel<Exam>(Builders<Exam>.IndexKeys.Ascending(e => e.StartTime),
            new CreateIndexOptions { Name = "startTime_index" }));

        // Questions collection indexes
        var questionsCollection = database.GetCollection<Question>("questions");
        questionsCollection.Indexes.CreateOne(
            new CreateIndexModel<Question>(Builders<Question>.IndexKeys.Ascending(q => q.ExamId),
            new CreateIndexOptions { Name = "examId_index" }));
        questionsCollection.Indexes.CreateOne(
            new CreateIndexModel<Question>(
                Builders<Question>.IndexKeys.Combine(
                    Builders<Question>.IndexKeys.Ascending(q => q.ExamId),
                    Builders<Question>.IndexKeys.Ascending(q => q.Order)),
                new CreateIndexOptions { Name = "examId_order_index" }));

        // Responses collection indexes
        var responsesCollection = database.GetCollection<Response>("responses");
        responsesCollection.Indexes.CreateOne(
            new CreateIndexModel<Response>(
                Builders<Response>.IndexKeys.Combine(
                    Builders<Response>.IndexKeys.Ascending(r => r.ExamId),
                    Builders<Response>.IndexKeys.Ascending(r => r.StudentId)),
                new CreateIndexOptions { Unique = true, Name = "examId_studentId_unique" }));
        responsesCollection.Indexes.CreateOne(
            new CreateIndexModel<Response>(Builders<Response>.IndexKeys.Ascending(r => r.StudentId),
            new CreateIndexOptions { Name = "studentId_index" }));

        // Results collection indexes
        var resultsCollection = database.GetCollection<Result>("results");
        resultsCollection.Indexes.CreateOne(
            new CreateIndexModel<Result>(
                Builders<Result>.IndexKeys.Combine(
                    Builders<Result>.IndexKeys.Ascending(r => r.ExamId),
                    Builders<Result>.IndexKeys.Ascending(r => r.StudentId)),
                new CreateIndexOptions { Unique = true, Name = "result_examId_studentId_unique" }));
        resultsCollection.Indexes.CreateOne(
            new CreateIndexModel<Result>(Builders<Result>.IndexKeys.Ascending(r => r.StudentId),
            new CreateIndexOptions { Name = "result_studentId_index" }));

        // Violations collection indexes
        var violationsCollection = database.GetCollection<Violation>("violations");
        violationsCollection.Indexes.CreateOne(
            new CreateIndexModel<Violation>(
                Builders<Violation>.IndexKeys.Combine(
                    Builders<Violation>.IndexKeys.Ascending(v => v.ExamId),
                    Builders<Violation>.IndexKeys.Ascending(v => v.StudentId)),
                new CreateIndexOptions { Name = "violation_examId_studentId_index" }));

        // Payments collection indexes
        var paymentsCollection = database.GetCollection<Payment>("payments");
        paymentsCollection.Indexes.CreateOne(
            new CreateIndexModel<Payment>(Builders<Payment>.IndexKeys.Ascending(p => p.UserId),
            new CreateIndexOptions { Name = "userId_index" }));
        paymentsCollection.Indexes.CreateOne(
            new CreateIndexModel<Payment>(Builders<Payment>.IndexKeys.Ascending(p => p.OrderId),
            new CreateIndexOptions { Unique = true, Name = "orderId_unique" }));
    }
}

