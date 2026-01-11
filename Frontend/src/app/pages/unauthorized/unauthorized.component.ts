import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-unauthorized',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="unauthorized-container fade-in">
      <div class="unauthorized-card card">
        <h1>403</h1>
        <h2>Unauthorized Access</h2>
        <p>You don't have permission to access this page.</p>
        <button class="btn btn-primary" routerLink="/login">Go to Login</button>
      </div>
    </div>
  `,
  styles: [`
    .unauthorized-container {
      min-height: 100vh;
      display: flex;
      align-items: center;
      justify-content: center;
      padding: 20px;
    }

    .unauthorized-card {
      text-align: center;
      max-width: 400px;
    }

    h1 {
      font-size: 72px;
      color: var(--error);
      margin: 0;
    }
  `]
})
export class UnauthorizedComponent {}

