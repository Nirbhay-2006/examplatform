import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { ApiService } from '../../../services/api.service';
import { AuthService } from '../../../services/auth.service';
import { ThemeService } from '../../../services/theme.service';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="dashboard-container fade-in">
      <header class="dashboard-header">
        <h1>Admin Dashboard</h1>
        <div class="header-actions">
          <button class="btn btn-secondary" routerLink="/admin/users">Manage Users</button>
          <button class="btn btn-secondary" (click)="logout()">Logout</button>
        </div>
      </header>
      <div class="dashboard-content">
        <div class="stats-grid">
          <div class="stat-card" *ngIf="stats">
            <h3>Total Users</h3>
            <p class="stat-number">{{ stats.totalUsers }}</p>
          </div>
          <div class="stat-card" *ngIf="stats">
            <h3>Students</h3>
            <p class="stat-number">{{ stats.totalStudents }}</p>
          </div>
          <div class="stat-card" *ngIf="stats">
            <h3>Teachers</h3>
            <p class="stat-number">{{ stats.totalTeachers }}</p>
          </div>
          <div class="stat-card" *ngIf="stats">
            <h3>Total Exams</h3>
            <p class="stat-number">{{ stats.totalExams }}</p>
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
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    .dashboard-content {
      max-width: 1200px;
      margin: 0 auto;
      padding: 20px;
    }

    .stats-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
      gap: 20px;
    }

    .stat-card {
      background: var(--surface);
      padding: 24px;
      border-radius: 8px;
      box-shadow: 0 2px 8px var(--shadow);
    }

    .stat-number {
      font-size: 32px;
      font-weight: bold;
      color: var(--primary-color);
    }
  `]
})
export class DashboardComponent implements OnInit {
  stats: any = null;

  constructor(
    public authService: AuthService,
    public themeService: ThemeService,
    private apiService: ApiService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadStats();
  }

  loadStats(): void {
    this.apiService.getAdminDashboard().subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.stats = response.data;
        }
      }
    });
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}

