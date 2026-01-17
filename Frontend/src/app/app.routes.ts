import { Routes } from '@angular/router';

export const routes: Routes = [
    { path: '', redirectTo: 'auth/login', pathMatch: 'full' },
    { path: 'auth', loadChildren: () => import('./auth/auth-module').then(m => m.AuthModule) },
    { path: 'student', loadChildren: () => import('./student/student-module').then(m => m.StudentModule) },
    { path: 'admin', loadChildren: () => import('./admin/admin-module').then(m => m.AdminModule) },
    { path: '**', redirectTo: 'auth/login' }
];
