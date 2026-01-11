import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ApiService } from '../../../services/api.service';

@Component({
  selector: 'app-exam-details',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="exam-details-container fade-in">
      <header>
        <h1>Exam Details</h1>
        <button class="btn btn-secondary" routerLink="/teacher/dashboard">Back</button>
      </header>
      <div class="card">
        <h2>{{ exam?.title }}</h2>
        <p>{{ exam?.description }}</p>
        <div class="exam-info">
          <p>Duration: {{ exam?.duration }} minutes</p>
          <p>Total Marks: {{ exam?.totalMarks }}</p>
          <p>Status: {{ exam?.isPublished ? 'Published' : 'Draft' }}</p>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .exam-details-container {
      max-width: 800px;
      margin: 0 auto;
      padding: 20px;
    }

    header {
      display: flex;
      justify-content: space-between;
      margin-bottom: 20px;
    }
  `]
})
export class ExamDetailsComponent implements OnInit {
  exam: any = null;

  constructor(
    private route: ActivatedRoute,
    private apiService: ApiService
  ) {}

  ngOnInit(): void {
    const examId = this.route.snapshot.paramMap.get('id');
    if (examId) {
      this.apiService.getExamById(examId).subscribe({
        next: (response) => {
          if (response.success && response.data) {
            this.exam = response.data;
          }
        }
      });
    }
  }
}

