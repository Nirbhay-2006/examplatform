# Backend Project File Configuration

## Project Files

The backend directory contains two project files:
- `ExamPlatformApi.csproj` - **Active project file (use this one)**
- `ExamPlatform.API.csproj` - Legacy/duplicate project file

## Which Project File to Use?

**Always use `ExamPlatformApi.csproj`** - This is the active project file that contains:
- All latest packages (itext7, EPPlus for file parsing)
- Correct configuration
- All recent updates

## Running the Backend

### Explicitly Specify Project File

Always specify the project file when running commands:

```bash
# Restore packages
dotnet restore ExamPlatformApi.csproj

# Build
dotnet build ExamPlatformApi.csproj

# Run
dotnet run --project ExamPlatformApi.csproj
```

### Using Startup Scripts

The startup scripts (`start-backend.bat`, `start-all.bat`) are already configured to use `ExamPlatformApi.csproj`.

## Why Two Project Files?

The `ExamPlatform.API.csproj` appears to be a legacy or duplicate file. The active project is `ExamPlatformApi.csproj` which has:
- Latest package references
- All recent features
- Correct namespace configuration

## Recommendation

You can safely ignore or delete `ExamPlatform.API.csproj` if it's not needed, but the scripts are configured to use the correct one (`ExamPlatformApi.csproj`).

