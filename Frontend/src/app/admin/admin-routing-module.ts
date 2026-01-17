import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { Dashboard } from './components/dashboard/dashboard';
import { ManageExams } from './components/manage-exams/manage-exams';

const routes: Routes = [
  { path: 'dashboard', component: Dashboard },
  { path: 'manage-exams', component: ManageExams },
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AdminRoutingModule { }
