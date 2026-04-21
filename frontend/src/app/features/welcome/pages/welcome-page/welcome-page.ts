import { Component, OnInit } from '@angular/core';

import { AuthService } from '../../../../core/services';
import { WelcomeService, WelcomeShortcut } from '../../services/welcome.service';

@Component({
  selector: 'app-welcome-page',
  standalone: false,
  templateUrl: './welcome-page.html',
  styleUrl: './welcome-page.scss',
})
export class WelcomePage implements OnInit {
  shortcuts: WelcomeShortcut[] = [];
  userName = '';
  userRole = '';
  loading = false;

  constructor(
    private readonly authService: AuthService,
    private readonly welcomeService: WelcomeService
  ) {}

  ngOnInit(): void {
    this.userName = this.authService.getUserName() ?? 'Usuario';
    this.userRole = this.authService.getUserRole() ?? 'Sin rol';
    this.loading = true;
    this.welcomeService.getShortcuts().subscribe({
      next: (items) => {
        this.shortcuts = items;
      },
      complete: () => {
        this.loading = false;
      },
    });
  }
}
