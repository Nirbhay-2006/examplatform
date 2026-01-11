import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormArray } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { ApiService } from '../../../services/api.service';
import { AuthService } from '../../../services/auth.service';
import { ThemeService } from '../../../services/theme.service';
import { CreateExamRequest, CreateQuestionRequest } from '../../../models/exam.model';

@Component({
  selector: 'app-create-exam',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  template: `
    <div class="create-exam-container fade-in">
      <header>
        <h1>Create New Exam</h1>
        <button class="btn btn-secondary" routerLink="/teacher/dashboard">Back</button>
      </header>

      <form [formGroup]="examForm" (ngSubmit)="onSubmit()" class="exam-form">
        <div class="form-section card">
          <h2>Exam Details</h2>
          <div class="form-group">
            <label>Title</label>
            <input formControlName="title" class="form-input" placeholder="Exam Title">
          </div>
          <div class="form-group">
            <label>Description</label>
            <textarea formControlName="description" class="form-input" rows="3"></textarea>
          </div>
          <div class="form-row">
            <div class="form-group">
              <label>Duration (minutes)</label>
              <input type="number" formControlName="duration" class="form-input">
            </div>
            <div class="form-group">
              <label>Total Marks</label>
              <input type="number" formControlName="totalMarks" class="form-input">
            </div>
            <div class="form-group">
              <label>Passing Marks</label>
              <input type="number" formControlName="passingMarks" class="form-input">
            </div>
          </div>
          <div class="form-row">
            <div class="form-group">
              <label>Start Time</label>
              <input type="datetime-local" formControlName="startTime" class="form-input">
            </div>
            <div class="form-group">
              <label>End Time</label>
              <input type="datetime-local" formControlName="endTime" class="form-input">
            </div>
          </div>
          <div class="form-row form-row-inline">
            <div class="form-group toggle-group">
              <label class="switch-label">
                <span>Paid Exam</span>
                <label class="switch">
                  <input type="checkbox" formControlName="isPaid">
                  <span class="slider"></span>
                </label>
              </label>
              <p class="help-text small">
                Enable this if the exam should be available only for paid subscribers.
              </p>
            </div>
            <div class="form-group">
              <label>Max Violations (1-10)</label>
              <input 
                type="number" 
                formControlName="maxViolations" 
                class="form-input" 
                min="1" 
                max="10">
              <p class="help-text small">
                Exam will be auto-submitted when this limit is reached.
              </p>
            </div>
          </div>
        </div>

        <div class="form-section card">
          <h2>Questions</h2>
          
          <div class="file-upload-section card-glass">
            <h3>Upload Questions from File (PDF or Excel)</h3>
            <p class="help-text">
              Upload a PDF or Excel file containing questions. Questions and options will be automatically randomized for each student.
            </p>
            
            <input 
              type="file" 
              id="questionFile" 
              class="file-input" 
              accept=".pdf,.xlsx,.xls"
              (change)="onFileSelected($event)">
            
            <label for="questionFile" class="file-upload-label">
              <span class="file-upload-button">Browse Files</span>
            </label>
            
            <div *ngIf="selectedFile" class="file-info">
              <span>📄 {{ selectedFile.name }} ({{ formatFileSize(selectedFile.size) }})</span>
              <button type="button" class="btn btn-secondary btn-small" (click)="clearFile()">Remove</button>
            </div>
            
            <div *ngIf="uploadingFile" class="upload-status">
              Uploading and parsing questions...
            </div>
            
            <div class="file-format-info">
              <strong>File Format Guidelines:</strong><br>
              <strong>Excel (.xlsx, .xls):</strong> Column 1 = Question, Columns 2-5 = Options, Column 6 = Correct Answer, Column 7 = Marks<br>
              <strong>PDF:</strong> Questions should be formatted as "Q1. Question text..." with options as "A) Option 1", "B) Option 2", etc.
            </div>
          </div>

          <div class="form-row">
            <div class="form-group">
              <label>Number of Questions to Use</label>
              <input 
                type="number" 
                formControlName="questionLimit" 
                class="form-input" 
                min="1">
              <p class="help-text small">
                Optional. If set, only this many questions will be imported from the file. Leave empty to use all parsed questions.
              </p>
            </div>
          </div>

          <div class="divider">
            <span>OR</span>
          </div>

          <p class="help-text">Add questions manually below:</p>
          
          <div formArrayName="questions">
            <div *ngFor="let question of questions.controls; let i = index" [formGroupName]="i" class="question-item card">
              <h3>Question {{ i + 1 }}</h3>
              <div class="form-group">
                <label>Question Text</label>
                <textarea formControlName="questionText" class="form-input" rows="2"></textarea>
              </div>
              <div class="form-group">
                <label>Type</label>
                <select formControlName="questionType" class="form-input">
                  <option value="mcq">MCQ</option>
                  <option value="text">Text</option>
                </select>
              </div>
              <div class="form-group">
                <label>Marks</label>
                <input type="number" formControlName="marks" class="form-input">
              </div>
              <div *ngIf="question.get('questionType')?.value === 'mcq'" class="form-group">
                <label>Options (one per line)</label>
                <textarea formControlName="options" class="form-input" rows="4" placeholder="Option 1&#10;Option 2&#10;Option 3&#10;Option 4"></textarea>
                <label>Correct Answer</label>
                <input formControlName="correctAnswer" class="form-input" placeholder="Correct option">
              </div>
              <button type="button" class="btn btn-secondary" (click)="removeQuestion(i)">Remove</button>
            </div>
          </div>
          <button type="button" class="btn btn-outline btn-small" (click)="addQuestion()">+ Add Question</button>
        </div>

        <button type="submit" class="btn btn-primary btn-large" [disabled]="examForm.invalid || saving">
          {{ saving ? 'Creating...' : 'Create Exam' }}
        </button>
      </form>
    </div>
  `,
  styles: [`
    .create-exam-container {
      max-width: 1100px;
      margin: 0 auto;
      padding: 32px 20px 40px;
      background: var(--background);
    }

    header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 32px;
    }

    .form-section {
      margin-bottom: 32px;
    }

    .form-row {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
      gap: 16px;
    }

    .form-row-inline {
      align-items: flex-start;
    }

    .question-item h3 {
      margin-bottom: 16px;
      color: var(--text-primary);
    }

    .btn-large {
      width: 100%;
      padding: 16px;
      font-size: 16px;
      margin-top: 8px;
    }

    .file-upload-section {
      margin-bottom: 24px;
      padding: 24px;
    }

    .help-text {
      color: var(--text-secondary);
      margin-bottom: 16px;
      font-size: 13px;
    }

    .help-text.small {
      margin-bottom: 0;
    }

    .file-input {
      display: none;
    }

    .file-upload-label {
      display: block;
      cursor: pointer;
    }

    .file-upload-button {
      display: inline-block;
      padding: 10px 20px;
      background: var(--royal-blue-gradient);
      color: var(--text-on-primary);
      border-radius: 999px;
      cursor: pointer;
      font-size: 14px;
      font-weight: 600;
      letter-spacing: 0.03em;
      text-transform: uppercase;
      box-shadow: 0 4px 12px rgba(30, 64, 175, 0.35);
      transition: all 0.2s;
    }

    .file-upload-button:hover {
      transform: translateY(-1px);
      box-shadow: 0 6px 16px rgba(30, 64, 175, 0.45);
    }

    .file-info {
      margin-top: 12px;
      display: flex;
      align-items: center;
      gap: 10px;
      padding: 10px 12px;
      background: var(--surface);
      border-radius: 10px;
      border: 1px dashed var(--border-color);
    }

    .upload-status {
      margin-top: 10px;
      padding: 10px 12px;
      background: rgba(59, 130, 246, 0.06);
      border-radius: 10px;
      color: var(--royal-blue-light);
      font-size: 13px;
    }

    .file-format-info {
      margin-top: 16px;
      padding: 16px;
      background: var(--surface);
      border-radius: 12px;
      font-size: 13px;
      line-height: 1.6;
    }

    .divider {
      text-align: center;
      margin: 24px 0;
      position: relative;
    }

    .divider::before {
      content: '';
      position: absolute;
      left: 0;
      top: 50%;
      width: 100%;
      height: 1px;
      background: var(--border-color);
    }

    .divider span {
      background: var(--background);
      padding: 0 15px;
      position: relative;
      color: var(--text-secondary);
    }

    .btn-small {
      padding: 8px 16px;
      font-size: 13px;
    }

    .switch-label {
      display: flex;
      align-items: center;
      justify-content: space-between;
      gap: 12px;
      font-size: 14px;
      color: var(--text-primary);
      font-weight: 500;
    }

    .switch {
      position: relative;
      display: inline-block;
      width: 46px;
      height: 24px;
    }

    .switch input {
      opacity: 0;
      width: 0;
      height: 0;
    }

    .slider {
      position: absolute;
      cursor: pointer;
      top: 0;
      left: 0;
      right: 0;
      bottom: 0;
      background-color: var(--border-color);
      transition: .2s;
      border-radius: 999px;
    }

    .slider:before {
      position: absolute;
      content: '';
      height: 18px;
      width: 18px;
      left: 3px;
      top: 3px;
      background-color: white;
      transition: .2s;
      border-radius: 50%;
      box-shadow: 0 1px 3px rgba(15, 23, 42, 0.3);
    }

    .switch input:checked + .slider {
      background: var(--royal-blue-gradient);
    }

    .switch input:checked + .slider:before {
      transform: translateX(22px);
    }
  `]
})
export class CreateExamComponent {
  examForm: FormGroup;
  saving = false;
  selectedFile: File | null = null;
  uploadingFile = false;

