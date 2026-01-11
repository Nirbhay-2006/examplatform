# ⚡ EXAM PLATFORM - QUICK START GUIDE

## 🚀 Get Running in 5 Minutes!

---

## Step 1: Install Prerequisites (5 min)

### Download & Install:
1. **.NET 8 SDK** → https://dotnet.microsoft.com/download/dotnet/8.0
2. **MongoDB** → https://www.mongodb.com/try/download/community
3. **Postman** (optional) → https://www.postman.com/downloads/

---

## Step 2: Configure Email (2 min)

### Get Gmail App Password:
1. Go to: https://myaccount.google.com/apppasswords
2. Generate password for "Mail" + "Windows Computer"
3. Copy the 16-character password

### Update Configuration:
Edit: `Backend\ExamPlatform.API\appsettings.json`

```json
"EmailSettings": {
  "SenderEmail": "your-email@gmail.com",
  "Password": "your-16-char-password"
}
```

---

## Step 3: Start MongoDB (1 min)

```bash
# Windows
net start MongoDB

# Or manually
"C:\Program Files\MongoDB\Server\7.0\bin\mongod.exe"
```

---

## Step 4: Run the API (1 min)

### Option A: Using Script
```bash
cd Backend
start.bat
```

### Option B: Manual
```bash
cd Backend\ExamPlatform.API
dotnet restore
dotnet run
```

**✅ API Running at:** http://localhost:5000  
**✅ Swagger UI:** http://localhost:5000/swagger

---

## Step 5: Test the API (5 min)

### Quick Test with Swagger:

1. **Open:** http://localhost:5000/swagger

2. **Register a Student:**
   - Click on `POST /api/auth/register`
   - Click "Try it out"
   - Use this JSON:
   ```json
   {
     "name": "John Doe",
     "email": "your-email@gmail.com",
     "password": "Password123",
     "role": "student"
   }
   ```
   - Click "Execute"

3. **Check Your Email** for OTP code

4. **Verify OTP:**
   - Click on `POST /api/auth/verify-otp`
   - Use:
   ```json
   {
     "email": "your-email@gmail.com",
     "otpCode": "123456"
   }
   ```

5. **Login:**
   - Click on `POST /api/auth/login`
   - Use:
   ```json
   {
     "email": "your-email@gmail.com",
     "password": "Password123"
   }
   ```
   - **Copy the token** from response

6. **Authorize:**
   - Click the "Authorize" button at top
   - Paste: `Bearer YOUR_TOKEN_HERE`
   - Click "Authorize"

7. **Test Protected Endpoint:**
   - Click on `GET /api/exam/student/my-exams`
   - Click "Execute"
   - Should return empty array (no exams yet)

**✅ If you see `[]` - Everything is working!**

---

## 🎯 Complete Test Flow

### Create Full Exam Scenario:

**1. Register Teacher:**
```json
POST /api/auth/register
{
  "name": "Teacher Smith",
  "email": "teacher@example.com",
  "password": "Password123",
  "role": "teacher"
}
```

**2. Verify & Login Teacher** (get teacher token)

**3. Create Exam:**
```json
POST /api/exam
Authorization: Bearer TEACHER_TOKEN
{
  "title": "Math Final",
  "description": "Final exam",
  "duration": 60,
  "totalMarks": 100,
  "passingMarks": 40,
  "startTime": "2024-12-20T10:00:00Z",
  "endTime": "2024-12-20T11:00:00Z",
  "isPaid": false,
  "maxViolations": 3
}
```
**Copy exam ID from response**

**4. Add Question:**
```json
POST /api/exam/{EXAM_ID}/questions
Authorization: Bearer TEACHER_TOKEN
{
  "questionText": "What is 2 + 2?",
  "questionType": "mcq",
  "options": ["2", "3", "4", "5"],
  "correctAnswer": "4",
  "marks": 10
}
```

**5. Assign to Student:**
```json
POST /api/exam/{EXAM_ID}/assign
Authorization: Bearer TEACHER_TOKEN
["STUDENT_USER_ID"]
```

