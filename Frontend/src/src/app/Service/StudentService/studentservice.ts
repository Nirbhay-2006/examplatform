import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class Studentservice {
  constructor(private http: HttpClient) {}

  GetPublishedCourses(){
    return this.http.get('https://localhost:44385/api/Student/published-courses');
  }
}
