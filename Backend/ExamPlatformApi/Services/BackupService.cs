using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.Bson;
using System.IO;

namespace ExamPlatform.API.Services;

public class BackupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BackupService> _logger;
    private readonly string _backupPath;

    public BackupService(IServiceProvider serviceProvider, ILogger<BackupService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _backupPath = Path.Combine(Directory.GetCurrentDirectory(), "Backups");
        if (!Directory.Exists(_backupPath))
        {
            Directory.CreateDirectory(_backupPath);
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Backup Service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Run backup every 24 hours
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
                await PerformBackupAsync();
            }
            catch (TaskCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during backup.");
            }
        }
    }

    private async Task PerformBackupAsync()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var database = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();
            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            var backupFile = Path.Combine(_backupPath, $"backup_{timestamp}.json");

            _logger.LogInformation("Starting database backup to {Path}", backupFile);

            var collections = await database.ListCollectionNamesAsync();
            var collectionNames = await collections.ToListAsync();

            using (var writer = new StreamWriter(backupFile))
            {
                await writer.WriteLineAsync("{");
                
                for (int i = 0; i < collectionNames.Count; i++)
                {
                    var name = collectionNames[i];
                    await writer.WriteAsync($"  \"{name}\": [");
                    
                    var collection = database.GetCollection<BsonDocument>(name);
                    var documents = await collection.Find(new BsonDocument()).ToListAsync();
                    
                    for (int j = 0; j < documents.Count; j++)
                    {
                        var json = documents[j].ToJson(new MongoDB.Bson.IO.JsonWriterSettings { OutputMode = MongoDB.Bson.IO.JsonOutputMode.Shell });
                        await writer.WriteAsync(json);
                        if (j < documents.Count - 1) await writer.WriteAsync(",");
                    }

                    await writer.WriteAsync("]");
                    if (i < collectionNames.Count - 1) await writer.WriteAsync(",");
                    
                    await writer.WriteLineAsync();
                }

                await writer.WriteLineAsync("}");
            }

            _logger.LogInformation("Database backup completed successfully.");
        }
    }
}
