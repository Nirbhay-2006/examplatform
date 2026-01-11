import { Component, OnInit, effect } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ThemeService } from './services/theme.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, CommonModule],
  template: `
    <div class="app-container" [class.dark-theme]="themeService.isDarkMode()">
      <router-outlet></router-outlet>
    </div>
  `,
  styles: [`
    .app-container {
      min-height: 100vh;
      transition: background-color 0.3s ease, color 0.3s ease;
    }
  `]
})
export class AppComponent implements OnInit {
  constructor(public themeService: ThemeService) {
    // React to theme changes
    effect(() => {
      this.themeService.currentTheme();
    });
  }

  ngOnInit(): void {
    // Apply initial theme
    this.themeService.applyTheme(this.themeService.currentTheme());
  }
}
