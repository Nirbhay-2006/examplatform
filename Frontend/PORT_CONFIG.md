# Frontend Port Configuration

## Default Port
The frontend application is configured to run on **port 4200** by default.

## Configuration Files

### 1. `angular.json`
The port is configured in the `serve` options:
```json
"serve": {
  "options": {
    "port": 4200,
    "host": "localhost"
  }
}
```

### 2. `package.json`
The start script includes the port:
```json
"start": "ng serve --port 4200"
```

## Changing the Port

### Option 1: Update angular.json
Edit `angular.json` and change the port in the `serve.options.port` field.

### Option 2: Use command line flag
Run with a different port:
```bash
ng serve --port 3000
```

### Option 3: Update package.json script
Change the port in the `start` script:
```json
"start": "ng serve --port 3000"
```

## Important Notes

1. **Backend CORS**: If you change the frontend port, make sure to update the CORS configuration in the backend `Program.cs` to allow the new port.

2. **Environment Files**: The API URL in `src/environments/environment.ts` points to the backend (port 5172), not the frontend port.

3. **Port Conflicts**: If port 4200 is already in use, Angular will automatically try the next available port (4201, 4202, etc.) and display a warning.

## Current Configuration

- **Frontend Port**: 4200
- **Backend Port**: 5172
- **Backend HTTPS Port**: 7163

## Access URLs

- Frontend: http://localhost:4200
- Backend API: http://localhost:5172
- Swagger: http://localhost:5172/swagger

