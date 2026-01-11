import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { ThemeService } from '../../../services/theme.service';
import { LoginRequest } from '../../../models/user.model';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  template: `
    <div class="auth-container fade-in">
      <!-- Premium Gradient Background -->
      <div class="auth-background">
        <div class="gradient-orb orb-1"></div>
        <div class="gradient-orb orb-2"></div>
        <div class="gradient-orb orb-3"></div>
      </div>

      <!-- Glassmorphism Login Card -->
      <div class="auth-card card-glass">
        <!-- Security Indicators -->
        <div class="security-indicators">
          <div class="security-badge">
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z"/>
            </svg>
            <span>Secure</span>
          </div>
          <div class="security-badge">
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <rect x="3" y="11" width="18" height="11" rx="2" ry="2"/>
              <path d="M7 11V7a5 5 0 0 1 10 0v4"/>
            </svg>
            <span>Encrypted</span>
          </div>
        </div>

        <div class="auth-header">
          <div class="logo-icon">
            <svg width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="M12 2L2 7l10 5 10-5-10-5z"/>
              <path d="M2 17l10 5 10-5"/>
              <path d="M2 12l10 5 10-5"/>
            </svg>
          </div>
          <h1>Welcome Back</h1>
          <p>Sign in to continue to your exam portal</p>
        </div>

        <form [formGroup]="loginForm" (ngSubmit)="onSubmit()" class="auth-form">
          <!-- Floating Label Email Input -->
          <div class="input-group">
            <input 
              type="email" 
              formControlName="email" 
              id="email"
              class="form-input floating-input"
              placeholder=" "
              [class.error]="loginForm.get('email')?.invalid && loginForm.get('email')?.touched"
              autocomplete="email">
            <label for="email" class="floating-label">
              <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <path d="M4 4h16c1.1 0 2 .9 2 2v12c0 1.1-.9 2-2 2H4c-1.1 0-2-.9-2-2V6c0-1.1.9-2 2-2z"/>
                <polyline points="22,6 12,13 2,6"/>
              </svg>
              Email Address
            </label>
            <div *ngIf="loginForm.get('email')?.invalid && loginForm.get('email')?.touched" class="error-message">
              Valid email is required
            </div>
          </div>

          <!-- Floating Label Password Input with Toggle -->
          <div class="input-group">
            <input 
              [type]="showPassword ? 'text' : 'password'"
              formControlName="password" 
              id="password"
              class="form-input floating-input"
              placeholder=" "
              [class.error]="loginForm.get('password')?.invalid && loginForm.get('password')?.touched"
              autocomplete="current-password">
            <label for="password" class="floating-label">
              <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <rect x="3" y="11" width="18" height="11" rx="2" ry="2"/>
                <path d="M7 11V7a5 5 0 0 1 10 0v4"/>
              </svg>
              Password
            </label>
            <button 
              type="button"
              class="password-toggle"
              (click)="showPassword = !showPassword"
              tabindex="-1">
              <svg *ngIf="!showPassword" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"/>
                <circle cx="12" cy="12" r="3"/>
              </svg>
              <svg *ngIf="showPassword" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19m-6.72-1.07a3 3 0 1 1-4.24-4.24"/>
                <line x1="1" y1="1" x2="23" y2="23"/>
              </svg>
            </button>
            <div *ngIf="loginForm.get('password')?.invalid && loginForm.get('password')?.touched" class="error-message">
              Password is required
            </div>
          </div>

          <!-- Remember Me & Forgot Password -->
          <div class="form-options">
            <label class="checkbox-label">
              <input type="checkbox" formControlName="rememberMe" class="checkbox-input">
              <span class="checkbox-custom"></span>
              <span class="checkbox-text">Remember me</span>
            </label>
            <a routerLink="/forgot-password" class="forgot-link">Forgot password?</a>
          </div>

          <div *ngIf="errorMessage" class="error-message global-error">{{ errorMessage }}</div>

          <button 
            type="submit" 
            class="btn btn-primary btn-block btn-premium"
            [disabled]="loginForm.invalid || loading">
            <span *ngIf="!loading">
              <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <path d="M15 3h4a2 2 0 0 1 2 2v14a2 2 0 0 1-2 2h-4"/>
                <polyline points="10 17 15 12 10 7"/>
                <line x1="15" y1="12" x2="3" y2="12"/>
              </svg>
              Sign In
            </span>
            <span *ngIf="loading" class="loading-spinner">
              <div class="spinner-small"></div>
              Signing In...
            </span>
          </button>
        </form>

        <div class="auth-divider">
          <span>or</span>
        </div>

        <div class="auth-footer">
          <p>Don't have an account? <a routerLink="/register" class="link-primary">Create Account</a></p>
        </div>

        <button class="theme-toggle" (click)="toggleTheme()" aria-label="Toggle theme">
          <svg *ngIf="themeService.isDarkMode()" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <circle cx="12" cy="12" r="5"/>
            <line x1="12" y1="1" x2="12" y2="3"/>
            <line x1="12" y1="21" x2="12" y2="23"/>
            <line x1="4.22" y1="4.22" x2="5.64" y2="5.64"/>
            <line x1="18.36" y1="18.36" x2="19.78" y2="19.78"/>
            <line x1="1" y1="12" x2="3" y2="12"/>
            <line x1="21" y1="12" x2="23" y2="12"/>
            <line x1="4.22" y1="19.78" x2="5.64" y2="18.36"/>
            <line x1="18.36" y1="5.64" x2="19.78" y2="4.22"/>
          </svg>
          <svg *ngIf="!themeService.isDarkMode()" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M21 12.79A9 9 0 1 1 11.21 3 7 7 0 0 0 21 12.79z"/>
          </svg>
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
      background: linear-gradient(135deg, #0f172a 0%, #1e293b 50%, #0f172a 100%);
      padding: 24px;
      position: relative;
      overflow: hidden;
    }

    .auth-background {
      position: absolute;
      top: 0;
      left: 0;
      right: 0;
      bottom: 0;
      overflow: hidden;
      z-index: 0;
    }

    .gradient-orb {
      position: absolute;
      border-radius: 50%;
      filter: blur(80px);
      opacity: 0.3;
      animation: float 20s ease-in-out infinite;
    }

    .orb-1 {
      width: 400px;
      height: 400px;
      background: linear-gradient(135deg, #1e40af 0%, #3b82f6 100%);
      top: -200px;
      left: -200px;
      animation-delay: 0s;
    }

    .orb-2 {
      width: 300px;
      height: 300px;
      background: linear-gradient(135deg, #059669 0%, #10b981 100%);
      bottom: -150px;
      right: -150px;
      animation-delay: 5s;
    }

    .orb-3 {
      width: 250px;
      height: 250px;
      background: linear-gradient(135deg, #3b82f6 0%, #8b5cf6 100%);
      top: 50%;
      right: 10%;
      animation-delay: 10s;
    }

    .auth-card {
      width: 100%;
      max-width: 440px;
      padding: 48px;
      position: relative;
      z-index: 1;
    }

    .security-indicators {
      display: flex;
      gap: 12px;
      justify-content: center;
      margin-bottom: 24px;
    }

    .security-badge {
      display: flex;
      align-items: center;
      gap: 6px;
      padding: 6px 12px;
      background: rgba(16, 185, 129, 0.1);
      border: 1px solid rgba(16, 185, 129, 0.2);
      border-radius: 20px;
      font-size: 11px;
      font-weight: 600;
      color: var(--emerald-green-light);
      text-transform: uppercase;
      letter-spacing: 0.5px;
    }

    .security-badge svg {
      width: 14px;
      height: 14px;
    }

    .auth-header {
      text-align: center;
      margin-bottom: 40px;
    }

    .logo-icon {
      width: 64px;
      height: 64px;
      margin: 0 auto 20px;
      background: var(--royal-blue-gradient);
      border-radius: 16px;
      display: flex;
      align-items: center;
      justify-content: center;
      color: white;
      box-shadow: 0 8px 24px rgba(30, 64, 175, 0.3);
    }

    .auth-header h1 {
      color: var(--text-primary);
      margin-bottom: 8px;
      font-size: 32px;
      font-weight: 700;
      letter-spacing: -0.02em;
      font-family: 'Poppins', sans-serif;
    }

    .auth-header p {
      color: var(--text-secondary);
      font-size: 15px;
      font-weight: 400;
    }

    .auth-form {
      margin-bottom: 24px;
    }

    .input-group {
      position: relative;
      margin-bottom: 24px;
    }

    .floating-input {
      width: 100%;
      padding: 18px 16px 10px 48px;
      border: 2px solid var(--border-color);
      border-radius: 12px;
      background: var(--surface);
      color: var(--text-primary);
      font-size: 15px;
      transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
      font-family: 'Inter', sans-serif;
    }

    .floating-input:focus {
      outline: none;
      border-color: var(--royal-blue-light);
      box-shadow: 0 0 0 4px rgba(59, 130, 246, 0.1);
      background: var(--surface-hover);
    }

    .floating-input.error {
      border-color: var(--error);
    }

    .floating-label {
      position: absolute;
      left: 48px;
      top: 18px;
      color: var(--text-secondary);
      pointer-events: none;
      transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
      font-size: 15px;
      display: flex;
      align-items: center;
      gap: 8px;
      font-weight: 500;
    }

    .floating-label svg {
      width: 18px;
      height: 18px;
      opacity: 0.6;
    }

    .floating-input:focus + .floating-label,
    .floating-input:not(:placeholder-shown) + .floating-label {
      transform: translateY(-24px) scale(0.85);
      color: var(--royal-blue-light);
      left: 16px;
    }

    .floating-input:focus + .floating-label svg,
    .floating-input:not(:placeholder-shown) + .floating-label svg {
      opacity: 1;
      color: var(--royal-blue-light);
    }

    .password-toggle {
      position: absolute;
      right: 16px;
      top: 50%;
      transform: translateY(-50%);
      background: transparent;
      border: none;
      color: var(--text-secondary);
      cursor: pointer;
      padding: 8px;
      display: flex;
      align-items: center;
      transition: color 0.2s;
    }

    .password-toggle:hover {
      color: var(--royal-blue-light);
    }

    .form-options {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 24px;
    }

    .checkbox-label {
      display: flex;
      align-items: center;
      gap: 10px;
      cursor: pointer;
      user-select: none;
    }

    .checkbox-input {
      display: none;
    }

    .checkbox-custom {
      width: 20px;
      height: 20px;
      border: 2px solid var(--border-color);
      border-radius: 6px;
      position: relative;
      transition: all 0.2s;
    }

    .checkbox-input:checked + .checkbox-custom {
      background: var(--royal-blue-gradient);
      border-color: var(--royal-blue-light);
    }

    .checkbox-input:checked + .checkbox-custom::after {
      content: '✓';
      position: absolute;
      top: 50%;
      left: 50%;
      transform: translate(-50%, -50%);
      color: white;
      font-size: 12px;
      font-weight: bold;
    }

    .checkbox-text {
      font-size: 14px;
      color: var(--text-secondary);
      font-weight: 500;
    }

    .forgot-link {
      color: var(--royal-blue-light);
      text-decoration: none;
      font-size: 14px;
      font-weight: 500;
      transition: color 0.2s;
    }

    .forgot-link:hover {
      color: var(--royal-blue);
      text-decoration: underline;
    }

    .error-message {
      color: var(--error);
      font-size: 13px;
      margin-top: 6px;
      display: flex;
      align-items: center;
      gap: 6px;
    }

    .global-error {
      background: rgba(239, 68, 68, 0.1);
      border: 1px solid rgba(239, 68, 68, 0.2);
      border-radius: 8px;
      padding: 12px;
      margin-bottom: 20px;
    }

    .btn-premium {
      padding: 16px 24px;
      font-size: 16px;
      font-weight: 600;
      border-radius: 12px;
      display: flex;
      align-items: center;
      justify-content: center;
      gap: 10px;
      margin-top: 8px;
    }

    .loading-spinner {
      display: flex;
      align-items: center;
      gap: 10px;
    }

    .spinner-small {
      width: 16px;
      height: 16px;
      border: 2px solid rgba(255, 255, 255, 0.3);
      border-top-color: white;
      border-radius: 50%;
      animation: spin 0.8s linear infinite;
    }

    .auth-divider {
      display: flex;
      align-items: center;
      text-align: center;
      margin: 32px 0;
      color: var(--text-tertiary);
      font-size: 14px;
    }

    .auth-divider::before,
    .auth-divider::after {
      content: '';
      flex: 1;
      border-bottom: 1px solid var(--border-color);
    }

    .auth-divider span {
      padding: 0 16px;
    }

    .auth-footer {
      text-align: center;
      margin-top: 24px;
    }

    .auth-footer p {
      color: var(--text-secondary);
      font-size: 14px;
    }

    .link-primary {
      color: var(--royal-blue-light);
      text-decoration: none;
      font-weight: 600;
      transition: color 0.2s;
    }

    .link-primary:hover {
      color: var(--royal-blue);
      text-decoration: underline;
    }

    .theme-toggle {
      position: absolute;
      top: 24px;
      right: 24px;
      background: rgba(255, 255, 255, 0.1);
      backdrop-filter: blur(10px);
      border: 1px solid var(--glass-border);
      border-radius: 12px;
      cursor: pointer;
      padding: 10px;
      display: flex;
      align-items: center;
      justify-content: center;
      color: var(--text-primary);
      transition: all 0.3s;
      width: 44px;
      height: 44px;
    }

    .theme-toggle:hover {
      background: rgba(255, 255, 255, 0.15);
      transform: scale(1.05);
    }

    @media (max-width: 480px) {
      .auth-card {
        padding: 32px 24px;
      }

      .auth-header h1 {
        font-size: 28px;
      }

      .logo-icon {
        width: 56px;
        height: 56px;
      }
    }
  `]
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup;
  errorMessage = '';
  loading = false;
  showPassword = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    public themeService: ThemeService
  ) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      rememberMe: [false]
    });
  }

  ngOnInit(): void {
    if (this.authService.isAuthenticated()) {
      this.redirectByRole();
    }
  }

  onSubmit(): void {
    if (this.loginForm.valid) {
      this.loading = true;
      this.errorMessage = '';

      const request: LoginRequest = this.loginForm.value;
      
      this.authService.login(request).subscribe({
        next: (response) => {
          if (response.success) {
            this.redirectByRole();
          } else {
            this.errorMessage = response.message || 'Login failed';
            this.loading = false;
          }
        },
        error: (error) => {
          this.errorMessage = error.message || 'An error occurred during login';
          this.loading = false;
        }
      });
    }
  }

  toggleTheme(): void {
    this.themeService.toggleTheme();
  }

  private redirectByRole(): void {
    const role = this.authService.getRole();
    if (role === 'student') {
      this.router.navigate(['/student/dashboard']);
    } else if (role === 'teacher') {
      this.router.navigate(['/teacher/dashboard']);
    } else if (role === 'admin') {
      this.router.navigate(['/admin/dashboard']);
    }
  }
}

