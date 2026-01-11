import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { ThemeService } from '../../../services/theme.service';
import { RegisterRequest } from '../../../models/user.model';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  template: `
    <div class="auth-container fade-in">
      <div class="auth-card">
        <div class="auth-header">
          <h1>Create Account</h1>
          <p>Sign up to get started</p>
        </div>

        <form [formGroup]="registerForm" (ngSubmit)="onSubmit()" class="auth-form">
          <div class="form-group">
            <label class="form-label">Full Name</label>
            <input 
              type="text" 
              formControlName="name" 
              class="form-input"
              placeholder="Enter your full name"
              [class.error]="registerForm.get('name')?.invalid && registerForm.get('name')?.touched">
            <div *ngIf="registerForm.get('name')?.invalid && registerForm.get('name')?.touched" class="error-message">
              Name is required (min 2 characters)
            </div>
          </div>

          <div class="form-group">
            <label class="form-label">Email</label>
            <input 
              type="email" 
              formControlName="email" 
              class="form-input"
              placeholder="Enter your email"
              [class.error]="registerForm.get('email')?.invalid && registerForm.get('email')?.touched">
            <div *ngIf="registerForm.get('email')?.invalid && registerForm.get('email')?.touched" class="error-message">
              Valid email is required
            </div>
          </div>

          <div class="form-group">
            <label class="form-label">Password</label>
            <input 
              type="password" 
              formControlName="password" 
              class="form-input"
              placeholder="Enter your password"
              [class.error]="registerForm.get('password')?.invalid && registerForm.get('password')?.touched">
            <div *ngIf="registerForm.get('password')?.invalid && registerForm.get('password')?.touched" class="error-message">
              Password must be at least 6 characters
            </div>
          </div>

          <div class="form-group">
            <label class="form-label">Role</label>
            <select formControlName="role" class="form-input">
              <option value="student">Student</option>
              <option value="teacher">Teacher</option>
            </select>
          </div>

          <div *ngIf="errorMessage" class="error-message">{{ errorMessage }}</div>
          <div *ngIf="successMessage" class="success-message">{{ successMessage }}</div>

          <button 
            type="submit" 
            class="btn btn-primary btn-block"
            [disabled]="registerForm.invalid || loading">
            <span *ngIf="!loading">Create Account</span>
            <span *ngIf="loading">Creating Account...</span>
          </button>
        </form>

        <div class="auth-footer">
          <p>Already have an account? <a routerLink="/login">Sign In</a></p>
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

    .btn:disabled {
      opacity: 0.6;
      cursor: not-allowed;
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
export class RegisterComponent {
  registerForm: FormGroup;
  errorMessage = '';
  successMessage = '';
  loading = false;
  registeredEmail = '';

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    public themeService: ThemeService
  ) {
    this.registerForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      role: ['student', Validators.required]
    });
  }

  onSubmit(): void {
    if (this.registerForm.valid) {
      this.loading = true;
      this.errorMessage = '';
      this.successMessage = '';

      const request: RegisterRequest = this.registerForm.value;
      
      this.authService.register(request).subscribe({
        next: (response) => {
          if (response.success) {
            this.registeredEmail = request.email;
            this.successMessage = response.message || 'Registration successful! Please verify your email.';
            setTimeout(() => {
              this.router.navigate(['/verify-otp'], { 
                queryParams: { email: this.registeredEmail } 
              });
            }, 2000);
          } else {
            this.errorMessage = response.message || 'Registration failed';
            this.loading = false;
          }
        },
        error: (error) => {
          this.errorMessage = error.message || 'An error occurred during registration';
          this.loading = false;
        }
      });
    }
  }

  toggleTheme(): void {
    this.themeService.toggleTheme();
  }
}

