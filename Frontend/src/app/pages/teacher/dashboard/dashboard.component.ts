import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { ApiService } from '../../../services/api.service';
import { AuthService } from '../../../services/auth.service';
import { ThemeService } from '../../../services/theme.service';
import { Exam } from '../../../models/exam.model';

@Component({
  selector: 'app-teacher-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="dashboard-container fade-in">
      <header class="dashboard-header">
        <div class="header-content">
          <h1>Teacher Dashboard</h1>
          <div class="header-actions">
            <button class="btn btn-primary" routerLink="/teacher/create-exam">Create Exam</button>
            <button class="theme-toggle" (click)="toggleTheme()">{{ themeService.isDarkMode() ? '☀️' : '🌙' }}</button>
            <button class="btn btn-secondary" (click)="logout()">Logout</button>
          </div>
        </div>
      </header>

      <div class="dashboard-content">
        <h2>My Exams</h2>
        <div *ngIf="loading" class="loading">Loading...</div>
        <div class="exams-grid">
          <div *ngFor="let exam of exams" class="exam-card card" (click)="viewExam(exam.id!)">
            <h3>{{ exam.title }}</h3>
            <p>{{ exam.description }}</p>
            <div class="exam-meta">
              <span>Duration: {{ exam.duration }} min</span>
              <span>Marks: {{ exam.totalMarks }}</span>
              <span [class.published]="exam.isPublished">{{ exam.isPublished ? 'Published' : 'Draft' }}</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .dashboard-container {
      min-height: 100vh;
      background: var(--background);
    }

    .dashboard-header {
      background: var(--surface);
      padding: 20px;
      margin-bottom: 30px;
    }

    .header-content {
      max-width: 1200px;
      margin: 0 auto;
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    .header-actions {
      display: flex;
      gap: 12px;
      align-items: center;
    }

    .dashboard-content {
      max-width: 1200px;
      margin: 0 auto;
      padding: 20px;
    }

    .exams-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
      gap: 20px;
    }

    .exam-card {
      cursor: pointer;
      transition: transform 0.2s;
    }

    .exam-card:hover {
      transform: translateY(-4px);
    }

    .exam-meta {
      display: flex;
      gap: 16px;
      margin-top: 12px;
      font-size: 14px;
      color: var(--text-secondary);
    }

    .published {
      color: var(--success);
      font-weight: bold;
    }
  `]
})
export class DashboardComponent implements OnInit {
  exams: Exam[] = [];
  loading = false;

  constructor(
    public authService: AuthService,
    public themeService: ThemeService,
    private apiService: ApiService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadExams();
  }

  loadExams(): void {
    this.loading = true;
    this.apiService.getMyExams().subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.exams = response.data;
        }
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  viewExam(examId: string): void {
    this.router.navigate(['/teacher/exam', examId]);
  }

  toggleTheme(): void {
    this.themeService.toggleTheme();
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}

