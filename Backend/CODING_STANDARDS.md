# Coding Standards & Best Practices - Exam Platform API

## Overview
This document outlines the coding standards, best practices, and architectural guidelines for the Exam Platform API (.NET 8.0).

## Table of Contents
1. [Project Structure](#project-structure)
2. [Naming Conventions](#naming-conventions)
3. [Code Organization](#code-organization)
4. [Error Handling](#error-handling)
5. [Logging](#logging)
6. [Security](#security)
7. [Validation](#validation)
8. [Documentation](#documentation)
9. [Performance](#performance)

---

## 1. Project Structure

### Folder Organization
```
ExamPlatformApi/
├── Controllers/          # API endpoints
├── Services/            # Business logic
├── Repositories/        # Data access layer
├── Models/              # Domain models
├── DTOs/                # Data Transfer Objects
├── Helpers/             # Utility classes
├── Middleware/          # Custom middleware
└── Properties/          # Configuration files
```

### Namespace Convention
- Use `ExamPlatform.API.{FolderName}` format
- Example: `ExamPlatform.API.Controllers`, `ExamPlatform.API.Services`

---

## 2. Naming Conventions

### Classes and Interfaces
- **Classes**: PascalCase (e.g., `UserRepository`, `EmailService`)
- **Interfaces**: PascalCase with `I` prefix (e.g., `IUserRepository`, `IEmailService`)
- **DTOs**: PascalCase with descriptive suffix (e.g., `RegisterRequest`, `LoginResponse`)

### Methods and Properties
- **Public methods**: PascalCase (e.g., `GetUserByIdAsync`, `CreateExamAsync`)
- **Private methods**: PascalCase (e.g., `ValidateUser`, `GenerateToken`)
- **Properties**: PascalCase (e.g., `UserId`, `Email`, `IsVerified`)

### Variables and Parameters
- **Local variables**: camelCase (e.g., `var userId = ...`, `var examList = ...`)
- **Parameters**: camelCase (e.g., `string userId`, `CreateExamRequest request`)
- **Private fields**: camelCase with `_` prefix (e.g., `_userRepository`, `_logger`)

### Constants
- **Constants**: PascalCase (e.g., `MaxViolations`, `DefaultDuration`)

---

## 3. Code Organization

### Controllers
- Keep controllers thin - delegate business logic to services
- Use dependency injection for all dependencies
- Add XML documentation comments for all public methods
- Use `[ApiController]` and `[Route]` attributes
- Implement proper authorization with `[Authorize]` attributes

**Example:**
```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExamController : ControllerBase
{
    private readonly IExamRepository _examRepository;
    private readonly ILogger<ExamController> _logger;

    public ExamController(
        IExamRepository examRepository,
        ILogger<ExamController> logger)
    {
        _examRepository = examRepository;
        _logger = logger;
    }

    /// <summary>
    /// Get exam by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<Exam>>> GetExam(string id)
    {
        // Implementation
    }
}
```

### Services
- Contain business logic
- Implement interfaces
- Use dependency injection
- Handle exceptions appropriately

### Repositories
- Handle all database operations
- Implement interfaces
- Use async/await for all I/O operations
- Return null for not found, not throw exceptions

---

## 4. Error Handling

### Exception Handling Strategy
1. **Global Exception Middleware**: Catch all unhandled exceptions
2. **Try-Catch in Controllers**: For logging and context
3. **Validation**: Use ModelState validation
4. **Custom Exceptions**: For domain-specific errors

### Response Format
All API responses should follow the `ApiResponse<T>` pattern:
```csharp
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
}
```

### HTTP Status Codes
- `200 OK`: Successful GET, PUT, PATCH
- `201 Created`: Successful POST
- `400 Bad Request`: Validation errors, invalid input
- `401 Unauthorized`: Missing or invalid authentication
- `403 Forbidden`: Valid authentication but insufficient permissions
- `404 Not Found`: Resource not found
- `500 Internal Server Error`: Server errors (handled by middleware)

---

## 5. Logging

### Logging Levels
- **Information**: Normal operations, successful actions
- **Warning**: Unusual situations, validation failures
- **Error**: Exceptions, failures
- **Debug**: Detailed diagnostic information (development only)

### Logging Best Practices
1. Always inject `ILogger<T>` in constructors
2. Include relevant context (userId, examId, etc.)
3. Use structured logging with parameters
4. Log at appropriate levels
5. Never log sensitive information (passwords, tokens)

**Example:**
```csharp
_logger.LogInformation("User registered successfully: {Email}, Role: {Role}", user.Email, user.Role);
_logger.LogWarning("Login failed: Invalid credentials for email: {Email}", request.Email);
_logger.LogError(ex, "Error during registration for email: {Email}", request.Email);
```

---

## 6. Security

### Authentication & Authorization
- Use JWT tokens for authentication
- Implement role-based authorization
- Validate tokens on every protected endpoint
- Use `[Authorize]` attributes appropriately

### Password Security
- Use BCrypt for password hashing
- Never store plain text passwords
- Enforce strong password policies (via validation)

### OTP Generation
- Use cryptographically secure random number generator
- Set appropriate expiry times (10 minutes)
- Clear OTP after successful verification

### Input Validation
- Validate all user inputs
- Use data annotations on DTOs
- Sanitize inputs to prevent injection attacks

### Sensitive Data
- Never commit secrets to version control
- Use User Secrets for development
- Use environment variables or Azure Key Vault for production
- Remove hardcoded credentials from appsettings.json

---

## 7. Validation

### Data Annotations
Use validation attributes on all DTOs:
- `[Required]`: Required fields
- `[EmailAddress]`: Email validation
- `[StringLength]`: String length constraints
- `[Range]`: Numeric range validation
- `[RegularExpression]`: Pattern validation

**Example:**
```csharp
public class RegisterRequest
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;
}
```

### ModelState Validation
Always check `ModelState.IsValid` in controllers:
```csharp
if (!ModelState.IsValid)
{
    return BadRequest(new ApiResponse<object> 
    { 
        Success = false, 
        Message = "Invalid input data",
        Data = ModelState
    });
}
```

---

## 8. Documentation

### XML Documentation Comments
Add XML comments to all public classes, methods, and properties:

```csharp
/// <summary>
/// Register a new user (student or teacher)
/// </summary>
/// <param name="request">Registration request containing user details</param>
/// <returns>Success response with verification message</returns>
[HttpPost("register")]
public async Task<ActionResult<ApiResponse<object>>> Register([FromBody] RegisterRequest request)
{
    // Implementation
}
```

### Swagger Documentation
- Configure Swagger in `Program.cs`
- Add descriptions to endpoints
- Document request/response models

---

## 9. Performance

### Async/Await
- Use `async Task` for all I/O operations
- Use `async Task<T>` for operations returning data
- Avoid `async void` (except event handlers)

### Database Operations
- Use async methods from MongoDB driver
- Implement proper indexing in MongoDB
- Use projections to limit data transfer

### Dependency Injection
- Use appropriate lifetimes:
  - `Singleton`: Stateless services, repositories
  - `Scoped`: Services with request-scoped state
  - `Transient`: Lightweight, stateless services

---

## 10. Additional Best Practices

### CORS Configuration
- Use restrictive CORS in production
- Allow specific origins only
- Use credentials only when necessary

### Environment-Specific Configuration
- Use `appsettings.Development.json` for development
- Use environment variables for production
- Never commit production secrets

### Code Formatting
- Use `.editorconfig` for consistent formatting
- Follow C# coding conventions
- Use file-scoped namespaces (C# 10+)

### Testing Considerations
- Write unit tests for services
- Write integration tests for controllers
- Mock external dependencies

---

## Checklist for New Code

- [ ] Follows naming conventions
- [ ] Has XML documentation comments
- [ ] Includes proper error handling
- [ ] Has logging statements
- [ ] Validates input data
- [ ] Uses async/await for I/O
- [ ] Implements proper authorization
- [ ] Follows response format (`ApiResponse<T>`)
- [ ] No hardcoded secrets
- [ ] Follows project structure

---

## References

- [Microsoft C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- [ASP.NET Core Best Practices](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/best-practices)
- [REST API Design Best Practices](https://restfulapi.net/)

---

**Last Updated**: 2024
**Maintained By**: Development Team

