import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { ThemeService } from '../../../services/theme.service';
import { ForgotPasswordRequest } from '../../../models/user.model';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  template: `
    <div class="auth-container fade-in">
      <div class="auth-card">
        <div class="auth-header">
          <h1>Forgot Password</h1>
          <p>Enter your email to receive a reset code</p>
        </div>

        <form [formGroup]="form" (ngSubmit)="onSubmit()" class="auth-form">
          <div class="form-group">
            <label class="form-label">Email</label>
            <input
              type="email"
              formControlName="email"
              class="form-input"
              placeholder="Enter your email"
              [class.error]="form.get('email')?.invalid && form.get('email')?.touched">
            <div *ngIf="form.get('email')?.invalid && form.get('email')?.touched" class="error-message">
              Valid email is required
            </div>
          </div>

          <div *ngIf="errorMessage" class="error-message">{{ errorMessage }}</div>
          <div *ngIf="successMessage" class="success-message">{{ successMessage }}</div>

          <button
            type="submit"
            class="btn btn-primary btn-block"
            [disabled]="form.invalid || loading">
            <span *ngIf="!loading">Send Reset Code</span>
            <span *ngIf="loading">Sending...</span>
          </button>
        </form>

        <div class="auth-footer">
          <p><a routerLink="/login">Back to Login</a></p>
        </div>

        <button class="theme-toggle" (click)="toggleTheme()">
          {{ themeService.isDarkMode() ? '☀️' : '🌙' }}
        </button>
      </div>
    </div>
  `,
  styles: [`
    .auth-container {
      min-height: 100vh;
      display: flex;
      align-items: center;
      justify-content: center;
      background: var(--background);
      padding: 20px;
    }

    .auth-card {
      background: var(--surface);
      border-radius: 12px;
      padding: 40px;
      width: 100%;
      max-width: 400px;
      box-shadow: 0 4px 20px var(--shadow);
      position: relative;
    }

    .auth-header {
      text-align: center;
      margin-bottom: 30px;
    }

    .auth-header h1 {
      color: var(--text-primary);
      margin-bottom: 8px;
      font-size: 28px;
    }

    .auth-header p {
      color: var(--text-secondary);
      font-size: 14px;
    }

    .auth-form {
      margin-bottom: 20px;
    }

    .form-input.error {
      border-color: var(--error);
    }

    .error-message {
      color: var(--error);
      font-size: 12px;
      margin-top: 4px;
    }

    .success-message {
      color: var(--success);
      font-size: 14px;
      margin-top: 10px;
      padding: 10px;
      background: rgba(56, 142, 60, 0.1);
      border-radius: 4px;
    }

    .btn-block {
      width: 100%;
      margin-top: 20px;
    }

    .auth-footer {
      text-align: center;
      margin-top: 20px;
    }

    .auth-footer a {
      color: var(--primary-color);
      text-decoration: none;
    }

    .theme-toggle {
      position: absolute;
      top: 20px;
      right: 20px;
      background: transparent;
      border: none;
      font-size: 24px;
      cursor: pointer;
      padding: 8px;
      border-radius: 50%;
    }
  `]
})
export class ForgotPasswordComponent {
  form: FormGroup;
  errorMessage = '';
  successMessage = '';
  loading = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    public themeService: ThemeService
  ) {
    this.form = this.fb.group({
      email: ['', [Validators.required, Validators.email]]
    });
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    this.loading = true;
    this.errorMessage = '';
    this.successMessage = '';

    const request: ForgotPasswordRequest = this.form.value;

    this.authService.forgotPassword(request).subscribe({
      next: (response) => {
        this.successMessage = response.message || 'If an account exists, a reset code has been sent.';
        this.loading = false;
        setTimeout(() => {
          this.router.navigate(['/reset-password'], {
            queryParams: { email: request.email }
          });
        }, 1500);
      },
      error: (error) => {
        this.errorMessage = error.message || 'An error occurred';
        this.loading = false;
      }
    });
  }

  toggleTheme(): void {
    this.themeService.toggleTheme();
  }
}

