import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ApiService } from '../../../services/api.service';
import { AuthService } from '../../../services/auth.service';
import { ThemeService } from '../../../services/theme.service';

@Component({
  selector: 'app-results',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="results-container fade-in">
      <header class="results-header">
        <h1>My Results</h1>
        <button class="btn btn-secondary" routerLink="/student/dashboard">Back to Dashboard</button>
      </header>

      <div class="results-content">
        <div *ngIf="loading" class="loading">Loading results...</div>
        <div *ngIf="!loading && results.length === 0" class="empty-state">
          <p>No results available yet.</p>
        </div>
        <div *ngFor="let result of results" class="result-card card">
          <h3>{{ result.examTitle }}</h3>
          <div class="result-details">
            <div class="result-item">
              <span class="label">Score:</span>
              <span class="value">{{ result.obtainedMarks }}/{{ result.totalMarks }}</span>
            </div>
            <div class="result-item">
              <span class="label">Percentage:</span>
              <span class="value">{{ result.percentage }}%</span>
            </div>
            <div class="result-item">
              <span class="label">Status:</span>
              <span class="value" [class.pass]="result.isPass" [class.fail]="!result.isPass">
                {{ result.isPass ? 'PASS' : 'FAIL' }}
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .results-container {
      min-height: 100vh;
      background: var(--background);
      padding: 20px;
    }

    .results-header {
      max-width: 1200px;
      margin: 0 auto 30px;
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    .results-content {
      max-width: 1200px;
      margin: 0 auto;
    }

    .result-card {
      margin-bottom: 20px;
    }

    .result-details {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
      gap: 20px;
      margin-top: 16px;
    }

    .result-item {
      display: flex;
      flex-direction: column;
      gap: 4px;
    }

    .label {
      color: var(--text-secondary);
      font-size: 14px;
    }

    .value {
      font-size: 20px;
      font-weight: bold;
      color: var(--text-primary);
    }

    .value.pass {
      color: var(--success);
    }

    .value.fail {
      color: var(--error);
    }
  `]
})
export class ResultsComponent implements OnInit {
  results: any[] = [];
  loading = false;

  constructor(
    private apiService: ApiService,
    public authService: AuthService,
    public themeService: ThemeService
  ) {}

  ngOnInit(): void {
    this.loadResults();
  }

  loadResults(): void {
    this.loading = true;
    // TODO: Implement getResults API call
    setTimeout(() => {
      this.results = [];
      this.loading = false;
    }, 1000);
  }
}

