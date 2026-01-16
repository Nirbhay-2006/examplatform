using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;
using ExamPlatform.API.Helpers;
using ExamPlatform.API.Repositories;
using ExamPlatform.API.Services;
using ExamPlatform.API.Middleware;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// Register repositories (using singleton database connection for connection pooling)
builder.Services.AddSingleton<IUserRepository>(sp => 
    new UserRepository(sp.GetRequiredService<MongoDB.Driver.IMongoDatabase>()));
builder.Services.AddSingleton<IExamRepository>(sp => 
    new ExamRepository(sp.GetRequiredService<MongoDB.Driver.IMongoDatabase>()));
builder.Services.AddSingleton<IQuestionRepository>(sp => 
    new QuestionRepository(sp.GetRequiredService<MongoDB.Driver.IMongoDatabase>()));
builder.Services.AddSingleton<IResponseRepository>(sp => 
    new ResponseRepository(sp.GetRequiredService<MongoDB.Driver.IMongoDatabase>()));
builder.Services.AddSingleton<IViolationRepository>(sp => 
    new ViolationRepository(sp.GetRequiredService<MongoDB.Driver.IMongoDatabase>()));
builder.Services.AddSingleton<IPaymentRepository>(sp => 
    new PaymentRepository(sp.GetRequiredService<MongoDB.Driver.IMongoDatabase>()));
builder.Services.AddSingleton<IResultRepository>(sp => 
    new ResultRepository(sp.GetRequiredService<MongoDB.Driver.IMongoDatabase>()));

// Register services
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IFileParserService, FileParserService>();
builder.Services.AddSingleton<ISystemConfigRepository>(sp => 
    new SystemConfigRepository(sp.GetRequiredService<MongoDB.Driver.IMongoDatabase>()));
builder.Services.AddSingleton<IAuditService>(sp => 
    new AuditService(sp.GetRequiredService<MongoDB.Driver.IMongoDatabase>(), sp.GetRequiredService<ILogger<AuditService>>()));
builder.Services.AddScoped<IAntiCheatService, AntiCheatService>();
builder.Services.AddMemoryCache(); // For Rate Limiting
builder.Services.AddHostedService<BackupService>(); // Periodic Backups

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings!.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
    };
});

builder.Services.AddAuthorization();

// CORS - Configure based on environment
builder.Services.AddCors(options =>
{
    if (builder.Environment.IsDevelopment())
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
    }
    else
    {
        // Production CORS - restrict to specific origins
        options.AddPolicy("AllowSpecificOrigins", policy =>
        {
            var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() 
                ?? new[] { "https://localhost:4200" };
            
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
    }
});

// Response compression for faster API responses
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.BrotliCompressionProvider>();
    options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.GzipCompressionProvider>();
});

builder.Services.Configure<Microsoft.AspNetCore.ResponseCompression.BrotliCompressionProviderOptions>(options =>
{
    options.Level = System.IO.Compression.CompressionLevel.Optimal;
});

builder.Services.Configure<Microsoft.AspNetCore.ResponseCompression.GzipCompressionProviderOptions>(options =>
{
    options.Level = System.IO.Compression.CompressionLevel.Optimal;
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Exam Platform API",
        Version = "v1",
        Description = "Online Examination Platform API with JWT Authentication, OTP Verification, and Anti-Cheat Monitoring"
    });
});

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// MongoDB connection singleton for connection pooling
builder.Services.AddSingleton<MongoDB.Driver.IMongoClient>(serviceProvider =>
{
    var settings = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<MongoDbSettings>>().Value;
    return new MongoDB.Driver.MongoClient(settings.ConnectionString);
});

builder.Services.AddSingleton<MongoDB.Driver.IMongoDatabase>(serviceProvider =>
{
    var client = serviceProvider.GetRequiredService<MongoDB.Driver.IMongoClient>();
    var settings = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<MongoDbSettings>>().Value;
    return client.GetDatabase(settings.DatabaseName);
});

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseMiddleware<SecurityHeadersMiddleware>(); // Security Headers first
app.UseMiddleware<RateLimitingMiddleware>(); // Rate Limiting next
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Exam Platform API v1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
    });
}

// Response compression (should be early in pipeline)
app.UseResponseCompression();

// Global exception handling middleware (must be first)
app.UseMiddleware<ExceptionHandlingMiddleware>();

// CORS
if (app.Environment.IsDevelopment())
{
app.UseCors("AllowAll");
}
else
{
    app.UseCors("AllowSpecificOrigins");
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Create database indexes on startup for optimal performance
try
{
    var mongoDatabase = app.Services.GetRequiredService<MongoDB.Driver.IMongoDatabase>();
    ExamPlatform.API.Helpers.DatabaseIndexes.CreateIndexes(mongoDatabase);
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Database indexes created successfully for optimal performance");
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogWarning(ex, "Failed to create database indexes: {Message}", ex.Message);
}

app.Run();