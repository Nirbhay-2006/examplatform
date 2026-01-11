import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { ApiResponse } from '../models/api-response.model';
import { Exam, CreateExamRequest, CreateQuestionRequest, SubmitAnswerRequest, ViolationRequest, Question, Response } from '../models/exam.model';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private readonly apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  // Exam endpoints
  createExam(request: CreateExamRequest): Observable<ApiResponse<Exam>> {
    return this.http.post<ApiResponse<Exam>>(`${this.apiUrl}/exam`, request);
  }

  getMyExams(): Observable<ApiResponse<Exam[]>> {
    return this.http.get<ApiResponse<Exam[]>>(`${this.apiUrl}/exam/teacher/my-exams`);
  }

  getStudentExams(): Observable<ApiResponse<Exam[]>> {
    return this.http.get<ApiResponse<Exam[]>>(`${this.apiUrl}/exam/student/my-exams`);
  }

  getAllExams(): Observable<ApiResponse<Exam[]>> {
    return this.http.get<ApiResponse<Exam[]>>(`${this.apiUrl}/exam`);
  }

  getExamById(examId: string): Observable<ApiResponse<Exam>> {
    return this.http.get<ApiResponse<Exam>>(`${this.apiUrl}/exam/${examId}`);
  }

  assignExam(examId: string, studentIds: string[]): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.apiUrl}/exam/${examId}/assign`, studentIds);
  }

  startExam(examId: string): Observable<ApiResponse<Response>> {
    return this.http.post<ApiResponse<Response>>(`${this.apiUrl}/exam/${examId}/start`, {});
  }

  getQuestions(examId: string): Observable<ApiResponse<Question[]>> {
    return this.http.get<ApiResponse<Question[]>>(`${this.apiUrl}/exam/${examId}/questions`);
  }

  submitAnswer(examId: string, request: SubmitAnswerRequest): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.apiUrl}/exam/${examId}/answer`, request);
  }

  submitExam(examId: string): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.apiUrl}/exam/${examId}/submit`, {});
  }

  reportViolation(examId: string, request: ViolationRequest): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.apiUrl}/exam/${examId}/violation`, request);
  }

  addQuestion(examId: string, request: CreateQuestionRequest): Observable<ApiResponse<Question>> {
    return this.http.post<ApiResponse<Question>>(`${this.apiUrl}/exam/${examId}/questions`, request);
  }

  uploadQuestions(examId: string, file: File, maxQuestions?: number): Observable<ApiResponse<any>> {
    const formData = new FormData();
    formData.append('file', file);
    if (maxQuestions && maxQuestions > 0) {
      formData.append('maxQuestions', maxQuestions.toString());
    }
    return this.http.post<ApiResponse<any>>(`${this.apiUrl}/exam/${examId}/upload-questions`, formData);
  }

  validateSession(examId: string, fingerprint?: string): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.apiUrl}/exam/${examId}/validate-session`, {
      browserFingerprint: fingerprint,
      userAgent: navigator.userAgent,
      timestamp: new Date().toISOString()
    });
  }

  // Payment endpoints
  createOrder(amount: number): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.apiUrl}/payment/create-order`, amount);
  }

  verifyPayment(orderId: string, paymentId: string): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.apiUrl}/payment/verify`, { orderId, paymentId });
  }

  getMyPayments(): Observable<ApiResponse<any[]>> {
    return this.http.get<ApiResponse<any[]>>(`${this.apiUrl}/payment/my-payments`);
  }

  // Admin endpoints
  getAllUsers(): Observable<ApiResponse<any[]>> {
    return this.http.get<ApiResponse<any[]>>(`${this.apiUrl}/admin/users`);
  }

  activateUser(userId: string): Observable<ApiResponse<any>> {
    return this.http.put<ApiResponse<any>>(`${this.apiUrl}/admin/users/${userId}/activate`, {});
  }

  deactivateUser(userId: string): Observable<ApiResponse<any>> {
    return this.http.put<ApiResponse<any>>(`${this.apiUrl}/admin/users/${userId}/deactivate`, {});
  }

  updateUserRole(userId: string, role: string): Observable<ApiResponse<any>> {
    return this.http.put<ApiResponse<any>>(`${this.apiUrl}/admin/users/${userId}/role`, role);
  }

  getAdminDashboard(): Observable<ApiResponse<any>> {
    return this.http.get<ApiResponse<any>>(`${this.apiUrl}/admin/dashboard`);
  }
}

