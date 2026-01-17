import { Component, EventEmitter, Output } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
    selector: 'app-subscription-modal',
    standalone: true,
    imports: [CommonModule],
    templateUrl: './subscription-modal.html',
    styleUrl: './subscription-modal.scss'
})
export class SubscriptionModal {
    @Output() close = new EventEmitter<void>();
    isLoading = false;

    upgrade() {
        this.isLoading = true;
        // Simulate API call using PaymentController mock
        setTimeout(() => {
            this.isLoading = false;
            alert('Payment Successful! You are now a Premium user.');
            // In real app, we would reload user or update AuthService state here
            // For now, reloading page to refresh token is simplest as Backend updates DB
            window.location.reload();
        }, 2000);
    }

    onClose() {
        this.close.emit();
    }
}
