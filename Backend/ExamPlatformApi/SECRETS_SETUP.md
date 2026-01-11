# User Secrets Setup Guide

## Overview
This project uses .NET User Secrets to store sensitive configuration data (passwords, API keys) during development. This prevents sensitive information from being committed to version control.

## ✅ Already Configured

The following secrets have been set up:
- **EmailSettings:Password** - Gmail app password for sending OTP emails
- **JwtSettings:Secret** - JWT token signing key

## How User Secrets Work

User Secrets are stored in:
- **Windows**: `%APPDATA%\Microsoft\UserSecrets\<UserSecretsId>\secrets.json`
- **Linux/Mac**: `~/.microsoft/usersecrets/<UserSecretsId>/secrets.json`

The `UserSecretsId` for this project is: `f67ff6e8-326b-4514-81dc-88a299d6ed5e`

## Viewing Secrets

To view all configured secrets:
```bash
dotnet user-secrets list --project ExamPlatformApi.csproj
```

## Adding New Secrets

To add a new secret:
```bash
dotnet user-secrets set "Section:Key" "value" --project ExamPlatformApi.csproj
```

Example:
```bash
dotnet user-secrets set "PaymentSettings:RazorpayKeySecret" "your_secret" --project ExamPlatformApi.csproj
```

## Removing Secrets

To remove a secret:
```bash
dotnet user-secrets remove "Section:Key" --project ExamPlatformApi.csproj
```

## Clearing All Secrets

To clear all secrets:
```bash
dotnet user-secrets clear --project ExamPlatformApi.csproj
```

## Current Configuration

### Email Settings
- **SMTP Server**: smtp.gmail.com
- **Port**: 587
- **Sender Email**: pansheriyanirbhay@gmail.com
- **Password**: ✅ Stored in User Secrets

### JWT Settings
- **Secret**: ✅ Stored in User Secrets
- **Issuer**: ExamPlatformAPI
- **Audience**: ExamPlatformClient
- **Expiry**: 1440 minutes (24 hours)

## Production Deployment

⚠️ **Important**: User Secrets are for **development only**.

For production, use:
- **Environment Variables**
- **Azure Key Vault**
- **AWS Secrets Manager**
- **Other secure secret management services**

### Setting Environment Variables

**Windows (PowerShell)**:
```powershell
$env:EmailSettings__Password = "your-password"
$env:JwtSettings__Secret = "your-secret"
```

**Linux/Mac**:
```bash
export EmailSettings__Password="your-password"
export JwtSettings__Secret="your-secret"
```

**Docker**:
```yaml
environment:
  - EmailSettings__Password=your-password
  - JwtSettings__Secret=your-secret
```

## Security Best Practices

1. ✅ **Never commit secrets to Git**
   - User Secrets are automatically ignored
   - `secrets.json` is in `.gitignore`

2. ✅ **Use strong JWT secrets**
   - Minimum 32 characters
   - Mix of letters, numbers, and symbols
   - Generate using: `openssl rand -base64 32`

3. ✅ **Rotate secrets regularly**
   - Change passwords every 90 days
   - Update JWT secret when compromised

4. ✅ **Use App Passwords for Gmail**
   - Enable 2FA on Gmail account
   - Generate app-specific password
   - Don't use your main Gmail password

## Troubleshooting

### Secrets Not Loading
- Ensure you're running in **Development** environment
- Check that `UserSecretsId` is set in `.csproj` file
- Verify secrets exist: `dotnet user-secrets list`

### Email Not Sending
- Verify Gmail app password is correct
- Check Gmail "Less secure app access" settings
- Ensure SMTP settings are correct in `appsettings.json`

### JWT Token Issues
- Verify JWT secret is set correctly
- Check token expiry settings
- Ensure secret matches between environments

## Need Help?

If you encounter issues:
1. Check the logs for configuration errors
2. Verify User Secrets are properly initialized
3. Ensure you're using the correct project file
4. Review the `appsettings.json` structure

---

**Last Updated**: 2024  
**Maintained By**: Development Team

