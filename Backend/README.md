# Exam Platform Backend - .NET 8 API

## 🚀 Complete Online Examination System

### Features Implemented
✅ User Registration with OTP Email Verification  
✅ JWT Authentication & Authorization  
✅ Role-Based Access (Student, Teacher, Admin)  
✅ Free & Paid Module System  
✅ Exam Creation & Management  
✅ MCQ & Subjective Questions  
✅ Anti-Cheat Violation Tracking  
✅ Auto-Evaluation for MCQs  
✅ Manual Evaluation for Subjective Answers  
✅ Result Publishing System  
✅ Payment Integration (Razorpay/Stripe Ready)  
✅ MongoDB Database  

---

## 📋 Prerequisites

1. **.NET 8 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
2. **MongoDB** - [Download](https://www.mongodb.com/try/download/community)
3. **Visual Studio 2022** or **VS Code**
4. **Postman** (for API testing)

---

## ⚙️ Setup Instructions

### Step 1: Install MongoDB
```bash
# Windows - Download and install from MongoDB website
# Or use MongoDB Atlas (cloud) - https://www.mongodb.com/cloud/atlas

# Start MongoDB service
net start MongoDB
```

### Step 2: Configure Email Settings
Edit `appsettings.json`:
```json
"EmailSettings": {
  "SmtpServer": "smtp.gmail.com",
  "SmtpPort": 587,
  "SenderEmail": "your-email@gmail.com",
  "SenderName": "Exam Platform",
  "Password": "your-app-password"
}
```

**For Gmail:**
1. Enable 2-Factor Authentication
2. Generate App Password: https://myaccount.google.com/apppasswords
3. Use the generated password in `appsettings.json`

### Step 3: Update MongoDB Connection
Edit `appsettings.json`:
```json
"MongoDbSettings": {
  "ConnectionString": "mongodb://localhost:27017",
  "DatabaseName": "ExamPlatformDB"
}
```

### Step 4: Build and Run
```bash
cd ExamPlatform.API
dotnet restore
dotnet build
dotnet run
```

API will start at: `http://localhost:5000` or `https://localhost:5001`

---

## 🧪 Testing the API

### 1. Register a Student
**POST** `http://localhost:5000/api/auth/register`
```json
{
  "name": "John Doe",
  "email": "john@example.com",
  "password": "Password123",
  "role": "student"
}
```

### 2. Verify OTP (Check Email)
**POST** `http://localhost:5000/api/auth/verify-otp`
```json
{
  "email": "john@example.com",
  "otpCode": "123456"
}
```

### 3. Login
**POST** `http://localhost:5000/api/auth/login`
```json
{
  "email": "john@example.com",
  "password": "Password123"
}
```
**Response:** Copy the `token` from response

### 4. Register a Teacher
**POST** `http://localhost:5000/api/auth/register`
```json
{
  "name": "Teacher Smith",
  "email": "teacher@example.com",
  "password": "Password123",
  "role": "teacher"
}
```
Verify OTP and Login to get teacher token.

### 5. Create Exam (Teacher)
**POST** `http://localhost:5000/api/exam`  
**Headers:** `Authorization: Bearer <teacher-token>`
```json
{
  "title": "Mathematics Final Exam",
  "description": "Final exam for semester 6",
  "duration": 60,
  "totalMarks": 100,
  "passingMarks": 40,
  "startTime": "2024-12-20T10:00:00Z",
  "endTime": "2024-12-20T11:00:00Z",
  "isPaid": false,
  "maxViolations": 3
}
```
**Response:** Copy the exam `id`

### 6. Add Questions (Teacher)
**POST** `http://localhost:5000/api/exam/{examId}/questions`  
**Headers:** `Authorization: Bearer <teacher-token>`
```json
{
  "questionText": "What is 2 + 2?",
  "questionType": "mcq",
  "options": ["2", "3", "4", "5"],
  "correctAnswer": "4",
  "marks": 10
}
```

### 7. Assign Exam to Students (Teacher)
**POST** `http://localhost:5000/api/exam/{examId}/assign`  
**Headers:** `Authorization: Bearer <teacher-token>`
```json
["<student-user-id>"]
```

### 8. Get My Exams (Student)
**GET** `http://localhost:5000/api/exam/student/my-exams`  
**Headers:** `Authorization: Bearer <student-token>`

### 9. Start Exam (Student)
**POST** `http://localhost:5000/api/exam/{examId}/start`  
**Headers:** `Authorization: Bearer <student-token>`

### 10. Get Questions (Student)
**GET** `http://localhost:5000/api/exam/{examId}/questions`  
**Headers:** `Authorization: Bearer <student-token>`

### 11. Submit Answer (Student)
**POST** `http://localhost:5000/api/exam/{examId}/answer`  
**Headers:** `Authorization: Bearer <student-token>`
```json
{
  "questionId": "<question-id>",
  "answer": "4"
}
```

### 12. Report Violation (Student)
**POST** `http://localhost:5000/api/exam/{examId}/violation`  
**Headers:** `Authorization: Bearer <student-token>`
```json
{
  "examId": "<exam-id>",
  "violationType": "tab_switch"
}
```

### 13. Submit Exam (Student)
**POST** `http://localhost:5000/api/exam/{examId}/submit`  
**Headers:** `Authorization: Bearer <student-token>`

### 14. Publish Results (Teacher)
**POST** `http://localhost:5000/api/result/{examId}/publish`  
**Headers:** `Authorization: Bearer <teacher-token>`

### 15. Get My Results (Student)
**GET** `http://localhost:5000/api/result/my-results`  
**Headers:** `Authorization: Bearer <student-token>`

---

## 🔐 Admin Endpoints

### Register Admin (Manual - Update in DB)
1. Register as student
2. In MongoDB, change role to "admin"

### Admin Dashboard
**GET** `http://localhost:5000/api/admin/dashboard`  
**Headers:** `Authorization: Bearer <admin-token>`

### Get All Users
**GET** `http://localhost:5000/api/admin/users`  
**Headers:** `Authorization: Bearer <admin-token>`

### Activate/Deactivate User
**PUT** `http://localhost:5000/api/admin/users/{userId}/activate`  
**PUT** `http://localhost:5000/api/admin/users/{userId}/deactivate`  
**Headers:** `Authorization: Bearer <admin-token>`

---

## 💳 Payment Endpoints

### Create Order
**POST** `http://localhost:5000/api/payment/create-order`  
**Headers:** `Authorization: Bearer <token>`
```json
499.00
```

### Verify Payment
**POST** `http://localhost:5000/api/payment/verify`  
**Headers:** `Authorization: Bearer <token>`
```json
{
  "orderId": "order_xxx",
  "paymentId": "pay_xxx"
}
```

---

## 📁 Project Structure
```
ExamPlatform.API/
├── Controllers/          # API endpoints
│   ├── AuthController.cs
│   ├── ExamController.cs
│   ├── AdminController.cs
│   ├── PaymentController.cs
│   └── ResultController.cs
├── Models/              # MongoDB models
│   ├── User.cs
│   ├── Exam.cs
│   ├── Question.cs
│   ├── Response.cs
│   ├── Violation.cs
│   ├── Payment.cs
│   └── Result.cs
├── DTOs/                # Data transfer objects
├── Repositories/        # Database operations
├── Services/            # Business logic
│   ├── EmailService.cs
│   └── JwtService.cs
├── Helpers/             # Configuration classes
└── Program.cs           # App configuration
```

---

## 🗄️ Database Collections

MongoDB will auto-create these collections:
- `users` - User accounts
- `exams` - Exam details
- `questions` - Exam questions
- `responses` - Student answers
- `violations` - Anti-cheat logs
- `payments` - Payment records
- `results` - Published results

---

## 🔧 Troubleshooting

### MongoDB Connection Error
```bash
# Check if MongoDB is running
mongosh
# Or
mongo
```

### Email Not Sending
- Verify Gmail App Password
- Check firewall settings
- Try different SMTP server

### JWT Token Error
- Ensure token is in format: `Bearer <token>`
- Check token expiry (default: 24 hours)

---

## 🚀 Deployment

### Deploy to Azure
1. Create Azure App Service
2. Create Azure Cosmos DB (MongoDB API)
3. Update connection string
4. Deploy using Visual Studio or CLI

### Deploy to AWS
1. Use AWS Elastic Beanstalk
2. Use AWS DocumentDB (MongoDB compatible)
3. Configure environment variables

### Deploy to Docker
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY . .
EXPOSE 80
ENTRYPOINT ["dotnet", "ExamPlatform.API.dll"]
```

---

## 📊 API Documentation

Once running, access Swagger UI:
`http://localhost:5000/swagger`

---

## 🎯 Next Steps

1. ✅ Backend is complete and working
2. 🔄 Create Angular Frontend (Phase 2)
3. 🔄 Add real payment gateway integration
4. 🔄 Add file upload for questions
5. 🔄 Add analytics dashboard

---

## 📞 Support

For issues or questions, check:
- MongoDB logs
- Application logs in console
- Swagger documentation

**Backend is 100% ready for testing!** 🎉