**6. Student Takes Exam:**
```json
# Get exams
GET /api/exam/student/my-exams
Authorization: Bearer STUDENT_TOKEN

# Start exam
POST /api/exam/{EXAM_ID}/start
Authorization: Bearer STUDENT_TOKEN

# Get questions
GET /api/exam/{EXAM_ID}/questions
Authorization: Bearer STUDENT_TOKEN

# Submit answer
POST /api/exam/{EXAM_ID}/answer
Authorization: Bearer STUDENT_TOKEN
{
  "questionId": "QUESTION_ID",
  "answer": "4"
}

# Submit exam
POST /api/exam/{EXAM_ID}/submit
Authorization: Bearer STUDENT_TOKEN
```

**7. Teacher Publishes Results:**
```json
POST /api/result/{EXAM_ID}/publish
Authorization: Bearer TEACHER_TOKEN
```

**8. Student Views Result:**
```json
GET /api/result/my-results
Authorization: Bearer STUDENT_TOKEN
```

---

## 🔧 Troubleshooting

### MongoDB Not Starting?
```bash
# Check if running
mongosh

# If error, start manually
"C:\Program Files\MongoDB\Server\7.0\bin\mongod.exe"
```

### Email Not Sending?
- Verify Gmail App Password is correct
- Check 2FA is enabled on Gmail
- Try with different email

### Port 5000 Already in Use?
```bash
# Find process
netstat -ano | findstr :5000

# Kill process
taskkill /PID <process_id> /F
```

### JWT Token Error?
- Format must be: `Bearer <token>`
- Token expires in 24 hours
- Re-login to get new token

---

## 📁 Important Files

| File | Purpose |
|------|---------|
| `Backend/start.bat` | Quick start script |
| `Backend/README.md` | Full documentation |
| `DEPLOYMENT_GUIDE.md` | Deployment instructions |
| `Backend/ExamPlatform.postman_collection.json` | Postman tests |
| `Backend/ExamPlatform.API/appsettings.json` | Configuration |

---

## 🎯 What's Working?

✅ User Registration with OTP  
✅ Email Verification  
✅ JWT Authentication  
✅ Role-Based Access (Student/Teacher/Admin)  
✅ Exam Creation & Management  
✅ Question Bank (MCQ + Subjective)  
✅ Exam Taking & Submission  
✅ Anti-Cheat Violation Tracking  
✅ Auto-Evaluation (MCQ)  
✅ Manual Evaluation (Subjective)  
✅ Result Publishing  
✅ Payment System (Free/Paid)  
✅ Admin Dashboard  

---

## 📊 Database Check

```bash
# Open MongoDB shell
mongosh

# Switch to database
use ExamPlatformDB

# View collections
show collections

# Check users
db.users.find().pretty()

# Check exams
db.exams.find().pretty()
```

---

## 🚀 Next Steps

1. ✅ **Test locally** using Swagger or Postman
2. ✅ **Deploy to cloud** (Azure/AWS/Heroku)
3. 🔄 **Build Angular frontend** (Phase 2)
4. 🔄 **Add real payment gateway** (Razorpay/Stripe)
5. 🔄 **Add analytics dashboard**

---

## 📞 Need Help?

1. Check `DEPLOYMENT_GUIDE.md` for detailed instructions
2. Review `Backend/README.md` for API documentation
3. Use Swagger UI for interactive testing
4. Check console logs for errors

---

## 🎉 Success Checklist

- [ ] MongoDB is running
- [ ] Email is configured
- [ ] API starts without errors
- [ ] Swagger UI loads
- [ ] Can register user
- [ ] Receive OTP email
- [ ] Can verify OTP
- [ ] Can login and get token
- [ ] Can access protected endpoints

**If all checked ✅ - You're ready to go!**

---

## 🔗 Quick Links

- **API:** http://localhost:5000
- **Swagger:** http://localhost:5000/swagger
- **MongoDB:** mongodb://localhost:27017

---

## 💡 Pro Tips

1. **Use Postman Collection** for faster testing
2. **Keep Swagger UI open** for quick reference
3. **Check MongoDB** to verify data is saving
4. **Save tokens** in Postman variables
5. **Test with multiple users** (student + teacher)

---

**⚡ You're all set! Start testing your exam platform now!**

```bash
cd Backend
start.bat
```

Then open: **http://localhost:5000/swagger** 🚀
