import { Component, OnInit, OnDestroy, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService } from '../../../services/api.service';
import { AuthService } from '../../../services/auth.service';
import { AntiCheatService } from '../../../services/anti-cheat.service';
import { Question, SubmitAnswerRequest, ViolationRequest } from '../../../models/exam.model';

@Component({
  selector: 'app-exam-taking',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="exam-container" [class.fullscreen]="isFullscreen">
      <div class="exam-header">
        <div class="exam-info">
          <h2>{{ examTitle }}</h2>
          <div class="timer" [class.warning]="timeRemaining < 300">
            <span>⏱️</span>
            <span>{{ formatTime(timeRemaining) }}</span>
          </div>
        </div>
        <div class="violation-warning" *ngIf="violationCount > 0">
          ⚠️ Violations: {{ violationCount }}/{{ maxViolations }}
          <span *ngIf="violationCount >= maxViolations" class="critical-warning">
            - Exam will be auto-submitted!
          </span>
        </div>
      </div>

      <div class="fullscreen-warning" *ngIf="!isFullscreen && !isDocumentHidden">
        ⚠️ Please enter fullscreen mode to continue the exam
      </div>

      <div class="exam-content">
        <div class="question-nav">
          <div 
            *ngFor="let q of questions; let i = index" 
            class="question-nav-item"
            [class.active]="currentQuestionIndex === i"
            [class.answered]="answers.has(q.id!)"
            (click)="goToQuestion(i)">
            {{ i + 1 }}
          </div>
        </div>

        <div class="question-content">
          <div *ngIf="currentQuestion" class="question-card card">
            <div class="question-header">
              <h3>Question {{ currentQuestionIndex + 1 }} of {{ questions.length }}</h3>
              <span class="marks">Marks: {{ currentQuestion.marks }}</span>
            </div>
            
            <p class="question-text">{{ currentQuestion.questionText }}</p>

            <div *ngIf="currentQuestion.questionType === 'mcq'" class="options">
              <label 
                *ngFor="let option of currentQuestion.options; let i = index" 
                class="option-label"
                [class.selected]="getAnswer(currentQuestion.id!) === option">
                <input 
                  type="radio" 
                  [name]="'q' + currentQuestion.id"
                  [value]="option"
                  [checked]="getAnswer(currentQuestion.id!) === option"
                  (change)="selectAnswer(option)"
                  (copy)="$event.preventDefault()"
                  (paste)="$event.preventDefault()">
                <span>{{ option }}</span>
              </label>
            </div>

            <div *ngIf="currentQuestion.questionType === 'text'" class="text-answer">
              <textarea 
                [(ngModel)]="textAnswers[currentQuestion.id!]"
                (blur)="saveAnswer()"
                (copy)="$event.preventDefault()"
                (paste)="$event.preventDefault()"
                class="answer-textarea"
                placeholder="Type your answer here..."></textarea>
            </div>
          </div>

          <div class="exam-actions">
            <button 
              class="btn btn-secondary" 
              (click)="previousQuestion()"
              [disabled]="currentQuestionIndex === 0">
              Previous
            </button>
            <button 
              class="btn btn-primary" 
              (click)="nextQuestion()"
              [disabled]="currentQuestionIndex === questions.length - 1">
              Next
            </button>
            <button 
              class="btn btn-primary submit-btn" 
              (click)="submitExam()"
              [disabled]="submitting">
              {{ submitting ? 'Submitting...' : 'Submit Exam' }}
            </button>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .exam-container {
      min-height: 100vh;
      background: var(--background);
      padding: 20px;
    }

    .exam-container.fullscreen {
      position: fixed;
      top: 0;
      left: 0;
      right: 0;
      bottom: 0;
      z-index: 9999;
      padding: 0;
    }

    .exam-header {
      background: var(--surface);
      padding: 20px;
      margin-bottom: 20px;
      border-radius: 8px;
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    .exam-info h2 {
      margin: 0 0 10px 0;
      color: var(--text-primary);
    }

    .timer {
      display: flex;
      align-items: center;
      gap: 8px;
      font-size: 20px;
      font-weight: bold;
      color: var(--primary-color);
    }

    .timer.warning {
      color: var(--error);
      animation: pulse 1s infinite;
    }

    .violation-warning {
      color: var(--error);
      font-weight: bold;
      display: flex;
      flex-direction: column;
      gap: 5px;
    }

    .critical-warning {
      font-size: 14px;
      animation: blink 1s infinite;
    }

    @keyframes blink {
      0%, 100% { opacity: 1; }
      50% { opacity: 0.5; }
    }

    .exam-content {
      display: grid;
      grid-template-columns: 100px 1fr;
      gap: 20px;
    }

    .question-nav {
      display: flex;
      flex-direction: column;
      gap: 8px;
    }

    .question-nav-item {
      width: 40px;
      height: 40px;
      display: flex;
      align-items: center;
      justify-content: center;
      border: 2px solid var(--border-color);
      border-radius: 4px;
      cursor: pointer;
      background: var(--surface);
      color: var(--text-primary);
      transition: all 0.2s;
    }

    .question-nav-item:hover {
      border-color: var(--primary-color);
    }

    .question-nav-item.active {
      background: var(--primary-color);
      color: white;
      border-color: var(--primary-color);
    }

    .question-nav-item.answered {
      background: var(--success);
      color: white;
    }

    .question-content {
      flex: 1;
    }

    .question-card {
      margin-bottom: 20px;
    }

    .question-header {
      display: flex;
      justify-content: space-between;
      margin-bottom: 16px;
    }

    .question-text {
      font-size: 18px;
      margin-bottom: 20px;
      color: var(--text-primary);
    }

    .options {
      display: flex;
      flex-direction: column;
      gap: 12px;
    }

    .option-label {
      display: flex;
      align-items: center;
      padding: 12px;
      border: 2px solid var(--border-color);
      border-radius: 4px;
      cursor: pointer;
      transition: all 0.2s;
    }

    .option-label:hover {
      border-color: var(--primary-color);
      background: rgba(25, 118, 210, 0.1);
    }

    .option-label.selected {
      border-color: var(--primary-color);
      background: rgba(25, 118, 210, 0.2);
    }

    .option-label input {
      margin-right: 12px;
    }

    .answer-textarea {
      width: 100%;
      min-height: 200px;
      padding: 12px;
      border: 1px solid var(--border-color);
      border-radius: 4px;
      background: var(--background);
      color: var(--text-primary);
      font-size: 16px;
      resize: vertical;
    }

    .exam-actions {
      display: flex;
      gap: 12px;
      justify-content: flex-end;
    }

    .submit-btn {
      background: var(--error);
    }

    .submit-btn:hover {
      background: #c62828;
    }

    /* Prevent text selection */
    .exam-container {
      -webkit-user-select: none;
      -moz-user-select: none;
      -ms-user-select: none;
      user-select: none;
    }

    .question-text, .option-label span {
      -webkit-user-select: text;
      -moz-user-select: text;
      -ms-user-select: text;
      user-select: text;
    }

    /* Prevent image dragging */
    img {
      -webkit-user-drag: none;
      -khtml-user-drag: none;
      -moz-user-drag: none;
      -o-user-drag: none;
      user-drag: none;
      pointer-events: none;
    }

    /* Violation warning animation */
    .violation-warning {
      animation: shake 0.5s;
    }

    @keyframes shake {
      0%, 100% { transform: translateX(0); }
      25% { transform: translateX(-10px); }
      75% { transform: translateX(10px); }
    }

    /* Fullscreen warning */
    .fullscreen-warning {
      position: fixed;
      top: 0;
      left: 0;
      right: 0;
      background: var(--error);
      color: white;
      padding: 10px;
      text-align: center;
      z-index: 10001;
      font-weight: bold;
    }

    /* Watermark */
    #exam-watermark {
      position: fixed;
      top: 0;
      left: 0;
      width: 100%;
      height: 100%;
      pointer-events: none;
      z-index: 9998;
    }

    #watermark-text {
      position: fixed;
      pointer-events: none;
      z-index: 9999;
      user-select: none;
      -webkit-user-select: none;
      -moz-user-select: none;
      -ms-user-select: none;
    }

    /* Prevent screenshot with CSS */
    .exam-container::before {
      content: '';
      position: fixed;
      top: 0;
      left: 0;
      width: 100%;
      height: 100%;
      background: repeating-linear-gradient(
        0deg,
        rgba(255, 0, 0, 0.02) 0px,
        transparent 1px,
        transparent 10px
      );
      pointer-events: none;
      z-index: 9997;
    }
  `]
})
export class ExamTakingComponent implements OnInit, OnDestroy {
  examId = '';
  examTitle = 'Exam';
  questions: Question[] = [];
  currentQuestionIndex = 0;
  answers = new Map<string, string>();
  textAnswers: { [key: string]: string } = {};
  timeRemaining = 0;
  maxViolations = 3;
  violationCount = 0;
  isFullscreen = false;
  submitting = false;
  private timerInterval: any;
  private examDuration = 0;
  private answerTimings: number[] = [];
  private questionStartTimes = new Map<string, number>();
  private heartbeatInterval: any;
  private watermarkInterval: any;

  get isDocumentHidden(): boolean {
    return document.hidden;
  }

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private apiService: ApiService,
    private authService: AuthService,
    private antiCheatService: AntiCheatService
  ) {
    // Expose anti-cheat service globally for detection
    (window as any).antiCheatService = this.antiCheatService;
  }

  ngOnInit(): void {
    this.examId = this.route.snapshot.paramMap.get('id') || '';
    this.startExam();
    this.enterFullscreen();
    this.setupAntiCheat();
    this.antiCheatService.startMonitoring();
    this.setupWatermark();
    this.startHeartbeat();
  }

  ngOnDestroy(): void {
    this.exitFullscreen();
    if (this.timerInterval) {
      clearInterval(this.timerInterval);
    }
    if (this.heartbeatInterval) {
      clearInterval(this.heartbeatInterval);
    }
    if (this.watermarkInterval) {
      clearInterval(this.watermarkInterval);
    }
    this.antiCheatService.stopMonitoring();
    document.removeEventListener('visibilitychange', this.handleVisibilityChange);
    document.removeEventListener('blur', this.handleBlur);
  }

  @HostListener('window:beforeunload', ['$event'])
  onBeforeUnload(event: any): void {
    if (!this.submitting) {
      event.preventDefault();
      event.returnValue = 'Are you sure you want to leave? Your progress will be saved.';
      this.reportViolation('Page unload attempt');
    }
  }

  @HostListener('document:keydown', ['$event'])
  onKeyDown(event: KeyboardEvent): void {
    // Block F12, Ctrl+Shift+I, Ctrl+Shift+J, Ctrl+U, PrintScreen, etc.
    const blockedKeys = [
      'F12', 'PrintScreen', 'F1', 'F2', 'F3', 'F4', 'F5', 'F6', 'F7', 'F8', 'F9', 'F10', 'F11'
    ];
    
    const blockedCombinations = [
      { ctrl: true, shift: true, keys: ['I', 'J', 'C', 'K', 'Delete'] },
      { ctrl: true, keys: ['U', 'S', 'P', 'A', 'C', 'V', 'X', 'F', 'G', 'H'] },
      { alt: true, keys: ['Tab', 'F4'] },
      { meta: true, keys: ['Tab'] }
    ];

    if (blockedKeys.includes(event.key)) {
      event.preventDefault();
      event.stopPropagation();
      this.reportViolation(`Blocked key: ${event.key}`);
      return;
    }

    for (const combo of blockedCombinations) {
      const ctrlMatch = combo.ctrl ? event.ctrlKey : !event.ctrlKey;
      const shiftMatch = combo.shift ? event.shiftKey : !event.shiftKey;
      const altMatch = combo.alt ? event.altKey : !event.altKey;
      const metaMatch = combo.meta ? event.metaKey : !event.metaKey;
      
      if (ctrlMatch && shiftMatch && altMatch && metaMatch && combo.keys.includes(event.key)) {
        event.preventDefault();
        event.stopPropagation();
        this.reportViolation(`Blocked keyboard shortcut: ${event.key}`);
        return;
      }
    }
  }

  @HostListener('contextmenu', ['$event'])
  onContextMenu(event: MouseEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.reportViolation('Right-click attempt');
  }

  @HostListener('window:resize', ['$event'])
  onResize(event: any): void {
    // Monitor window resize which might indicate trying to escape fullscreen
    if (!document.fullscreenElement && this.isFullscreen) {
      this.reportViolation('Window resize detected (possible fullscreen exit)');
      this.enterFullscreen();
    }
  }

  startExam(): void {
    this.apiService.startExam(this.examId).subscribe({
      next: (response) => {
        if (response.success) {
          this.loadQuestions();
        }
      },
      error: (error) => {
        console.error('Error starting exam:', error);
        alert(error.message || 'Failed to start exam');
        this.router.navigate(['/student/dashboard']);
      }
    });
  }

  loadQuestions(): void {
    this.apiService.getQuestions(this.examId).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.questions = response.data;
          // Questions are already randomized on backend
          this.examDuration = this.questions.length * 2; // 2 min per question default
          this.timeRemaining = this.examDuration * 60;
          this.startTimer();
          
          // Start timing for first question
          if (this.questions.length > 0 && this.questions[0].id) {
            this.questionStartTimes.set(this.questions[0].id, Date.now());
          }
        }
      },
      error: (error) => {
        console.error('Error loading questions:', error);
      }
    });
  }

  get currentQuestion(): Question | null {
    return this.questions[this.currentQuestionIndex] || null;
  }

  selectAnswer(option: string): void {
    if (this.currentQuestion) {
      const questionId = this.currentQuestion.id!;
      const startTime = this.questionStartTimes.get(questionId) || Date.now();
      const timeSpent = (Date.now() - startTime) / 1000; // in seconds
      
      this.answers.set(questionId, option);
      this.answerTimings.push(timeSpent);
      
      // Analyze answer pattern
      if (this.answerTimings.length > 3) {
        this.antiCheatService.analyzeAnswerPattern(
          Array.from(this.answers.values()),
          this.answerTimings
        );
      }
      
      this.saveAnswer();
    }
  }

  getAnswer(questionId: string): string {
    return this.answers.get(questionId) || this.textAnswers[questionId] || '';
  }

  saveAnswer(): void {
    if (this.currentQuestion) {
      const answer = this.currentQuestion.questionType === 'mcq' 
        ? this.answers.get(this.currentQuestion.id!) 
        : this.textAnswers[this.currentQuestion.id!];
      
      if (answer) {
        const request: SubmitAnswerRequest = {
          questionId: this.currentQuestion.id!,
          answer: answer
        };
        
        this.apiService.submitAnswer(this.examId, request).subscribe({
          error: (error) => console.error('Error saving answer:', error)
        });
      }
    }
  }

  previousQuestion(): void {
    if (this.currentQuestionIndex > 0) {
      this.saveAnswer();
      this.currentQuestionIndex--;
    }
  }

  nextQuestion(): void {
    if (this.currentQuestionIndex < this.questions.length - 1) {
      this.saveAnswer();
      this.currentQuestionIndex++;
    }
  }

  goToQuestion(index: number): void {
    this.saveAnswer();
    
    // Track question navigation time
    if (this.currentQuestion) {
      const questionId = this.currentQuestion.id!;
      if (!this.questionStartTimes.has(questionId)) {
        this.questionStartTimes.set(questionId, Date.now());
      }
    }
    
    this.currentQuestionIndex = index;
    
    // Start timing for new question
    const newQuestion = this.questions[index];
    if (newQuestion && newQuestion.id) {
      this.questionStartTimes.set(newQuestion.id, Date.now());
    }
  }

  startTimer(): void {
    this.timerInterval = setInterval(() => {
      this.timeRemaining--;
      if (this.timeRemaining <= 0) {
        this.submitExam();
      }
    }, 1000);
  }

  formatTime(seconds: number): string {
    const mins = Math.floor(seconds / 60);
    const secs = seconds % 60;
    return `${mins.toString().padStart(2, '0')}:${secs.toString().padStart(2, '0')}`;
  }

  submitExam(): void {
    if (this.submitting) return;
    
    this.submitting = true;
    this.saveAnswer();
    
    if (this.timerInterval) {
      clearInterval(this.timerInterval);
    }

    this.apiService.submitExam(this.examId).subscribe({
      next: (response) => {
        if (response.success) {
          alert('Exam submitted successfully!');
          this.router.navigate(['/student/dashboard']);
        }
      },
      error: (error) => {
        alert(error.message || 'Error submitting exam');
        this.submitting = false;
      }
    });
  }

  setupAntiCheat(): void {
    // Tab switch detection
    this.handleVisibilityChange = () => {
      if (document.hidden) {
        this.reportViolation('Tab switch detected');
      } else {
        // Check if fullscreen is still active when tab becomes visible
        if (!document.fullscreenElement && this.isFullscreen) {
          this.reportViolation('Fullscreen exit detected');
          this.enterFullscreen();
        }
      }
    };
    document.addEventListener('visibilitychange', this.handleVisibilityChange);

    // Window blur detection
    this.handleBlur = () => {
      this.reportViolation('Window blur detected');
    };
    window.addEventListener('blur', this.handleBlur);

    // Window focus detection
    window.addEventListener('focus', () => {
      if (!document.fullscreenElement && this.isFullscreen) {
        this.reportViolation('Fullscreen exit on focus');
        this.enterFullscreen();
      }
    });

    // Copy/Paste blocking
    document.addEventListener('copy', (e) => {
      e.preventDefault();
      this.reportViolation('Copy attempt');
    });

    document.addEventListener('paste', (e) => {
      e.preventDefault();
      this.reportViolation('Paste attempt');
    });

    document.addEventListener('cut', (e) => {
      e.preventDefault();
      this.reportViolation('Cut attempt');
    });

    // Print blocking
    window.addEventListener('beforeprint', (e) => {
      e.preventDefault();
      this.reportViolation('Print attempt');
    });

    // Screenshot detection (PrintScreen key)
    document.addEventListener('keydown', (e) => {
      // Block PrintScreen, F12, and common dev tools shortcuts
      if (e.key === 'PrintScreen' || 
          e.key === 'F12' ||
          (e.ctrlKey && e.shiftKey && (e.key === 'I' || e.key === 'J' || e.key === 'C')) ||
          (e.ctrlKey && e.key === 'U') ||
          (e.ctrlKey && e.shiftKey && e.key === 'K') ||
          (e.ctrlKey && e.shiftKey && e.key === 'Delete')) {
        e.preventDefault();
        e.stopPropagation();
        this.reportViolation(`Blocked keyboard shortcut: ${e.key}`);
      }
    }, true);

    // Developer tools detection
    this.detectDevTools();

    // Mouse leave detection
    document.addEventListener('mouseleave', (e) => {
      if (e.clientY <= 0) {
        this.reportViolation('Mouse left window area');
      }
    });

    // Fullscreen change monitoring
    document.addEventListener('fullscreenchange', () => {
      if (!document.fullscreenElement && this.isFullscreen) {
        this.reportViolation('Fullscreen exit detected');
        setTimeout(() => this.enterFullscreen(), 100);
      }
    });

    // Periodic focus and fullscreen checks
    setInterval(() => {
      if (!document.hasFocus() && !document.hidden) {
        this.reportViolation('Window lost focus');
      }
      
      if (!document.fullscreenElement && this.isFullscreen) {
        this.reportViolation('Fullscreen not active');
        this.enterFullscreen();
      }

      // Check for multiple windows
      if (window.screenLeft < 0 || window.screenTop < 0) {
        this.reportViolation('Window position suspicious');
      }

      // Check for window resize (possible attempt to escape fullscreen)
      if (window.innerWidth < screen.width * 0.9 || window.innerHeight < screen.height * 0.9) {
        if (this.isFullscreen) {
          this.reportViolation('Window resized during fullscreen');
          this.enterFullscreen();
        }
      }
    }, 3000); // Check every 3 seconds (more frequent)

    // Disable text selection (additional protection)
    document.addEventListener('selectstart', (e) => {
      e.preventDefault();
      return false;
    });

    // Disable drag and drop
    document.addEventListener('dragstart', (e) => {
      e.preventDefault();
      this.reportViolation('Drag attempt');
    });

    document.addEventListener('drop', (e) => {
      e.preventDefault();
      this.reportViolation('Drop attempt');
    });

    // Block all forms of screenshot
    document.addEventListener('keydown', (e) => {
      // Windows: PrintScreen, Alt+PrintScreen, Win+PrintScreen
      if (e.key === 'PrintScreen' || 
          (e.altKey && e.key === 'PrintScreen') ||
          (e.metaKey && e.key === 'PrintScreen')) {
        e.preventDefault();
        e.stopPropagation();
        this.reportViolation('Screenshot attempt blocked');
      }
    }, true);

    // Block browser extensions that might interfere
    if ((window as any).chrome?.runtime) {
      try {
        (window as any).chrome.runtime.onConnect = null;
      } catch (e) {}
    }

    // Prevent iframe embedding
    if (window.self !== window.top) {
      this.reportViolation('Iframe embedding detected');
      window.top!.location.href = window.self.location.href;
    }

    // Block common cheating tools
    const blockedFunctions = ['debugger', 'eval', 'Function'];
    blockedFunctions.forEach(func => {
      try {
        (window as any)[func] = function() {
          this.reportViolation(`Blocked function call: ${func}`);
          throw new Error(`${func} is not allowed during exam`);
        };
      } catch (e) {}
    });

    // Monitor for suspicious extensions
    setTimeout(() => {
      if ((navigator as any).plugins.length === 0 && 
          !(navigator as any).mimeTypes.length) {
        this.reportViolation('Suspicious browser configuration');
      }
    }, 2000);
  }

  private detectDevTools(): void {
    let devToolsOpen = false;
    const element = new Image();
    Object.defineProperty(element, 'id', {
      get: () => {
        devToolsOpen = true;
        this.reportViolation('Developer tools detected');
      }
    });

    setInterval(() => {
      devToolsOpen = false;
      console.log(element);
      console.clear();
      if (devToolsOpen) {
        this.reportViolation('Developer tools opened');
      }
    }, 1000);
  }

  private handleVisibilityChange: () => void = () => {};
  private handleBlur: () => void = () => {};

  reportViolation(type: string): void {
    this.violationCount++;
    this.antiCheatService.reportViolation(type, 'medium');
    
    const request: ViolationRequest = {
      examId: this.examId,
      violationType: type
    };

    this.apiService.reportViolation(this.examId, request).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.violationCount = response.data.violationCount || this.violationCount;
          if (response.data.autoSubmitted) {
            alert('Maximum violations reached! Exam auto-submitted.');
            this.router.navigate(['/student/dashboard']);
          }
        }
      },
      error: (error) => console.error('Error reporting violation:', error)
    });
  }

  private startHeartbeat(): void {
    // Send periodic heartbeat to backend to validate session
    this.heartbeatInterval = setInterval(() => {
      const fingerprint = this.antiCheatService.getBrowserFingerprint();
      this.apiService.validateSession(this.examId, fingerprint).subscribe({
        next: (response) => {
          if (response.success && response.data) {
            if (response.data.autoSubmitted) {
              alert('Session invalidated. Exam auto-submitted.');
              this.router.navigate(['/student/dashboard']);
            }
            if (response.data.violationCount !== undefined) {
              this.violationCount = response.data.violationCount;
            }
          }
        },
        error: (error) => {
          if (error.error?.message?.includes('auto-submitted') || 
              error.error?.message?.includes('expired')) {
            alert(error.error.message);
            this.router.navigate(['/student/dashboard']);
          }
        }
      });
    }, 10000); // Every 10 seconds
  }

  private setupWatermark(): void {
    // Add dynamic watermark to prevent screenshots
    const watermark = document.createElement('div');
    watermark.id = 'exam-watermark';
    watermark.style.cssText = `
      position: fixed;
      top: 0;
      left: 0;
      width: 100%;
      height: 100%;
      pointer-events: none;
      z-index: 9998;
      background: repeating-linear-gradient(
        45deg,
        transparent,
        transparent 100px,
        rgba(255, 0, 0, 0.03) 100px,
        rgba(255, 0, 0, 0.03) 200px
      );
    `;
    document.body.appendChild(watermark);

    // Add student ID watermark text
    const watermarkText = document.createElement('div');
    watermarkText.id = 'watermark-text';
    const user = this.authService.getUser();
    const studentId = user?.userId || 'STUDENT';
    watermarkText.textContent = `EXAM MODE - ${studentId} - ${new Date().toISOString()}`;
    watermarkText.style.cssText = `
      position: fixed;
      top: 50%;
      left: 50%;
      transform: translate(-50%, -50%) rotate(-45deg);
      font-size: 48px;
      color: rgba(255, 0, 0, 0.1);
      pointer-events: none;
      z-index: 9999;
      white-space: nowrap;
      user-select: none;
      font-weight: bold;
    `;
    document.body.appendChild(watermarkText);

    // Update watermark text periodically
    this.watermarkInterval = setInterval(() => {
      const timestamp = new Date().toISOString();
      watermarkText.textContent = `EXAM MODE - ${studentId} - ${timestamp}`;
    }, 1000);
  }

  enterFullscreen(): void {
    const elem = document.documentElement;
    if (elem.requestFullscreen) {
      elem.requestFullscreen().then(() => {
        this.isFullscreen = true;
      }).catch((err) => {
        console.error('Fullscreen error:', err);
        this.reportViolation('Fullscreen exit attempt');
        // Try again after a short delay
        setTimeout(() => {
          if (!document.fullscreenElement) {
            this.enterFullscreen();
          }
        }, 1000);
      });
    } else if ((elem as any).webkitRequestFullscreen) {
      // Safari support
      (elem as any).webkitRequestFullscreen();
      this.isFullscreen = true;
    } else if ((elem as any).mozRequestFullScreen) {
      // Firefox support
      (elem as any).mozRequestFullScreen();
      this.isFullscreen = true;
    } else if ((elem as any).msRequestFullscreen) {
      // IE/Edge support
      (elem as any).msRequestFullscreen();
      this.isFullscreen = true;
    }
  }

  exitFullscreen(): void {
    if (document.fullscreenElement) {
      document.exitFullscreen();
    }
    this.isFullscreen = false;
  }

  private shuffleArray<T>(array: T[]): T[] {
    const shuffled = [...array];
    for (let i = shuffled.length - 1; i > 0; i--) {
      const j = Math.floor(Math.random() * (i + 1));
      [shuffled[i], shuffled[j]] = [shuffled[j], shuffled[i]];
    }
    return shuffled;
  }
}

