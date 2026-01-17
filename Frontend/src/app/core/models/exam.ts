export interface Exam {
    id?: string;
    title: string;
    description: string;
    teacherId?: string;
    duration: number; // minutes
    totalMarks: number;
    passingMarks: number;
    startTime: Date;
    endTime: Date;
    isPaid: boolean;
    maxViolations: number;
    assignedStudents: string[];
    createdAt?: Date;
    questions?: any[];
}

export interface Question {
    id?: string;
    questionText: string;
    questionType: 'mcq' | 'subjective';
    options: string[];
    correctAnswer: string;
    marks: number;
    order: number;
}
