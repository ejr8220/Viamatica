import { Component, inject } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';

import { AuthService, NotificationService } from '../../../../core/services';

@Component({
  selector: 'app-login-page',
  standalone: false,
  templateUrl: './login-page.html',
  styleUrl: './login-page.scss',
})
export class LoginPage {
  submitting = false;
  readonly currentYear = new Date().getFullYear();
  private readonly fb = inject(FormBuilder);

  readonly form = this.fb.nonNullable.group({
    userNameOrEmail: ['', [Validators.required]],
    password: ['', [Validators.required, Validators.minLength(8)]],
  });

  constructor(
    private readonly authService: AuthService,
    private readonly notificationService: NotificationService,
    private readonly router: Router
  ) {}

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.submitting = true;
    this.authService.login(this.form.getRawValue()).subscribe({
      next: (response) => {
        this.notificationService.success(`Bienvenido ${response.userName}`);
        this.router.navigate([response.role === 'Administrador' ? '/dashboard' : '/welcome']);
      },
      error: () => {
        this.submitting = false;
      },
      complete: () => {
        this.submitting = false;
      },
    });
  }
}
