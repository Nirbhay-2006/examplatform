import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

export interface AntiCheatEvent {
  type: string;
  timestamp: Date;
  severity: 'low' | 'medium' | 'high' | 'critical';
  details?: any;
}

@Injectable({
  providedIn: 'root'
})
export class AntiCheatService {
  private violationCount = 0;
  private violations$ = new BehaviorSubject<AntiCheatEvent[]>([]);
  private isMonitoring = false;
  private monitoringIntervals: any[] = [];
  private lastActivityTime = Date.now();
  private suspiciousPatterns: any[] = [];

  constructor() {}

  startMonitoring(): void {
    if (this.isMonitoring) return;
    this.isMonitoring = true;
    this.lastActivityTime = Date.now();

    // Multiple tab/window detection
    this.monitorMultipleTabs();
    
    // Activity monitoring
    this.monitorActivity();
    
    // Screen sharing detection
    this.monitorScreenSharing();
    
    // Browser automation detection
    this.detectBrowserAutomation();
    
    // Virtual machine detection
    this.detectVirtualMachine();
    
    // Network monitoring
    this.monitorNetwork();
    
    // Answer pattern analysis
    this.monitorAnswerPatterns();
  }

  stopMonitoring(): void {
    this.isMonitoring = false;
    this.monitoringIntervals.forEach(interval => clearInterval(interval));
    this.monitoringIntervals = [];
  }

  getViolations(): Observable<AntiCheatEvent[]> {
    return this.violations$.asObservable();
  }

  reportViolation(type: string, severity: 'low' | 'medium' | 'high' | 'critical' = 'medium', details?: any): void {
    const event: AntiCheatEvent = {
      type,
      timestamp: new Date(),
      severity,
      details
    };

    const current = this.violations$.value;
    current.push(event);
    this.violations$.next([...current]);
    this.violationCount++;
  }

  private monitorMultipleTabs(): void {
    // Use localStorage to detect multiple tabs
    const tabId = Math.random().toString(36).substring(7);
    localStorage.setItem('examTabId', tabId);

    const checkTabs = setInterval(() => {
      const storedId = localStorage.getItem('examTabId');
      if (storedId && storedId !== tabId) {
        this.reportViolation('Multiple tabs detected', 'high', { storedId, currentId: tabId });
      }
    }, 1000);

    this.monitoringIntervals.push(checkTabs);

    // Listen for storage events (multiple tabs)
    window.addEventListener('storage', (e) => {
      if (e.key === 'examTabId' && e.newValue !== tabId) {
        this.reportViolation('Multiple tabs/windows detected', 'high');
      }
    });

    // Window focus check
    let focusCount = 0;
    window.addEventListener('focus', () => {
      focusCount++;
      if (focusCount > 1) {
        this.reportViolation('Multiple window focus detected', 'high');
      }
    });
  }

  private monitorActivity(): void {
    // Track mouse movements
    let lastMouseMove = Date.now();
    document.addEventListener('mousemove', () => {
      lastMouseMove = Date.now();
      this.lastActivityTime = Date.now();
    });

    // Track keyboard activity
    document.addEventListener('keydown', () => {
      this.lastActivityTime = Date.now();
    });

    // Check for inactivity (suspicious if too long)
    const activityCheck = setInterval(() => {
      const inactiveTime = Date.now() - this.lastActivityTime;
      if (inactiveTime > 30000) { // 30 seconds
        this.reportViolation('Extended inactivity detected', 'medium', { inactiveTime });
      }
    }, 5000);

    this.monitoringIntervals.push(activityCheck);
  }

  private monitorScreenSharing(): void {
    // Check for getDisplayMedia (screen sharing)
    if (navigator.mediaDevices && typeof navigator.mediaDevices.getDisplayMedia === 'function') {
      const originalGetDisplayMedia = navigator.mediaDevices.getDisplayMedia;
      (navigator.mediaDevices as any).getDisplayMedia = function() {
        (window as any).antiCheatService?.reportViolation('Screen sharing attempt', 'critical');
        return Promise.reject(new Error('Screen sharing not allowed during exam'));
      };
    }

    // Monitor for screen capture APIs
    if ((window as any).chrome?.desktopCapture) {
      this.reportViolation('Screen capture API detected', 'high');
    }
  }

