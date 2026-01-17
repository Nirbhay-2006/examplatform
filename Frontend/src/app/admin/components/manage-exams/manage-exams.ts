import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ExamService } from '../../../core/services/exam';
import { AuthService } from '../../../core/services/auth';
import { Exam } from '../../../core/models/exam';

@Component({
  selector: 'app-manage-exams',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './manage-exams.html',
  styleUrl: './manage-exams.scss',
})
export class ManageExams implements OnInit {
  exams: Exam[] = [];
  isLoading = false;
  isPremium = false;
  canCreateExam = true;
  maxFreeExams = 5;

  constructor(
    private examService: ExamService,
    private authService: AuthService
  ) {
    this.authService.isPremium$.subscribe(val => this.isPremium = val);
  }

  ngOnInit() {
    this.loadExams();
  }

  loadExams() {
    this.isLoading = true;
    this.examService.getTeacherExams().subscribe({
      next: (res) => {
        this.exams = res.data || [];
        this.checkLimit();
        this.isLoading = false;
      },
      error: (err) => {
        console.error(err);
        this.isLoading = false;
      }
    });
  }

  checkLimit() {
    if (!this.isPremium && this.exams.length >= this.maxFreeExams) {
      this.canCreateExam = false;
    } else {
      this.canCreateExam = true;
    }
  }

  onCreateClick() {
    if (!this.canCreateExam) {
      alert(`Free Plan Limit Reached! You have created ${this.exams.length} / ${this.maxFreeExams} exams. Please upgrade to create more.`);
      return;
    }
    // Navigate to create exam page or open modal
    // For now, prompt generic message or navigate if route exists
    // this.router.navigate(['/admin/create-exam']); 
    // Since create exam page isn't fully scaffolded in this task, I'll alert for now or assume route
    alert('Navigate to Create Exam Page (Not implemented in this task scope, but limit check is working)');
  }
}
