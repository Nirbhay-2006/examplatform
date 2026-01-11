# 🚀 Quick Start Commands

## Prerequisites
- .NET 8.0 SDK installed
- Node.js and npm installed
- MongoDB running (or Docker with MongoDB)

---

## 📋 Running the Application

### Option 1: Run Separately (Recommended for Development)

#### **Backend (API Server)**

Open a terminal/command prompt and run:

```bash
# Navigate to backend directory
cd Backend/ExamPlatformApi

# Restore packages (first time only)
dotnet restore ExamPlatformApi.csproj

# Run the backend (specify project file to avoid ambiguity)
dotnet run --project ExamPlatformApi.csproj
```

**Backend will start at:**
- HTTP: `http://localhost:5172`
- HTTPS: `https://localhost:7163`
- Swagger UI: `http://localhost:5172/swagger` or `https://localhost:7163/swagger`

---

#### **Frontend (Angular App)**

Open a **NEW** terminal/command prompt and run:

```bash
# Navigate to frontend directory
cd Frontend

# Install dependencies (first time only)
npm install

# Run the frontend
npm start
```

**Frontend will start at:**
- `http://localhost:4200` (configured in `angular.json` and `package.json`)

---

### Option 2: Using Batch Scripts (Windows)

#### **Backend Only:**
```bash
cd Backend
start.bat
```

#### **Frontend Only:**
```bash
cd Frontend
npm start
```

---

### Option 3: Run Both Together (PowerShell/Windows)

Create a PowerShell script or run in separate terminals:

**Terminal 1 (Backend):**
```powershell
cd Backend/ExamPlatformApi
dotnet run
```

**Terminal 2 (Frontend):**
```powershell
cd Frontend
npm start
```

---

## 🔧 Development Commands

### Backend Commands

```bash
# Restore packages (specify project file)
dotnet restore ExamPlatformApi.csproj

# Build project
dotnet build ExamPlatformApi.csproj

# Run the backend
dotnet run --project ExamPlatformApi.csproj

# Run with watch (auto-reload on changes)
dotnet watch run --project ExamPlatformApi.csproj

# Run specific profile
dotnet run --project ExamPlatformApi.csproj --launch-profile https

# Clean build artifacts
dotnet clean ExamPlatformApi.csproj
```

**Note:** Always specify `ExamPlatformApi.csproj` as the project file since there are two .csproj files in the directory.

### Frontend Commands

```bash
# Install dependencies
npm install

# Start development server
npm start
# or
ng serve

# Build for production
npm run build
# or
ng build

# Run with specific port
ng serve --port 4200

# Run with open browser
ng serve --open
```

---

## 🌐 Access Points

Once both servers are running:

- **Frontend Application:** http://localhost:4200
- **Backend API:** http://localhost:5172
- **Swagger API Docs:** http://localhost:5172/swagger
- **HTTPS Backend:** https://localhost:7163

---

## ⚠️ Important Notes

1. **MongoDB must be running** before starting the backend
2. **Backend must start first** before frontend can connect
3. **CORS is enabled** for development (localhost:4200)
4. **Environment files** may need configuration:
   - Backend: `appsettings.Development.json`
   - Frontend: `src/environments/environment.ts`

---

## 🐛 Troubleshooting

### Backend won't start?
- Check if MongoDB is running: `mongod --version`
- Check if port 5172 is available
- Verify .NET SDK: `dotnet --version` (should be 8.0+)
- **If you see "more than one project file" error**: Always use `dotnet run --project ExamPlatformApi.csproj` to explicitly specify the project file

### Frontend won't start?
- Check if Node.js is installed: `node --version`
- Check if port 4200 is available
- Try deleting `node_modules` and running `npm install` again

### Connection issues?
- Ensure backend is running before frontend
- Check API URL in `Frontend/src/environments/environment.ts`
- Verify CORS settings in backend `Program.cs`

---

## 📝 Quick Reference

| Service | Command | Port | URL |
|---------|---------|------|-----|
| Backend | `dotnet run` | 5172 | http://localhost:5172 |
| Frontend | `npm start` | 4200 | http://localhost:4200 |
| Swagger | Auto | 5172 | http://localhost:5172/swagger |

