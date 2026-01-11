export interface User {
  id?: string;
  name: string;
  email: string;
  role: 'student' | 'teacher' | 'admin';
  subscriptionType: 'free' | 'paid';
  isVerified: boolean;
  isActive: boolean;
}

export interface LoginResponse {
  token: string;
  userId: string;
  name: string;
  email: string;
  role: string;
  subscriptionType: string;
}

export interface RegisterRequest {
  name: string;
  email: string;
  password: string;
  role: 'student' | 'teacher';
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface VerifyOtpRequest {
  email: string;
  otpCode: string;
}

export interface ForgotPasswordRequest {
  email: string;
}

export interface ResetPasswordRequest {
  email: string;
  otpCode: string;
  newPassword: string;
}

