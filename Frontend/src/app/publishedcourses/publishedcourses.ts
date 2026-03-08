import { Component, OnInit } from '@angular/core';
import { Studentservice } from '../Service/StudentService/studentservice';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-publishedcourses',
  imports: [CommonModule],
  templateUrl: './publishedcourses.html',
  styleUrl: './publishedcourses.css',
})
export class Publishedcourses implements OnInit {
  constructor(private service: Studentservice) {}
  courses: any[] = [];

  ngOnInit(): void {
    this.loadcourse();
  }

  loadcourse(){
    this.service.GetPublishedCourses().subscribe({
      next: (res: any) => {
        this.courses = res;
      },
      error: (err: any) => {
        console.log(err);
      },
    });
  }
  
  subscribe(courseId: number) {
    console.log(courseId);
  }

}