  private detectBrowserAutomation(): void {
    // Detect Selenium/WebDriver
    if ((window as any).navigator.webdriver) {
      this.reportViolation('Browser automation detected (WebDriver)', 'critical');
    }

    // Detect Puppeteer
    if ((window as any).navigator.plugins.length === 0) {
      this.reportViolation('Suspicious browser configuration', 'medium');
    }

    // Detect headless browser
    if (!(window as any).navigator.plugins.length && !(window as any).navigator.languages.length) {
      this.reportViolation('Headless browser detected', 'critical');
    }

    // Check for automation tools
    const automationChecks = [
      () => (window as any).__selenium_unwrap__,
      () => (window as any).__webdriver_evaluate,
      () => (window as any).__driver_evaluate,
      () => (window as any).__selenium_evaluate,
      () => (window as any).__fxdriver_evaluate,
      () => (window as any).__driver_unwrap,
      () => (window as any).__fxdriver_unwrap,
      () => (window as any).__selenium_unwrap,
      () => (window as any).__webdriver_script_fn,
      () => (window as any).__webdriver_script_func,
      () => (window as any).__webdriver_script_fn,
      () => (window as any).__fxdriver_script_fn,
      () => (window as any).__driver_script_fn,
      () => (window as any).__selenium_script_fn,
      () => (window as any).__webdriver_script_func,
      () => (window as any).__selenium_script_func,
      () => (window as any).__fxdriver_script_func,
      () => (window as any).__driver_script_func
    ];

    automationChecks.forEach(check => {
      if (check()) {
        this.reportViolation('Browser automation tool detected', 'critical');
      }
    });
  }

  private detectVirtualMachine(): void {
    // Check for VM indicators
    const userAgent = navigator.userAgent.toLowerCase();
    const vmIndicators = ['virtualbox', 'vmware', 'qemu', 'xen', 'kvm', 'parallels'];
    
    vmIndicators.forEach(indicator => {
      if (userAgent.includes(indicator)) {
        this.reportViolation('Virtual machine detected', 'high', { indicator });
      }
    });

    // Check screen resolution (VMs often have unusual resolutions)
    if (screen.width < 1024 || screen.height < 768) {
      this.reportViolation('Unusual screen resolution', 'medium', { width: screen.width, height: screen.height });
    }
  }

  private monitorNetwork(): void {
    // Monitor for network disconnections (possible proxy/VPN)
    window.addEventListener('online', () => {
      this.reportViolation('Network reconnection detected', 'low');
    });

    window.addEventListener('offline', () => {
      this.reportViolation('Network disconnection detected', 'medium');
    });
  }

  private monitorAnswerPatterns(): void {
    // This will be called from the component when answers are submitted
    // Track timing patterns, answer patterns, etc.
  }

  analyzeAnswerPattern(answers: any[], timeSpent: number[]): void {
    // Detect suspicious patterns
    // 1. All answers submitted too quickly
    const avgTime = timeSpent.reduce((a, b) => a + b, 0) / timeSpent.length;
    if (avgTime < 2) { // Less than 2 seconds per question
      this.reportViolation('Suspicious answer timing pattern', 'high', { avgTime });
    }

    // 2. All answers are the same option
    if (answers.length > 5) {
      const uniqueAnswers = new Set(answers);
      if (uniqueAnswers.size === 1) {
        this.reportViolation('Suspicious answer pattern (all same)', 'medium');
      }
    }

    // 3. Answers submitted in perfect sequence without review
    const timeBetweenAnswers = timeSpent.slice(1).map((t, i) => t - timeSpent[i]);
    const consistentTiming = timeBetweenAnswers.every(t => Math.abs(t - timeBetweenAnswers[0]) < 1);
    if (consistentTiming && answers.length > 3) {
      this.reportViolation('Suspicious timing pattern (automated)', 'high');
    }
  }

  getBrowserFingerprint(): string {
    const canvas = document.createElement('canvas');
    const ctx = canvas.getContext('2d');
    ctx!.textBaseline = 'top';
    ctx!.font = '14px Arial';
    ctx!.fillText('Anti-cheat fingerprint', 2, 2);
    
    const fingerprint = [
      navigator.userAgent,
      navigator.language,
      screen.width + 'x' + screen.height,
      new Date().getTimezoneOffset(),
      canvas.toDataURL(),
      navigator.platform,
      navigator.hardwareConcurrency || 'unknown'
    ].join('|');

    return btoa(fingerprint);
  }

  getViolationCount(): number {
    return this.violationCount;
  }

  reset(): void {
    this.violationCount = 0;
    this.violations$.next([]);
    this.suspiciousPatterns = [];
  }
}

