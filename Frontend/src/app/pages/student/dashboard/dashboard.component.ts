import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ApiService } from '../../../services/api.service';
import { AuthService } from '../../../services/auth.service';
import { ThemeService } from '../../../services/theme.service';
import { Exam } from '../../../models/exam.model';

@Component({
  selector: 'app-student-dashboard',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="dashboard-container fade-in">
      <header class="dashboard-header">
        <div class="header-content">
          <h1>Student Dashboard</h1>
          <div class="header-actions">
            <span class="user-name">Welcome, {{ authService.getUser()?.name }}</span>
            <button class="theme-toggle" (click)="toggleTheme()">
              {{ themeService.isDarkMode() ? '☀️' : '🌙' }}
            </button>
            <button class="btn btn-secondary" (click)="logout()">Logout</button>
          </div>
        </div>
      </header>

      <div class="dashboard-content">
        <div class="stats-grid">
          <div class="stat-card">
            <h3>Available Exams</h3>
            <p class="stat-number">{{ availableExams.length }}</p>
          </div>
          <div class="stat-card">
            <h3>Completed Exams</h3>
            <p class="stat-number">{{ completedExams.length }}</p>
          </div>
          <div class="stat-card">
            <h3>Subscription</h3>
            <p class="stat-badge" [class.paid]="authService.hasPaidSubscription()">
              {{ authService.getSubscriptionType() | uppercase }}
            </p>
          </div>
        </div>

        <div class="exams-section">
          <h2>Available Exams</h2>
          <div *ngIf="loading" class="loading">Loading exams...</div>
          <div *ngIf="!loading && availableExams.length === 0" class="empty-state">
            <p>No exams available at the moment.</p>
          </div>
          <div class="exams-grid">
            <div *ngFor="let exam of availableExams" class="exam-card card">
              <div class="exam-header">
                <h3>{{ exam.title }}</h3>
                <span *ngIf="exam.isPaid" class="badge paid-badge">PAID</span>
                <span *ngIf="!exam.isPaid" class="badge free-badge">FREE</span>
              </div>
              <p class="exam-description">{{ exam.description }}</p>
              <div class="exam-details">
                <span>Duration: {{ exam.duration }} min</span>
                <span>Marks: {{ exam.totalMarks }}</span>
                <span>Passing: {{ exam.passingMarks }}</span>
              </div>
              <div class="exam-actions">
                <button 
                  class="btn btn-primary" 
                  (click)="startExam(exam.id!)"
                  [disabled]="exam.isPaid && !authService.hasPaidSubscription()">
                  {{ exam.isPaid && !authService.hasPaidSubscription() ? 'Upgrade Required' : 'Start Exam' }}
                </button>
              </div>
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
      padding-bottom: 40px;
    }

    .dashboard-header {
      background: var(--surface);
      padding: 24px 32px;
      box-shadow: 0 4px 6px -1px var(--shadow);
      margin-bottom: 32px;
      border-bottom: 1px solid var(--border-color);
    }

    .header-content {
      max-width: 1400px;
      margin: 0 auto;
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    .header-content h1 {
      color: var(--text-primary);
      margin: 0;
      font-size: 28px;
      font-weight: 700;
      background: var(--primary-gradient);
      -webkit-background-clip: text;
      -webkit-text-fill-color: transparent;
      background-clip: text;
    }

    .header-actions {
      display: flex;
      align-items: center;
      gap: 16px;
    }

    .user-name {
      color: var(--text-secondary);
      font-weight: 500;
      padding: 8px 16px;
      background: var(--surface);
      border-radius: 8px;
    }

    .theme-toggle {
      width: 44px;
      height: 44px;
      border-radius: 50%;
      border: 2px solid var(--border-color);
      background: var(--surface);
      cursor: pointer;
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 20px;
      transition: all 0.3s ease;
    }

    .theme-toggle:hover {
      transform: scale(1.1);
      border-color: var(--primary-color);
    }

    .dashboard-content {
      max-width: 1400px;
      margin: 0 auto;
      padding: 0 32px;
    }

    .stats-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));
      gap: 24px;
      margin-bottom: 40px;
    }

    .stat-card {
      background: var(--surface);
      padding: 28px;
      border-radius: 16px;
      box-shadow: 0 4px 6px -1px var(--shadow);
      border: 1px solid var(--border-color);
      transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
      position: relative;
      overflow: hidden;
    }

    .stat-card::before {
      content: '';
      position: absolute;
      top: 0;
      left: 0;
      right: 0;
      height: 4px;
      background: var(--primary-gradient);
      transform: scaleX(0);
      transition: transform 0.3s ease;
    }

    .stat-card:hover {
      transform: translateY(-4px);
      box-shadow: 0 12px 24px -4px var(--shadow-lg);
    }

    .stat-card:hover::before {
      transform: scaleX(1);
    }

    .stat-card h3 {
      color: var(--text-secondary);
      font-size: 14px;
      margin-bottom: 12px;
      font-weight: 600;
      text-transform: uppercase;
      letter-spacing: 0.5px;
    }

    .stat-number {
      font-size: 40px;
      font-weight: 800;
      background: var(--primary-gradient);
      -webkit-background-clip: text;
      -webkit-text-fill-color: transparent;
      background-clip: text;
      line-height: 1;
    }

    .stat-badge {
      padding: 8px 16px;
      border-radius: 20px;
      font-size: 13px;
      font-weight: 600;
      background: var(--border-color);
      color: var(--text-primary);
      display: inline-block;
    }

    .stat-badge.paid {
      background: var(--success);
      color: white;
      box-shadow: 0 4px 12px rgba(16, 185, 129, 0.3);
    }

    .exams-section {
      margin-top: 48px;
    }

    .exams-section h2 {
      color: var(--text-primary);
      margin-bottom: 24px;
      font-size: 24px;
      font-weight: 700;
    }

    .exams-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
      gap: 24px;
    }

    .exam-card {
      display: flex;
      flex-direction: column;
      animation: fadeInUp 0.6s ease-out;
      animation-fill-mode: both;
    }

    .exam-card:nth-child(1) { animation-delay: 0.1s; }
    .exam-card:nth-child(2) { animation-delay: 0.2s; }
    .exam-card:nth-child(3) { animation-delay: 0.3s; }
    .exam-card:nth-child(4) { animation-delay: 0.4s; }

    .exam-header {
      display: flex;
      justify-content: space-between;
      align-items: start;
      margin-bottom: 16px;
      gap: 12px;
    }

    .exam-header h3 {
      color: var(--text-primary);
      margin: 0;
      flex: 1;
      font-size: 20px;
      font-weight: 700;
      line-height: 1.3;
    }

    .badge {
      padding: 6px 12px;
      border-radius: 12px;
      font-size: 11px;
      font-weight: 700;
      text-transform: uppercase;
      letter-spacing: 0.5px;
      white-space: nowrap;
    }

    .paid-badge {
      background: linear-gradient(135deg, #f59e0b 0%, #d97706 100%);
      color: white;
      box-shadow: 0 2px 8px rgba(245, 158, 11, 0.3);
    }

    .free-badge {
      background: linear-gradient(135deg, #10b981 0%, #059669 100%);
      color: white;
      box-shadow: 0 2px 8px rgba(16, 185, 129, 0.3);
    }

    .exam-description {
      color: var(--text-secondary);
      margin-bottom: 20px;
      flex: 1;
      line-height: 1.6;
      font-size: 15px;
    }

    .exam-details {
      display: flex;
      flex-wrap: wrap;
      gap: 16px;
      margin-bottom: 20px;
      font-size: 14px;
      color: var(--text-secondary);
      padding: 16px;
      background: var(--background);
      border-radius: 12px;
    }

    .exam-details span {
      display: flex;
      align-items: center;
      gap: 6px;
    }

    .exam-actions {
      margin-top: auto;
    }

    .exam-actions .btn {
      width: 100%;
      justify-content: center;
    }

    .loading, .empty-state {
      text-align: center;
      padding: 60px 20px;
      color: var(--text-secondary);
    }

    .empty-state {
      background: var(--surface);
      border-radius: 16px;
      border: 2px dashed var(--border-color);
    }

    .empty-state p {
      font-size: 16px;
      margin: 0;
    }

    @media (max-width: 768px) {
      .dashboard-header {
        padding: 16px 20px;
      }

      .header-content {
        flex-direction: column;
        gap: 16px;
        align-items: flex-start;
      }

      .header-actions {
        width: 100%;
        justify-content: space-between;
      }

      .dashboard-content {
        padding: 0 20px;
      }

      .stats-grid {
        grid-template-columns: 1fr;
      }

      .exams-grid {
        grid-template-columns: 1fr;
      }
    }
  `]
})
export class DashboardComponent implements OnInit {
  availableExams: Exam[] = [];
  completedExams: Exam[] = [];
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
    this.apiService.getStudentExams().subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.availableExams = response.data;
        }
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading exams:', error);
        this.loading = false;
      }
    });
  }

  startExam(examId: string): void {
    this.router.navigate(['/student/exam', examId]);
  }

  toggleTheme(): void {
    this.themeService.toggleTheme();
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}

