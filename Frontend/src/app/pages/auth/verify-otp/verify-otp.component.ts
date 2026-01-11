import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { ThemeService } from '../../../services/theme.service';
import { VerifyOtpRequest } from '../../../models/user.model';

@Component({
  selector: 'app-verify-otp',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  template: `
    <div class="auth-container fade-in">
      <div class="auth-card">
        <div class="auth-header">
          <h1>Verify Email</h1>
          <p>Enter the OTP sent to {{ email }}</p>
        </div>

        <form [formGroup]="otpForm" (ngSubmit)="onSubmit()" class="auth-form">
          <div class="form-group">
            <label class="form-label">OTP Code</label>
            <input 
              type="text" 
              formControlName="otpCode" 
              class="form-input otp-input"
              placeholder="Enter 6-digit OTP"
              maxlength="6"
              [class.error]="otpForm.get('otpCode')?.invalid && otpForm.get('otpCode')?.touched">
            <div *ngIf="otpForm.get('otpCode')?.invalid && otpForm.get('otpCode')?.touched" class="error-message">
              OTP code is required (6 digits)
            </div>
          </div>

          <div *ngIf="errorMessage" class="error-message">{{ errorMessage }}</div>
          <div *ngIf="successMessage" class="success-message">{{ successMessage }}</div>

          <button 
            type="submit" 
            class="btn btn-primary btn-block"
            [disabled]="otpForm.invalid || loading">
            <span *ngIf="!loading">Verify OTP</span>
            <span *ngIf="loading">Verifying...</span>
          </button>
        </form>

        <div class="auth-footer">
          <p>Didn't receive OTP? <a (click)="resendOtp()" style="cursor: pointer;">Resend</a></p>
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
export class VerifyOtpComponent implements OnInit {
  otpForm: FormGroup;
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
    this.otpForm = this.fb.group({
      otpCode: ['', [Validators.required, Validators.pattern(/^\d{6}$/)]]
    });
  }

  ngOnInit(): void {
    this.email = this.route.snapshot.queryParams['email'] || '';
    if (!this.email) {
      this.router.navigate(['/register']);
    }
  }

  onSubmit(): void {
    if (this.otpForm.valid) {
      this.loading = true;
      this.errorMessage = '';
      this.successMessage = '';

      const request: VerifyOtpRequest = {
        email: this.email,
        otpCode: this.otpForm.value.otpCode
      };
      
      this.authService.verifyOtp(request).subscribe({
        next: (response) => {
          if (response.success) {
            this.successMessage = response.message || 'Email verified successfully!';
            setTimeout(() => {
              this.router.navigate(['/login']);
            }, 2000);
          } else {
            this.errorMessage = response.message || 'Invalid OTP';
            this.loading = false;
          }
        },
        error: (error) => {
          this.errorMessage = error.message || 'An error occurred during verification';
          this.loading = false;
        }
      });
    }
  }

  resendOtp(): void {
    this.loading = true;
    this.errorMessage = '';
    
    this.authService.resendOtp(this.email).subscribe({
      next: (response) => {
        if (response.success) {
          this.successMessage = 'OTP resent successfully!';
        } else {
          this.errorMessage = response.message || 'Failed to resend OTP';
        }
        this.loading = false;
      },
      error: (error) => {
        this.errorMessage = error.message || 'Failed to resend OTP';
        this.loading = false;
      }
    });
  }

  toggleTheme(): void {
    this.themeService.toggleTheme();
  }
}

