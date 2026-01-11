import { Routes } from '@angular/router';
import { authGuard } from '../../guards/auth.guard';
import { roleGuard } from '../../guards/role.guard';

export const studentRoutes: Routes = [
  {
    path: 'dashboard',
    loadComponent: () => import('./dashboard/dashboard.component').then(m => m.DashboardComponent),
    canActivate: [authGuard, roleGuard(['student'])]
  },
  {
    path: 'exam/:id',
    loadComponent: () => import('./exam-taking/exam-taking.component').then(m => m.ExamTakingComponent),
    canActivate: [authGuard, roleGuard(['student'])]
  },
  {
    path: 'results',
    loadComponent: () => import('./results/results.component').then(m => m.ResultsComponent),
    canActivate: [authGuard, roleGuard(['student'])]
  }
];