  constructor(
    private fb: FormBuilder,
    private apiService: ApiService,
    private router: Router,
    public authService: AuthService,
    public themeService: ThemeService
  ) {
    this.examForm = this.fb.group({
      title: ['', Validators.required],
      description: [''],
      duration: [60, [Validators.required, Validators.min(1)]],
      totalMarks: [100, [Validators.required, Validators.min(1)]],
      passingMarks: [40, [Validators.required, Validators.min(0)]],
      startTime: ['', Validators.required],
      endTime: ['', Validators.required],
      isPaid: [false],
      maxViolations: [3, [Validators.required, Validators.min(1), Validators.max(10)]],
      questionLimit: [null, [Validators.min(1)]],
      questions: this.fb.array([this.createQuestionForm()])
    });
  }

  get questions(): FormArray {
    return this.examForm.get('questions') as FormArray;
  }

  createQuestionForm(): FormGroup {
    return this.fb.group({
      questionText: ['', Validators.required],
      questionType: ['mcq', Validators.required],
      options: [''],
      correctAnswer: [''],
      marks: [1, Validators.required]
    });
  }

  addQuestion(): void {
    this.questions.push(this.createQuestionForm());
  }

  removeQuestion(index: number): void {
    if (this.questions.length > 1) {
      this.questions.removeAt(index);
    }
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      const file = input.files[0];
      const extension = file.name.split('.').pop()?.toLowerCase();
      
      if (extension && (extension === 'pdf' || extension === 'xlsx' || extension === 'xls')) {
        this.selectedFile = file;
      } else {
        alert('Please select a PDF or Excel file (.pdf, .xlsx, .xls)');
        input.value = '';
      }
    }
  }

  clearFile(): void {
    this.selectedFile = null;
    const fileInput = document.getElementById('questionFile') as HTMLInputElement;
    if (fileInput) {
      fileInput.value = '';
    }
  }

  formatFileSize(bytes: number): string {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i];
  }

  onSubmit(): void {
    if (this.examForm.valid) {
      this.saving = true;
      const formValue = this.examForm.value;
      
      const examRequest: CreateExamRequest = {
        title: formValue.title,
        description: formValue.description,
        duration: formValue.duration,
        totalMarks: formValue.totalMarks,
        passingMarks: formValue.passingMarks,
        startTime: new Date(formValue.startTime).toISOString(),
        endTime: new Date(formValue.endTime).toISOString(),
        isPaid: formValue.isPaid,
        maxViolations: formValue.maxViolations
      };

      this.apiService.createExam(examRequest).subscribe({
        next: (response) => {
          if (response.success && response.data) {
            const examId = response.data.id;
            
            // If file is selected, upload questions from file
            if (this.selectedFile) {
              this.uploadQuestionsFromFile(examId!);
            } else {
              // Otherwise, add questions manually
              this.addQuestions(examId!, formValue.questions);
            }
          }
        },
        error: (error) => {
          const msg = error.error?.message || error.message || 'Error creating exam';
          alert(msg);
          this.saving = false;
        }
      });
    }
  }

  private uploadQuestionsFromFile(examId: string): void {
    if (!this.selectedFile) return;
    
    this.uploadingFile = true;
    const questionLimit = this.examForm.value.questionLimit as number | null;
    this.apiService.uploadQuestions(examId, this.selectedFile, questionLimit || undefined).subscribe({
      next: (response) => {
        this.uploadingFile = false;
        if (response.success) {
          const questionCount = response.data?.questionCount || 0;
          alert(`Exam created successfully! ${questionCount} questions uploaded from file. Questions and options will be randomized for each student.`);
          this.router.navigate(['/teacher/dashboard']);
        } else {
          alert(response.message || 'Error uploading questions');
          this.saving = false;
        }
      },
      error: (error) => {
        this.uploadingFile = false;
        this.saving = false;
        alert(error.error?.message || error.message || 'Error uploading questions from file');
      }
    });
  }

  private addQuestions(examId: string, questions: any[]): void {
    let completed = 0;
    questions.forEach((q, index) => {
      const options = q.options ? q.options.split('\n').filter((o: string) => o.trim()) : [];
      const questionRequest: CreateQuestionRequest = {
        questionText: q.questionText,
        questionType: q.questionType,
        options: options,
        correctAnswer: q.correctAnswer,
        marks: q.marks
      };

      this.apiService.addQuestion(examId, questionRequest).subscribe({
        next: () => {
          completed++;
          if (completed === questions.length) {
            alert('Exam created successfully!');
            this.router.navigate(['/teacher/dashboard']);
          }
        },
        error: () => {
          this.saving = false;
        }
      });
    });
  }
}

