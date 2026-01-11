# ✅ EXAM PLATFORM - TESTING CHECKLIST

## 🎯 Complete Testing Guide

Use this checklist to verify all features are working correctly.

---

## 📋 Pre-Testing Setup

### Environment Setup
- [ ] .NET 8 SDK installed
- [ ] MongoDB installed and running
- [ ] Email configured in appsettings.json
- [ ] API running at http://localhost:5000
- [ ] Swagger UI accessible at http://localhost:5000/swagger

### Tools Ready
- [ ] Postman installed (optional)
- [ ] MongoDB Compass or mongosh (optional)
- [ ] Browser for Swagger UI

---

## 🧪 Test Scenarios

### 1. Authentication Flow ✅

#### 1.1 Student Registration
- [ ] POST `/api/auth/register` with student role
- [ ] Verify 200 OK response
- [ ] Check email received OTP
- [ ] Verify OTP code is 6 digits
- [ ] Check MongoDB: user created with `isVerified: false`

**Test Data:**
```json
{
  "name": "John Doe",
  "email": "student@test.com",
  "password": "Password123",
  "role": "student"
}
```

**Expected:** Success message + OTP email sent

#### 1.2 OTP Verification
- [ ] POST `/api/auth/verify-otp` with correct OTP
- [ ] Verify 200 OK response
- [ ] Check MongoDB: `isVerified: true`
- [ ] Test with wrong OTP (should fail)
- [ ] Test with expired OTP (should fail)

**Test Data:**
```json
{
  "email": "student@test.com",
  "otpCode": "123456"
}
```

**Expected:** Email verified successfully

#### 1.3 Login
- [ ] POST `/api/auth/login` with correct credentials
- [ ] Verify 200 OK response
- [ ] Receive JWT token
- [ ] Token contains userId, email, role, subscriptionType
- [ ] Test with wrong password (should fail)
- [ ] Test with unverified email (should fail)

**Test Data:**
```json
{
  "email": "student@test.com",
  "password": "Password123"
}
```

**Expected:** Token + user details

#### 1.4 Teacher Registration
- [ ] Register teacher account
- [ ] Verify OTP
- [ ] Login and get teacher token
- [ ] Verify role is "teacher"

#### 1.5 Resend OTP
- [ ] POST `/api/auth/resend-otp`
- [ ] Verify new OTP received
- [ ] Old OTP should not work

---

### 2. Exam Management (Teacher) ✅

#### 2.1 Create Exam
- [ ] POST `/api/exam` with teacher token
- [ ] Verify 200 OK response
- [ ] Exam created in MongoDB
- [ ] Check all fields saved correctly
- [ ] Test without token (should fail - 401)
- [ ] Test with student token (should fail - 403)

**Test Data:**
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

**Expected:** Exam created with ID

#### 2.2 Add MCQ Question
- [ ] POST `/api/exam/{examId}/questions`
- [ ] Verify question added
- [ ] Check MongoDB: question linked to exam
- [ ] Verify order is auto-incremented

**Test Data:**
```json
{
  "questionText": "What is 2 + 2?",
  "questionType": "mcq",
  "options": ["2", "3", "4", "5"],
  "correctAnswer": "4",
  "marks": 10
}
```

**Expected:** Question added successfully

#### 2.3 Add Subjective Question
- [ ] POST `/api/exam/{examId}/questions`
- [ ] Verify question added
- [ ] Options array can be empty

**Test Data:**
```json
{
  "questionText": "Explain Newton's laws of motion.",
  "questionType": "text",
  "options": [],
  "correctAnswer": "",
  "marks": 20
}
```

**Expected:** Question added successfully

#### 2.4 Get My Exams
- [ ] GET `/api/exam/teacher/my-exams`
- [ ] Verify only teacher's exams returned
- [ ] Check exam count matches created exams

**Expected:** List of teacher's exams

