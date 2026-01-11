import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ApiService } from '../../../services/api.service';

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="users-container fade-in">
      <header>
        <h1>User Management</h1>
        <button class="btn btn-secondary" routerLink="/admin/dashboard">Back</button>
      </header>
      <div class="users-list">
        <div *ngFor="let user of users" class="user-card card">
          <div class="user-info">
            <h3>{{ user.name }}</h3>
            <p>{{ user.email }}</p>
            <span class="badge" [class.student]="user.role === 'student'" 
                  [class.teacher]="user.role === 'teacher'"
                  [class.admin]="user.role === 'admin'">
              {{ user.role }}
            </span>
          </div>
          <div class="user-actions">
            <button class="btn btn-secondary" (click)="toggleUser(user)">
              {{ user.isActive ? 'Deactivate' : 'Activate' }}
            </button>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .users-container {
      max-width: 1200px;
      margin: 0 auto;
      padding: 20px;
    }

    header {
      display: flex;
      justify-content: space-between;
      margin-bottom: 20px;
    }

    .user-card {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 16px;
    }

    .badge {
      padding: 4px 8px;
      border-radius: 4px;
      font-size: 12px;
      font-weight: bold;
    }

    .badge.student {
      background: var(--primary-color);
      color: white;
    }

    .badge.teacher {
      background: var(--warning);
      color: white;
    }

    .badge.admin {
      background: var(--error);
      color: white;
    }
  `]
})
export class UsersComponent implements OnInit {
  users: any[] = [];

  constructor(private apiService: ApiService) {}

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.apiService.getAllUsers().subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.users = response.data;
        }
      }
    });
  }

  toggleUser(user: any): void {
    if (user.isActive) {
      this.apiService.deactivateUser(user.id).subscribe({
        next: () => this.loadUsers()
      });
    } else {
      this.apiService.activateUser(user.id).subscribe({
        next: () => this.loadUsers()
      });
    }
  }
}

