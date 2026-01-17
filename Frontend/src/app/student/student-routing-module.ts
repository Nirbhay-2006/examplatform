import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { Dashboard } from './components/dashboard/dashboard';
import { ExamList } from './components/exam-list/exam-list';
import { ExamTake } from './components/exam-take/exam-take';

const routes: Routes = [
  { path: 'dashboard', component: Dashboard },
  { path: 'exams', component: ExamList },
  { path: 'exam/:id', component: ExamTake },
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class StudentRoutingModule { }
