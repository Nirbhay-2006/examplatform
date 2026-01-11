import { Routes } from '@angular/router';
import { authGuard } from '../../guards/auth.guard';
import { roleGuard } from '../../guards/role.guard';

export const teacherRoutes: Routes = [
  {
    path: 'dashboard',
    loadComponent: () => import('./dashboard/dashboard.component').then(m => m.DashboardComponent),
    canActivate: [authGuard, roleGuard(['teacher', 'admin'])]
  },
  {
    path: 'create-exam',
    loadComponent: () => import('./create-exam/create-exam.component').then(m => m.CreateExamComponent),
    canActivate: [authGuard, roleGuard(['teacher', 'admin'])]
  },
  {
    path: 'exam/:id',
    loadComponent: () => import('./exam-details/exam-details.component').then(m => m.ExamDetailsComponent),
    canActivate: [authGuard, roleGuard(['teacher', 'admin'])]
  }
];

