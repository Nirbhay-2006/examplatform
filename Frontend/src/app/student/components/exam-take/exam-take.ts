import { Component, OnInit, OnDestroy, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ExamService } from '../../../core/services/exam';
import { Question } from '../../../core/models/exam';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-exam-take',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './exam-take.html',
  styleUrl: './exam-take.scss',
})
export class ExamTake implements OnInit, OnDestroy {
  examId: string = '';
  questions: Question[] = [];
  currentQuestionIndex = 0;
  answers: { [key: string]: string } = {};
  timeRemaining = 0; // seconds
  timerInterval: any;
  isLoading = true;
  isSubmitting = false;

  // Anti-cheat
  violationCount = 0;
  maxViolations = 3; // Default, will update from API
  fullScreenWarning = false;

  // To track tab switches
  hiddenProp: string | null = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private examService: ExamService
  ) { }

  ngOnInit() {
    this.examId = this.route.snapshot.paramMap.get('id') || '';
    if (this.examId) {
      this.startExam();
    }

    // Setup visibility change listener
    this.setupVisibilityListener();
  }

  ngOnDestroy() {
    if (this.timerInterval) clearInterval(this.timerInterval);
    document.removeEventListener('visibilitychange', this.handleVisibilityChange);
    document.removeEventListener('contextmenu', this.preventRightClick);
    document.removeEventListener('copy', this.preventCopy);
  }

  startExam() {
    this.isLoading = true;
    this.examService.startExam(this.examId).subscribe({
      next: () => {
        this.fetchQuestions();
        this.validateSession(); // Get initial time and violation count
      },
      error: (err) => {
        alert(err.error?.message || 'Failed to start exam');
        this.router.navigate(['/student/exams']);
      }
    });

    // Prevent interactions
    document.addEventListener('contextmenu', this.preventRightClick);
    document.addEventListener('copy', this.preventCopy);
  }

  fetchQuestions() {
    this.examService.getQuestions(this.examId).subscribe(res => {
      this.questions = res.data;
      this.isLoading = false;
    });
  }

  validateSession() {
    this.examService.validateSession(this.examId).subscribe(res => {
      this.timeRemaining = res.data.timeRemaining;
      this.violationCount = res.data.violationCount;
      this.maxViolations = res.data.maxViolations;
      this.startTimer();
    });
  }

  startTimer() {
    if (this.timerInterval) clearInterval(this.timerInterval);
    this.timerInterval = setInterval(() => {
      this.timeRemaining--;
      if (this.timeRemaining <= 0) {
        this.submitExam();
      }

      // Periodically sync (every 60s)
      if (this.timeRemaining % 60 === 0) {
        // this.validateSession(); // Optional sync
      }
    }, 1000);
  }

  formatTime(seconds: number): string {
    const h = Math.floor(seconds / 3600);
    const m = Math.floor((seconds % 3600) / 60);
    const s = seconds % 60;
    return `${h}:${m.toString().padStart(2, '0')}:${s.toString().padStart(2, '0')}`;
  }

  nextQuestion() {
    if (this.currentQuestionIndex < this.questions.length - 1) {
      this.saveCurrentAnswer();
      this.currentQuestionIndex++;
    }
  }

  prevQuestion() {
    if (this.currentQuestionIndex > 0) {
      this.saveCurrentAnswer();
      this.currentQuestionIndex--;
    }
  }

  saveCurrentAnswer() {
    const q = this.questions[this.currentQuestionIndex];
    const ans = this.answers[q.id!];
    if (ans) {
      this.examService.submitAnswer(this.examId, q.id!, ans).subscribe();
    }
  }

  submitExam() {
    if (this.isSubmitting) return;
    this.isSubmitting = true;
    this.saveCurrentAnswer();

    this.examService.submitExam(this.examId).subscribe({
      next: () => {
        alert('Exam Submitted Successfully!');
        this.router.navigate(['/student/dashboard']);
      },
      error: () => {
        this.isSubmitting = false;
        alert('Submission failed, please try again.');
      }
    });
  }

  // Anti-Cheat Methods

  setupVisibilityListener() {
    document.addEventListener('visibilitychange', this.handleVisibilityChange.bind(this));
    window.addEventListener('blur', this.handleVisibilityChange.bind(this));
  }

  handleVisibilityChange(e: Event) {
    if (document.hidden || document.visibilityState === 'hidden') { // Simplified check
      this.reportViolation('tab_switch');
    }
  }

  @HostListener('window:blur', ['$event'])
  onWindowBlur(event: any): void {
    this.reportViolation('tab_switch');
  }

  preventRightClick(event: any) {
    event.preventDefault();
    return false;
  }

  preventCopy(event: any) {
    event.preventDefault();
    return false;
  }

  reportViolation(type: string) {
    // Debounce prevention could be good, but for now direct call
    this.examService.reportViolation(this.examId, type).subscribe(res => {
      this.violationCount = res.data.violationCount;
      this.showViolationWarning(type);

      if (res.data.autoSubmitted) {
        alert('Exam Auto-Submitted due to multiple violations.');
        this.router.navigate(['/student/dashboard']);
      }
    });
  }

  showViolationWarning(type: string) {
    const toast = document.createElement('div');
    toast.className = 'violation-toast';
    toast.innerText = `Warning: ${type} detected! Violation ${this.violationCount}/${this.maxViolations}`;
    document.body.appendChild(toast);
    setTimeout(() => toast.remove(), 4000);
  }
}

