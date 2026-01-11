import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { ThemeService } from '../../../services/theme.service';
import { ResetPasswordRequest } from '../../../models/user.model';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  template: `
    <div class="auth-container fade-in">
      <div class="auth-card">
        <div class="auth-header">
          <h1>Reset Password</h1>
          <p>Enter the code sent to {{ email }} and your new password</p>
        </div>

        <form [formGroup]="form" (ngSubmit)="onSubmit()" class="auth-form">
          <div class="form-group">
            <label class="form-label">Reset Code</label>
            <input
              type="text"
              formControlName="otpCode"
              class="form-input otp-input"
              placeholder="Enter 6-digit code"
              maxlength="6"
              [class.error]="form.get('otpCode')?.invalid && form.get('otpCode')?.touched">
            <div *ngIf="form.get('otpCode')?.invalid && form.get('otpCode')?.touched" class="error-message">
              Code is required (6 digits)
            </div>
          </div>

          <div class="form-group">
            <label class="form-label">New Password</label>
            <input
              type="password"
              formControlName="newPassword"
              class="form-input"
              placeholder="Enter new password"
              [class.error]="form.get('newPassword')?.invalid && form.get('newPassword')?.touched">
            <div *ngIf="form.get('newPassword')?.invalid && form.get('newPassword')?.touched" class="error-message">
              Password must be at least 6 characters
            </div>
          </div>

          <div *ngIf="errorMessage" class="error-message">{{ errorMessage }}</div>
          <div *ngIf="successMessage" class="success-message">{{ successMessage }}</div>

          <button
            type="submit"
            class="btn btn-primary btn-block"
            [disabled]="form.invalid || loading">
            <span *ngIf="!loading">Reset Password</span>
            <span *ngIf="loading">Resetting...</span>
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

    .otp-input {
      text-align: center;
      font-size: 24px;
      letter-spacing: 8px;
      font-weight: bold;
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
export class ResetPasswordComponent implements OnInit {
  form: FormGroup;
  email = '';
  errorMessage = '';
  successMessage = '';
  loading = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute,
    public themeService: ThemeService
  ) {
    this.form = this.fb.group({
      otpCode: ['', [Validators.required, Validators.pattern(/^\d{6}$/)]],
      newPassword: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  ngOnInit(): void {
    this.email = this.route.snapshot.queryParams['email'] || '';
    if (!this.email) {
      this.router.navigate(['/forgot-password']);
    }
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    this.loading = true;
    this.errorMessage = '';
    this.successMessage = '';

    const request: ResetPasswordRequest = {
      email: this.email,
      otpCode: this.form.value.otpCode,
      newPassword: this.form.value.newPassword
    };

    this.authService.resetPassword(request).subscribe({
      next: (response) => {
        if (response.success) {
          this.successMessage = response.message || 'Password reset successfully. You can now login.';
          this.loading = false;
          setTimeout(() => {
            this.router.navigate(['/login']);
          }, 1500);
        } else {
          this.errorMessage = response.message || 'Failed to reset password';
          this.loading = false;
        }
      },
      error: (error) => {
        this.errorMessage = error.message || 'Failed to reset password';
        this.loading = false;
      }
    });
  }

  toggleTheme(): void {
    this.themeService.toggleTheme();
  }
}

