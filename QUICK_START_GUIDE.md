# 🚀 QUICK START GUIDE - Exam Platform

## ✅ Application is Running!

**Frontend:** http://localhost:4200  
**Backend API:** http://localhost:5172/api  
**Swagger UI:** http://localhost:5172/swagger

---

## 🎯 Quick Test Flow

### Step 1: Register a User

1. Open http://localhost:4200
2. Click "Register" or go to `/register`
3. Fill in:
   - Name: Your Name
   - Email: your-email@example.com
   - Password: Password123
   - Role: Student or Teacher

### Step 2: Verify OTP

1. Check your email inbox (pansheriyanirbhay@gmail.com sends from)
2. Find the OTP code (6 digits)
3. Enter OTP in the verification page
4. Verify email

### Step 3: Login

1. Go to Login page
2. Enter email and password
3. You'll receive a JWT token
4. Redirected to dashboard based on role

---

## 👨‍🎓 Student Flow

1. **Login** → Student Dashboard
2. **View Exams** → See assigned exams
3. **Start Exam** → Click "Start Exam"
4. **Take Exam**:
   - Answer MCQ questions
   - Write subjective answers
   - Auto-save as you go
   - Submit when done
5. **View Results** → Check scores after teacher publishes

---

## 👨‍🏫 Teacher Flow

1. **Login** → Teacher Dashboard
2. **Create Exam**:
   - Enter exam details
   - Set duration, marks, passing criteria
   - Choose free/paid
3. **Add Questions**:
   - Add MCQ questions with options
   - Add subjective questions
4. **Assign to Students** → Select student emails
5. **Monitor Exams** → View student submissions
6. **Evaluate Answers** → Grade subjective answers
7. **Publish Results** → Make results visible to students

---

## 👨‍💼 Admin Flow

1. **Login as Admin** → Admin Dashboard
2. **View Statistics** → Users, exams, payments
3. **Manage Users**:
   - View all users
   - Activate/Deactivate users
   - Change user roles
4. **View Payment Logs** → See all transactions

---

## 🔧 Using Swagger UI

1. Open http://localhost:5172/swagger
2. Try endpoints:
   - `POST /api/auth/register` - Register user
   - `POST /api/auth/verify-otp` - Verify OTP
   - `POST /api/auth/login` - Login
   - Explore other endpoints!

---

## 📝 Test Credentials

**Note:** Create your own credentials through registration!

### Example Registration:
```json
{
  "name": "Test User",
  "email": "test@example.com",
  "password": "Password123",
  "role": "student"
}
```

---

## ⚡ Features to Test

- ✅ User Registration with OTP
- ✅ Email Verification
- ✅ JWT Authentication
- ✅ Exam Creation (Teacher)
- ✅ Question Management
- ✅ Exam Taking (Student)
- ✅ Anti-Cheat Detection
- ✅ Answer Submission
- ✅ Result Evaluation
- ✅ Result Publishing
- ✅ Payment System
- ✅ Admin Dashboard
- ✅ User Management
- ✅ Theme Toggle (Light/Dark)

---

## 🐛 Troubleshooting

### If OTP email doesn't arrive:
- Check spam folder
- Verify email service is configured
- Check backend logs for errors

### If API errors occur:
- Check MongoDB is running: `Get-Service MongoDB`
- Verify backend is running on port 5172
- Check Swagger UI for endpoint details

### If Frontend doesn't load:
- Verify Angular dev server is running
- Check console for errors
- Ensure API URL is correct: `http://localhost:5172/api`

---

## 🎉 Enjoy Testing!

**Application is fully functional and ready to use!**

For detailed documentation, see:
- `README.md` - Complete project overview
- `COMPLETION_ASSESSMENT.md` - Feature checklist
- `FUNCTIONALITY_CHECK_SUMMARY.md` - Status report

