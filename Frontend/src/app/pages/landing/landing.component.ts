import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-landing',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="landing-container">
      <!-- Animated Background -->
      <div class="bg-animation">
        <div class="bg-circle circle-1"></div>
        <div class="bg-circle circle-2"></div>
        <div class="bg-circle circle-3"></div>
      </div>

      <header class="header fade-in">
        <div class="header-content">
          <div class="logo-section">
            <div class="logo-icon">📚</div>
        <h1 class="logo">ExamPlatform</h1>
          </div>
        <div class="nav-buttons">
            <button class="btn btn-glass" (click)="goToLogin()">Login</button>
            <button class="btn btn-primary" (click)="goToRegister()">Get Started</button>
          </div>
        </div>
      </header>
      
      <main class="hero">
        <div class="hero-content fade-in-up">
          <div class="hero-badge">
            <span>✨ Premium Exam Platform</span>
          </div>
          <h1 class="hero-title">
            Secure Online
            <span class="gradient-text">Examinations</span>
            Made Simple
          </h1>
          <p class="hero-description">
            Experience the future of online testing with our advanced anti-cheat system, 
            real-time monitoring, and seamless user experience.
          </p>
        <div class="hero-buttons">
            <button class="btn btn-primary btn-large" (click)="goToRegister()">
              <span>Start Free Trial</span>
              <span>→</span>
            </button>
            <button class="btn btn-glass btn-large" (click)="goToLogin()">
              Sign In
            </button>
          </div>
          
          <div class="hero-features">
            <div class="feature-item">
              <div class="feature-icon">🔒</div>
              <span>Advanced Security</span>
            </div>
            <div class="feature-item">
              <div class="feature-icon">⚡</div>
              <span>Real-time Monitoring</span>
            </div>
            <div class="feature-item">
              <div class="feature-icon">📊</div>
              <span>Analytics Dashboard</span>
            </div>
          </div>
        </div>

        <div class="hero-visual float">
          <div class="visual-card card-glass">
            <div class="card-header">
              <div class="card-dots">
                <span></span><span></span><span></span>
              </div>
            </div>
            <div class="card-content">
              <div class="stat-item">
                <div class="stat-label">Active Exams</div>
                <div class="stat-value">1,234</div>
              </div>
              <div class="stat-item">
                <div class="stat-label">Students</div>
                <div class="stat-value">5,678</div>
              </div>
            </div>
          </div>
        </div>
      </main>
    </div>
  `,
  styles: [`
    .landing-container {
      min-height: 100vh;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
      position: relative;
      overflow: hidden;
    }

    .bg-animation {
      position: absolute;
      width: 100%;
      height: 100%;
      top: 0;
      left: 0;
      z-index: 0;
    }

    .bg-circle {
      position: absolute;
      border-radius: 50%;
      background: rgba(255, 255, 255, 0.1);
      animation: float 20s infinite ease-in-out;
    }

    .circle-1 {
      width: 300px;
      height: 300px;
      top: -100px;
      left: -100px;
      animation-delay: 0s;
    }

    .circle-2 {
      width: 200px;
      height: 200px;
      bottom: -50px;
      right: -50px;
      animation-delay: 5s;
    }

    .circle-3 {
      width: 150px;
      height: 150px;
      top: 50%;
      right: 10%;
      animation-delay: 10s;
    }

    .header {
      position: relative;
      z-index: 10;
      padding: 24px 32px;
    }

    .header-content {
      max-width: 1400px;
      margin: 0 auto;
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    .logo-section {
      display: flex;
      align-items: center;
      gap: 12px;
    }

    .logo-icon {
      font-size: 32px;
      animation: pulse 2s infinite;
    }

    .logo {
      font-size: 28px;
      font-weight: 800;
      margin: 0;
      background: linear-gradient(135deg, #fff 0%, rgba(255, 255, 255, 0.8) 100%);
      -webkit-background-clip: text;
      -webkit-text-fill-color: transparent;
      background-clip: text;
    }

    .nav-buttons {
      display: flex;
      gap: 12px;
    }

    .btn-glass {
      background: rgba(255, 255, 255, 0.2);
      backdrop-filter: blur(10px);
      -webkit-backdrop-filter: blur(10px);
      border: 1px solid rgba(255, 255, 255, 0.3);
      color: white;
    }

    .btn-glass:hover {
      background: rgba(255, 255, 255, 0.3);
      transform: translateY(-2px);
    }

    .hero {
      position: relative;
      z-index: 10;
      max-width: 1400px;
      margin: 0 auto;
      padding: 80px 32px;
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 60px;
      align-items: center;
    }

    .hero-content {
      text-align: left;
    }

    .hero-badge {
      display: inline-block;
      padding: 8px 16px;
      background: rgba(255, 255, 255, 0.2);
      backdrop-filter: blur(10px);
      border-radius: 20px;
      margin-bottom: 24px;
      font-size: 14px;
      font-weight: 600;
    }

    .hero-title {
      font-size: 64px;
      font-weight: 800;
      line-height: 1.2;
      margin-bottom: 24px;
      letter-spacing: -1px;
    }

    .gradient-text {
      background: linear-gradient(135deg, #fbbf24 0%, #f59e0b 100%);
      -webkit-background-clip: text;
      -webkit-text-fill-color: transparent;
      background-clip: text;
    }

    .hero-description {
      font-size: 20px;
      line-height: 1.6;
      margin-bottom: 40px;
      opacity: 0.9;
      max-width: 600px;
    }

    .hero-buttons {
      display: flex;
      gap: 16px;
      margin-bottom: 48px;
      flex-wrap: wrap;
    }

    .hero-features {
      display: flex;
      gap: 32px;
      flex-wrap: wrap;
    }

    .feature-item {
      display: flex;
      align-items: center;
      gap: 8px;
      font-size: 16px;
      font-weight: 500;
    }

    .feature-icon {
      font-size: 24px;
    }

    .hero-visual {
      display: flex;
      justify-content: center;
      align-items: center;
    }

    .visual-card {
      width: 100%;
      max-width: 400px;
      padding: 24px;
    }

    .card-header {
      margin-bottom: 24px;
    }

    .card-dots {
      display: flex;
      gap: 8px;
    }

    .card-dots span {
      width: 12px;
      height: 12px;
      border-radius: 50%;
      background: rgba(255, 255, 255, 0.3);
    }

    .stat-item {
      margin-bottom: 20px;
    }

    .stat-label {
      font-size: 14px;
      opacity: 0.8;
      margin-bottom: 4px;
    }

    .stat-value {
      font-size: 32px;
      font-weight: 700;
    }

    @media (max-width: 968px) {
      .hero {
        grid-template-columns: 1fr;
        text-align: center;
      }

      .hero-title {
        font-size: 48px;
      }

      .hero-description {
        font-size: 18px;
      }
    }

    @media (max-width: 640px) {
      .hero-title {
        font-size: 36px;
      }

      .header-content {
        flex-direction: column;
        gap: 16px;
      }
    }
  `]
})
export class LandingComponent {
  constructor(private router: Router) {}

  goToLogin() {
    this.router.navigate(['/login']);
  }

  goToRegister() {
    this.router.navigate(['/register']);
  }
}