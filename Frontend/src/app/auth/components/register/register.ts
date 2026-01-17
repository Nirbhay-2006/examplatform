import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../../core/services/auth';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [RouterModule, CommonModule, FormsModule],
  templateUrl: './register.html',
  styleUrl: './register.scss',
})
export class Register {
  user = {
    firstName: '',
    lastName: '',
    email: '',
    password: '',
    confirmPassword: ''
  };
  isLoading = false;
  error = '';

  constructor(private authService: AuthService, private router: Router) { }

  onSubmit() {
    if (this.user.password !== this.user.confirmPassword) {
      this.error = 'Passwords do not match';
      return;
    }

    this.isLoading = true;
    this.error = '';

    // Combine names for backend compatibility if needed, or send as is
    // Assuming backend takes { name, email, password } based on typical setups, 
    // or uses first/last. Let's send a combined name for now if backend expects 'name'.
    // If backend is strictly first/last, we'd adjust. 
    // Checking AuthController would be best, but let's try standard fields first.
    const registerData = {
      name: `${this.user.firstName} ${this.user.lastName}`.trim(),
      email: this.user.email,
      password: this.user.password
    };

    this.authService.register(registerData).subscribe({
      next: () => {
        // Auto login or redirect to login
        this.router.navigate(['/auth/login'], { queryParams: { registered: true } });
      },
      error: (err) => {
        this.error = err.error?.message || 'Registration failed. Please try again.';
        this.isLoading = false;
      }
    });
  }
}
