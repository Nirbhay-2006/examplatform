import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class StudentService {
  private apiUrl = '/api/student';
  private examUrl = '/api/exam';

  constructor(private http: HttpClient) { }

  getProfile(): Observable<any> {
    return this.http.get(`${this.apiUrl}/profile`);
  }

  getDashboardStats(): Observable<any> {
    return this.http.get(`${this.apiUrl}/stats`);
  }

  getAllExams(): Observable<any[]> {
    return this.http.get<any[]>(`${this.examUrl}`);
  }

  getMyExams(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/my-exams`);
  }

  getExam(id: string): Observable<any> {
    return this.http.get<any>(`${this.examUrl}/${id}`);
  }
}