#### 2.5 Assign Exam to Students
- [ ] POST `/api/exam/{examId}/assign`
- [ ] Verify students added to assignedStudents array
- [ ] Check MongoDB: exam updated

**Test Data:**
```json
["student-user-id-1", "student-user-id-2"]
```

**Expected:** Exam assigned successfully

---

### 3. Student Exam Flow ✅

#### 3.1 Get My Exams
- [ ] GET `/api/exam/student/my-exams` with student token
- [ ] Verify only assigned exams returned
- [ ] Paid exams filtered based on subscription
- [ ] Check exam details are complete

**Expected:** List of assigned exams

#### 3.2 Start Exam
- [ ] POST `/api/exam/{examId}/start`
- [ ] Verify response created in MongoDB
- [ ] Check startedAt timestamp
- [ ] Status is "in_progress"
- [ ] Test starting same exam twice (should fail)

**Expected:** Exam started, response ID returned

#### 3.3 Get Questions
- [ ] GET `/api/exam/{examId}/questions`
- [ ] Verify questions returned
- [ ] Correct answers NOT included
- [ ] Questions in correct order
- [ ] All question details present

**Expected:** List of questions without answers

#### 3.4 Submit Answer (Auto-save)
- [ ] POST `/api/exam/{examId}/answer`
- [ ] Verify answer saved
- [ ] Check MongoDB: answer in responses collection
- [ ] Submit same question again (should update)

**Test Data:**
```json
{
  "questionId": "question-id",
  "answer": "4"
}
```

**Expected:** Answer saved

#### 3.5 Report Violation
- [ ] POST `/api/exam/{examId}/violation`
- [ ] Verify violation logged
- [ ] Check violationCount incremented
- [ ] Test max violations (should auto-submit)

**Test Data:**
```json
{
  "examId": "exam-id",
  "violationType": "tab_switch"
}
```

**Expected:** Violation recorded

#### 3.6 Submit Exam
- [ ] POST `/api/exam/{examId}/submit`
- [ ] Verify submittedAt timestamp set
- [ ] Status changed to "submitted"
- [ ] MCQ answers auto-evaluated
- [ ] Marks calculated for MCQs
- [ ] Check MongoDB: response updated

**Expected:** Exam submitted successfully

---

### 4. Result Management ✅

#### 4.1 Evaluate Subjective Answer (Teacher)
- [ ] POST `/api/result/evaluate`
- [ ] Verify marks assigned
- [ ] Check MongoDB: answer updated

**Test Data:**
```json
{
  "responseId": "response-id",
  "questionId": "question-id",
  "marksObtained": 15
}
```

**Expected:** Answer evaluated

#### 4.2 Publish Results (Teacher)
- [ ] POST `/api/result/{examId}/publish`
- [ ] Verify results created for all students
- [ ] Check percentage calculated
- [ ] Pass/fail status correct
- [ ] Exam marked as published
- [ ] Response status changed to "evaluated"

**Expected:** Results published

#### 4.3 Get My Results (Student)
- [ ] GET `/api/result/my-results`
- [ ] Verify all results returned
- [ ] Check marks, percentage, pass/fail
- [ ] Results only for published exams

**Expected:** List of results

#### 4.4 Get Result by Exam (Student)
- [ ] GET `/api/result/exam/{examId}`
- [ ] Verify specific exam result
- [ ] All details present

**Expected:** Single result object

---

### 5. Payment System ✅

#### 5.1 Create Order
- [ ] POST `/api/payment/create-order`
- [ ] Verify order created
- [ ] Check orderId generated
- [ ] Status is "pending"

**Test Data:**
```json
499.00
```

**Expected:** Order created with ID

#### 5.2 Verify Payment
- [ ] POST `/api/payment/verify`
- [ ] Verify payment status updated
- [ ] User subscription changed to "paid"
- [ ] Check MongoDB: user and payment updated

