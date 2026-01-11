import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { ApiService } from '../../../services/api.service';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-upgrade',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="upgrade-container fade-in">
      <div class="upgrade-card card">
        <h1>Upgrade to Paid</h1>
        <p>Unlock all premium features and access paid exams</p>
        <div class="pricing">
          <h2>₹999/month</h2>
        </div>
        <button class="btn btn-primary btn-large" (click)="upgrade()" [disabled]="processing">
          {{ processing ? 'Processing...' : 'Upgrade Now' }}
        </button>
        <button class="btn btn-secondary" routerLink="/student/dashboard">Cancel</button>
      </div>
    </div>
  `,
  styles: [`
    .upgrade-container {
      min-height: 100vh;
      display: flex;
      align-items: center;
      justify-content: center;
      padding: 20px;
    }

    .upgrade-card {
      max-width: 400px;
      text-align: center;
    }

    .pricing {
      margin: 30px 0;
    }

    .btn-large {
      width: 100%;
      padding: 16px;
      margin-bottom: 12px;
    }
  `]
})
export class UpgradeComponent {
  processing = false;

  constructor(
    private apiService: ApiService,
    private authService: AuthService,
    private router: Router
  ) {}

  upgrade(): void {
    this.processing = true;
    this.apiService.createOrder(999).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          // In production, integrate with Razorpay/Stripe
          alert('Payment integration needed. For now, subscription activated.');
          this.router.navigate(['/student/dashboard']);
        }
      },
      error: () => {
        this.processing = false;
      }
    });
  }
}

