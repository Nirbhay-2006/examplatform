import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Exam } from '../models/exam';

@Injectable({
    providedIn: 'root'
})
export class ExamService {
    private apiUrl = '/api/exam';

    constructor(private http: HttpClient) { }

    getTeacherExams(): Observable<any> {
        return this.http.get<any>(`${this.apiUrl}/teacher/my-exams`);
    }

    createExam(exam: Exam): Observable<any> {
        return this.http.post<any>(this.apiUrl, exam);
    }

    createExamFromFile(formData: FormData): Observable<any> {
        return this.http.post<any>(`${this.apiUrl}/create-from-file`, formData);
    }

    // Student
    startExam(examId: string): Observable<any> {
        return this.http.post<any>(`${this.apiUrl}/${examId}/start`, {});
    }

    getQuestions(examId: string): Observable<any> {
        return this.http.get<any>(`${this.apiUrl}/${examId}/questions`);
    }

    submitAnswer(examId: string, questionId: string, answer: string): Observable<any> {
        return this.http.post<any>(`${this.apiUrl}/${examId}/answer`, { questionId, answer });
    }

    submitExam(examId: string): Observable<any> {
        return this.http.post<any>(`${this.apiUrl}/${examId}/submit`, {});
    }

    reportViolation(examId: string, violationType: string): Observable<any> {
        return this.http.post<any>(`${this.apiUrl}/${examId}/violation`, { violationType });
    }

    validateSession(examId: string): Observable<any> {
        return this.http.post<any>(`${this.apiUrl}/${examId}/validate-session`, {});
    }

    // Admin
    getAllExams(): Observable<any> {
        return this.http.get<any>(this.apiUrl);
    }
}