**Test Data:**
```json
{
  "orderId": "order_xxx",
  "paymentId": "pay_xxx"
}
```

**Expected:** Payment verified, subscription activated

#### 5.3 Get Payment History
- [ ] GET `/api/payment/my-payments`
- [ ] Verify all user payments returned
- [ ] Check payment details

**Expected:** List of payments

#### 5.4 Access Paid Exam
- [ ] Student with free subscription tries paid exam (should fail)
- [ ] Student with paid subscription accesses paid exam (should work)

---

### 6. Admin Module ✅

#### 6.1 Get All Users
- [ ] GET `/api/admin/users` with admin token
- [ ] Verify all users returned
- [ ] Check user details
- [ ] Test with non-admin token (should fail)

**Expected:** List of all users

#### 6.2 Activate User
- [ ] PUT `/api/admin/users/{userId}/activate`
- [ ] Verify user isActive = true
- [ ] User can login

**Expected:** User activated

#### 6.3 Deactivate User
- [ ] PUT `/api/admin/users/{userId}/deactivate`
- [ ] Verify user isActive = false
- [ ] User cannot login

**Expected:** User deactivated

#### 6.4 Update User Role
- [ ] PUT `/api/admin/users/{userId}/role`
- [ ] Verify role changed
- [ ] User has new permissions

**Test Data:**
```json
"teacher"
```

**Expected:** Role updated

#### 6.5 Get Dashboard
- [ ] GET `/api/admin/dashboard`
- [ ] Verify statistics correct
- [ ] Check counts match database

**Expected:** Dashboard statistics

---

### 7. Anti-Cheat System ✅

#### 7.1 Single Violation
- [ ] Report tab_switch violation
- [ ] Verify violationCount = 1
- [ ] Exam continues

#### 7.2 Multiple Violations
- [ ] Report 2 more violations
- [ ] Verify violationCount = 3
- [ ] Exam auto-submitted
- [ ] isAutoSubmitted = true

#### 7.3 Violation Types
- [ ] Test "tab_switch"
- [ ] Test "fullscreen_exit"
- [ ] Test "copy_paste"
- [ ] All logged correctly

---

### 8. Authorization Tests ✅

#### 8.1 No Token
- [ ] Call protected endpoint without token
- [ ] Verify 401 Unauthorized

#### 8.2 Invalid Token
- [ ] Call with invalid/expired token
- [ ] Verify 401 Unauthorized

#### 8.3 Wrong Role
- [ ] Student calls teacher endpoint
- [ ] Verify 403 Forbidden
- [ ] Teacher calls admin endpoint
- [ ] Verify 403 Forbidden

#### 8.4 Correct Role
- [ ] Each role accesses their endpoints
- [ ] All work correctly

---

### 9. Edge Cases ✅

#### 9.1 Duplicate Registration
- [ ] Register with existing email
- [ ] Verify error message

#### 9.2 Expired OTP
- [ ] Wait 10+ minutes
- [ ] Try to verify OTP
- [ ] Verify error message

#### 9.3 Exam Not Assigned
- [ ] Student tries to start unassigned exam
- [ ] Verify 403 Forbidden

#### 9.4 Exam Already Taken
- [ ] Student tries to start exam twice
- [ ] Verify error message

#### 9.5 Submit Without Starting
- [ ] Try to submit without starting
- [ ] Verify error message

---

### 10. Database Verification ✅

#### 10.1 Check Collections
```bash
mongosh
use ExamPlatformDB
show collections
```
- [ ] users collection exists
- [ ] exams collection exists
- [ ] questions collection exists
- [ ] responses collection exists
- [ ] violations collection exists
- [ ] payments collection exists
- [ ] results collection exists

#### 10.2 Check Data Integrity
```bash
db.users.find().pretty()
db.exams.find().pretty()
db.responses.find().pretty()
```
- [ ] All fields present
- [ ] Relationships correct
- [ ] No orphaned records

