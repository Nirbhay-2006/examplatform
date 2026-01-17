import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../core/services/auth';
import { SubscriptionModal } from '../subscription-modal/subscription-modal';

@Component({
  selector: 'app-navbar',
  imports: [RouterModule, CommonModule, SubscriptionModal],
  templateUrl: './navbar.html',
  styleUrl: './navbar.scss',
})
export class Navbar {
  isPremium = false;
  showSubscriptionModal = false;
  currentUser: any = null;

  constructor(private authService: AuthService) {
    this.authService.isPremium$.subscribe(isPremium => this.isPremium = isPremium);
    this.authService.currentUser.subscribe(user => this.currentUser = user);
  }

  logout() {
    this.authService.logout();
  }

  openSubscriptionModal() {
    this.showSubscriptionModal = true;
  }
}
