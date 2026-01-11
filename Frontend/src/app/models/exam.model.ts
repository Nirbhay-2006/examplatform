export interface Exam {
  id?: string;
  title: string;
  description: string;
  teacherId: string;
  duration: number; // in minutes
  totalMarks: number;
  passingMarks: number;
  startTime: string;
  endTime: string;
  isPaid: boolean;
  isPublished: boolean;
  maxViolations: number;
  createdAt?: string;
  assignedStudents: string[];
}

export interface Question {
  id?: string;
  examId: string;
  questionText: string;
  questionType: 'mcq' | 'text';
  options: string[];
  correctAnswer?: string;
  marks: number;
  order: number;
}

export interface Answer {
  questionId: string;
  answerText: string;
  isCorrect?: boolean;
  marksObtained?: number;
}

export interface Response {
  id?: string;
  examId: string;
  studentId: string;
  answers: Answer[];
  submittedAt?: string;
  status: 'in-progress' | 'submitted';
  violationCount: number;
  isAutoSubmitted?: boolean;
}

export interface Violation {
  id?: string;
  examId: string;
  studentId: string;
  violationType: string;
  timestamp?: string;
}

export interface CreateExamRequest {
  title: string;
  description: string;
  duration: number;
  totalMarks: number;
  passingMarks: number;
  startTime: string;
  endTime: string;
  isPaid: boolean;
  maxViolations: number;
}

export interface CreateQuestionRequest {
  questionText: string;
  questionType: 'mcq' | 'text';
  options: string[];
  correctAnswer: string;
  marks: number;
}

export interface SubmitAnswerRequest {
  questionId: string;
  answer: string;
}

export interface ViolationRequest {
  examId: string;
  violationType: string;
}