---

## 📊 Performance Tests

### Load Testing
- [ ] 10 concurrent registrations
- [ ] 50 concurrent logins
- [ ] 100 concurrent exam submissions
- [ ] Response time < 500ms

### Stress Testing
- [ ] 1000 users in database
- [ ] 100 exams created
- [ ] 1000 questions added
- [ ] System remains stable

---

## 🔐 Security Tests

### Authentication
- [ ] Cannot access protected routes without token
- [ ] Token expires after 24 hours
- [ ] Password is hashed in database
- [ ] OTP expires after 10 minutes

### Authorization
- [ ] Students cannot access teacher routes
- [ ] Teachers cannot access admin routes
- [ ] Users can only see their own data

### Input Validation
- [ ] SQL injection attempts blocked
- [ ] XSS attempts blocked
- [ ] Invalid email format rejected
- [ ] Weak passwords rejected

---

## 📱 Integration Tests

### Email Service
- [ ] OTP emails delivered
- [ ] Email format correct
- [ ] Links work (if any)

### Database
- [ ] All CRUD operations work
- [ ] Transactions are atomic
- [ ] Data persists after restart

### JWT Service
- [ ] Tokens generated correctly
- [ ] Claims included properly
- [ ] Expiry works

---

## ✅ Final Verification

### Complete User Journey
1. [ ] Student registers → receives OTP → verifies → logs in
2. [ ] Teacher registers → creates exam → adds questions → assigns
3. [ ] Student sees exam → starts → answers → submits
4. [ ] Teacher evaluates → publishes results
5. [ ] Student views results
6. [ ] Admin manages users

### All Features Working
- [ ] Authentication ✅
- [ ] Exam Management ✅
- [ ] Student Module ✅
- [ ] Teacher Module ✅
- [ ] Admin Module ✅
- [ ] Payment System ✅
- [ ] Anti-Cheat ✅
- [ ] Results ✅

---

## 🎯 Success Criteria

**Backend is ready if:**
- ✅ All API endpoints return correct responses
- ✅ Database operations work correctly
- ✅ Email OTP is sent and verified
- ✅ JWT authentication works
- ✅ Role-based access is enforced
- ✅ Exam flow works end-to-end
- ✅ Anti-cheat system tracks violations
- ✅ Results are calculated correctly
- ✅ Payment flow works
- ✅ Admin can manage users

---

## 📝 Test Results Template

```
Test Date: _______________
Tester: __________________

Authentication:        [ ] Pass  [ ] Fail
Exam Management:       [ ] Pass  [ ] Fail
Student Module:        [ ] Pass  [ ] Fail
Teacher Module:        [ ] Pass  [ ] Fail
Admin Module:          [ ] Pass  [ ] Fail
Payment System:        [ ] Pass  [ ] Fail
Anti-Cheat:           [ ] Pass  [ ] Fail
Results:              [ ] Pass  [ ] Fail

Overall Status:        [ ] Ready  [ ] Issues Found

Notes:
_________________________________
_________________________________
_________________________________
```

---

## 🐛 Bug Report Template

```
Bug ID: _______________
Date: _________________
Severity: [ ] Critical  [ ] High  [ ] Medium  [ ] Low

Endpoint: _____________
Method: _______________

Steps to Reproduce:
1. ___________________
2. ___________________
3. ___________________

Expected Result:
_________________________________

Actual Result:
_________________________________

Error Message:
_________________________________

Screenshots/Logs:
_________________________________
```

---

## 🎉 Testing Complete!

If all checkboxes are ✅, your backend is:
- **100% Functional**
- **Production Ready**
- **Ready for Frontend Integration**

**Next Steps:**
1. Deploy to staging environment
2. Perform UAT (User Acceptance Testing)
3. Start Phase 2: Angular Frontend
4. Integrate real payment gateway

---

**Happy Testing! 🚀**
