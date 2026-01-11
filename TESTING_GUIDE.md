# Complete Application Testing Guide

## Prerequisites

1. **MongoDB** must be running on `localhost:27017`
   - Download from: https://www.mongodb.com/try/download/community
   - Or use Docker: `docker run -d -p 27017:27017 mongo`

2. **.NET 8.0 SDK** installed
3. **Node.js** (v18+) and npm installed

## Step 1: Start MongoDB

### Option A: Using Docker
```bash
docker run -d -p 27017:27017 --name mongodb mongo
```

### Option B: Using MongoDB Service
- Start MongoDB service on Windows
- Or run `mongod` command

### Verify MongoDB is running:
```bash
# Test connection
mongosh mongodb://localhost:27017
```

## Step 2: Start Backend API

```bash
cd Backend/ExamPlatformApi
dotnet run --project ExamPlatformApi.csproj
```

The API will start at: `http://localhost:5000` or `https://localhost:5001`

**Verify Backend:**
- Open browser: `http://localhost:5000/swagger`
- You should see Swagger UI with all API endpoints

## Step 3: Start Frontend

Open a **new terminal**:

```bash
cd Frontend
npm install
npm start
```

The frontend will start at: `http://localhost:4200`

## Step 4: Test Complete Flow

### Test 1: User Registration

1. Open `http://localhost:4200`
2. Click "Sign Up" or navigate to `/register`
3. Fill in the form:
   - Name: `Test User`
   - Email: `test@example.com`
   - Password: `password123`
   - Role: `Student`
4. Click "Create Account"
5. **Expected**: Success message, redirect to OTP verification

### Test 2: OTP Verification

1. Check email for OTP code (sent to `pansheriyanirbhay@gmail.com`)
2. Enter the 6-digit OTP
3. Click "Verify OTP"
4. **Expected**: Success message, redirect to login

### Test 3: Login

1. On login page, enter:
   - Email: `test@example.com`
   - Password: `password123`
2. Click "Sign In"
3. **Expected**: Redirect to Student Dashboard

### Test 4: Student Dashboard

1. **Expected to see**:
   - Welcome message with user name
   - Available exams list
   - Stats (Available Exams, Completed Exams, Subscription)
   - Theme toggle button

### Test 5: Create Exam (Teacher)

1. Register a new user with role "Teacher"
2. Login as teacher
3. Navigate to Teacher Dashboard
4. Click "Create Exam"
5. Fill exam details:
   - Title: `Test Exam`
   - Description: `This is a test exam`
   - Duration: `60` minutes
   - Total Marks: `100`
   - Passing Marks: `40`
   - Start Time: Future date/time
   - End Time: Future date/time
   - Is Paid: `false`
6. Add questions:
   - Click "Add Question"
   - Question Text: `What is 2+2?`
   - Type: `MCQ`
   - Options: 
     ```
     2
     3
     4
     5
     ```
   - Correct Answer: `4`
   - Marks: `10`
7. Click "Create Exam"
8. **Expected**: Success message, redirect to dashboard

### Test 6: Assign Exam to Student

1. As teacher, view exam details
2. Assign exam to student (student email/user ID)
3. **Expected**: Exam appears in student's dashboard

### Test 7: Take Exam (Student)

1. Login as student
2. Click "Start Exam" on an assigned exam
3. **Expected**:
   - Full-screen mode activated
   - Timer starts
   - Questions displayed
   - Navigation panel on left
4. Answer questions
5. **Test Anti-Cheat**:
   - Try switching tabs → Violation recorded
   - Try copy/paste → Blocked
   - Try right-click → Blocked
6. Submit exam
7. **Expected**: Success message, redirect to dashboard

### Test 8: Admin Dashboard

1. Register/Login as admin (or update user role in database)
2. Navigate to `/admin/dashboard`
3. **Expected to see**:
   - Total Users
   - Total Students
   - Total Teachers
   - Total Exams

### Test 9: Theme Toggle

1. Click theme toggle button (🌙/☀️) on any page
2. **Expected**: Smooth transition between light/dark mode
3. Refresh page
4. **Expected**: Theme preference persists

### Test 10: Error Handling

1. Try to access `/admin/dashboard` as student
2. **Expected**: Redirect to unauthorized page
3. Try to access protected route without login
4. **Expected**: Redirect to login page

## API Testing with Swagger

1. Open `http://localhost:5000/swagger`
2. Test endpoints:
   - POST `/api/auth/register` - Register user
   - POST `/api/auth/verify-otp` - Verify OTP
   - POST `/api/auth/login` - Login
   - GET `/api/exam/student/my-exams` - Get student exams (requires auth)

## Common Issues & Solutions

### Issue 1: MongoDB Connection Error
**Error**: `Unable to connect to MongoDB`
**Solution**: 
- Ensure MongoDB is running
- Check connection string in `appsettings.json`
- Verify port 27017 is not blocked

### Issue 2: CORS Error
**Error**: `CORS policy blocked`
**Solution**: 
- Backend CORS is configured for development
- Check `Program.cs` CORS settings
- Ensure frontend URL matches allowed origins

### Issue 3: JWT Token Error
**Error**: `Unauthorized` or `Invalid token`
**Solution**:
- Check JWT secret in User Secrets
- Verify token expiry settings
- Clear localStorage and login again

### Issue 4: Email Not Sending
**Error**: OTP email not received
**Solution**:
- Check email credentials in User Secrets
- Verify SMTP settings
- Check spam folder
- For testing, check backend logs for OTP code

### Issue 5: Frontend Build Errors
**Error**: `Module not found` or compilation errors
**Solution**:
```bash
cd Frontend
rm -rf node_modules package-lock.json
npm install
```

### Issue 6: Port Already in Use
**Error**: `Port 5000 or 4200 already in use`
**Solution**:
- Change port in `launchSettings.json` (backend)
- Change port in `angular.json` (frontend)
- Or kill process using the port

## Quick Test Checklist

- [ ] MongoDB is running
- [ ] Backend builds successfully
- [ ] Backend starts without errors
- [ ] Swagger UI is accessible
- [ ] Frontend builds successfully
- [ ] Frontend starts without errors
- [ ] Can register new user
- [ ] Can verify OTP
- [ ] Can login
- [ ] Student dashboard loads
- [ ] Teacher can create exam
- [ ] Student can take exam
- [ ] Anti-cheat detection works
- [ ] Theme toggle works
- [ ] Guards protect routes correctly
- [ ] API integration works

## Performance Testing

1. **Load Testing**: Create multiple users and exams
2. **Concurrent Exams**: Multiple students taking exams simultaneously
3. **Large Exams**: Create exam with 50+ questions
4. **Network Testing**: Test with slow network (throttle in DevTools)

## Security Testing

1. **Authentication**: Try accessing protected routes without token
2. **Authorization**: Try accessing admin routes as student
3. **Input Validation**: Try submitting invalid data
4. **SQL Injection**: Test with malicious input (MongoDB injection)
5. **XSS**: Test with script tags in inputs

## Browser Testing

Test in:
- [ ] Chrome (latest)
- [ ] Firefox (latest)
- [ ] Safari (latest)
- [ ] Edge (latest)
- [ ] Mobile browsers (responsive design)

## Automated Testing (Future)

```bash
# Backend tests
cd Backend/ExamPlatformApi
dotnet test

# Frontend tests
cd Frontend
npm test
```

---

## Success Criteria

✅ All features work as expected
✅ No console errors
✅ No API errors
✅ Smooth user experience
✅ Responsive design works
✅ Theme switching works
✅ Anti-cheat detection works
✅ All guards protect routes correctly

---

**Happy Testing!** 🚀

