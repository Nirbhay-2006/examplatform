# Exam Platform - Frontend

Modern Angular frontend for the Online Examination Platform with light/dark mode, animations, and comprehensive API integration.

## Features

✅ **Authentication System**
- User Registration with OTP verification
- Secure Login with JWT tokens
- Role-based access control

✅ **Student Module**
- Dashboard with available exams
- Full-screen exam taking with anti-cheat detection
- Real-time timer and auto-save
- Results viewing

✅ **Teacher Module**
- Create and manage exams
- Add questions (MCQ and Text)
- Monitor exam sessions
- View exam details

✅ **Admin Module**
- User management
- System dashboard with statistics
- Activate/deactivate users

✅ **UI/UX Features**
- Light/Dark mode toggle
- Smooth animations and transitions
- Responsive design
- Modern Material Design

✅ **Security Features**
- Anti-cheat detection (tab switch, copy/paste blocking)
- JWT token authentication
- Role-based guards
- Paid subscription guards

## Installation

```bash
cd Frontend
npm install
```

## Development

```bash
npm start
```

The app will be available at `http://localhost:4200`

## Build

```bash
npm run build
```

## Configuration

Update `src/environments/environment.ts` with your backend API URL:

```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api'
};
```

## Project Structure

```
src/
├── app/
│   ├── guards/          # Route guards (Auth, Role, Paid)
│   ├── interceptors/    # HTTP interceptors (Auth, Error)
│   ├── models/           # TypeScript interfaces
│   ├── pages/            # Feature pages
│   │   ├── auth/         # Login, Register, OTP
│   │   ├── student/      # Student dashboard, exam taking
│   │   ├── teacher/      # Teacher dashboard, create exam
│   │   ├── admin/        # Admin dashboard, user management
│   │   └── payment/      # Payment/upgrade page
│   ├── services/         # API, Auth, Theme services
│   ├── app.routes.ts     # Route configuration
│   └── app.ts            # Root component
├── environments/         # Environment configurations
└── styles.scss          # Global styles with theme support
```

## API Integration

All API calls are handled through:
- `ApiService` - Centralized API service
- `AuthService` - Authentication and user management
- HTTP Interceptors - Automatic token attachment and error handling

## Theme System

The app supports light and dark themes:
- Toggle via theme button in header
- Preference saved in localStorage
- Smooth transitions between themes

## Anti-Cheat Features

The exam-taking component includes:
- Full-screen mode enforcement
- Tab switch detection
- Copy/paste blocking
- Window blur detection
- Keyboard shortcut blocking
- Right-click prevention
- Violation reporting to backend

## Guards

- **AuthGuard**: Protects routes requiring authentication
- **RoleGuard**: Restricts access based on user role
- **PaidGuard**: Requires paid subscription for premium content

## Error Handling

- Global error interceptor
- User-friendly error messages
- Automatic logout on 401 errors
- Redirect to unauthorized page on 403 errors

## Browser Support

- Chrome (latest)
- Firefox (latest)
- Safari (latest)
- Edge (latest)

## Notes

- Ensure backend API is running on the configured port
- CORS must be enabled on the backend
- JWT tokens are stored in localStorage
- Theme preference persists across sessions

---

**Built with Angular 21** 🚀
