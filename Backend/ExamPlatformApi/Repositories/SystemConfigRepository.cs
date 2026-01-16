using MongoDB.Driver;
using ExamPlatform.API.Models;
using Microsoft.Extensions.Options;

namespace ExamPlatform.API.Repositories;

public interface ISystemConfigRepository
{
    Task<SystemConfig> GetConfigAsync();
    Task UpdateConfigAsync(SystemConfig config);
}

public class SystemConfigRepository : ISystemConfigRepository
{
    private readonly IMongoCollection<SystemConfig> _systemConfig;

    public SystemConfigRepository(IMongoDatabase database)
    {
        _systemConfig = database.GetCollection<SystemConfig>("system_config");
    }

    public async Task<SystemConfig> GetConfigAsync()
    {
        var config = await _systemConfig.Find(c => c.Key == "default").FirstOrDefaultAsync();
        if (config == null)
        {
            config = new SystemConfig();
            await _systemConfig.InsertOneAsync(config);
        }
        return config;
    }

    public async Task UpdateConfigAsync(SystemConfig config)
    {
        await _systemConfig.ReplaceOneAsync(c => c.Key == "default", config, new ReplaceOptions { IsUpsert = true });
    }
}
