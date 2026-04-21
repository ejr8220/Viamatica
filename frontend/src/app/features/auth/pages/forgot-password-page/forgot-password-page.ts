import { Component, inject } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';

import { AuthService, NotificationService } from '../../../../core/services';

@Component({
  selector: 'app-forgot-password-page',
  standalone: false,
  templateUrl: './forgot-password-page.html',
  styleUrl: './forgot-password-page.scss',
})
export class ForgotPasswordPage {
  submitting = false;
  readonly currentYear = new Date().getFullYear();
  private readonly fb = inject(FormBuilder);

  readonly form = this.fb.nonNullable.group({
    userNameOrEmail: ['', [Validators.required]],
    identification: ['', [Validators.required, Validators.pattern(/^\d{10,13}$/)]],
    newPassword: ['', [Validators.required, Validators.minLength(8)]],
    confirmPassword: ['', [Validators.required, Validators.minLength(8)]],
  });

  constructor(
    private readonly authService: AuthService,
    private readonly notificationService: NotificationService,
    private readonly router: Router
  ) {}

  get passwordsDoNotMatch(): boolean {
    const { newPassword, confirmPassword } = this.form.getRawValue();
    return !!newPassword && !!confirmPassword && newPassword !== confirmPassword;
  }

  submit(): void {
    if (this.form.invalid || this.passwordsDoNotMatch) {
      this.form.markAllAsTouched();
      return;
    }

    const { confirmPassword: _, ...payload } = this.form.getRawValue();
    this.submitting = true;

    this.authService.forgotPassword(payload).subscribe({
      next: (response) => {
        this.notificationService.success(response.message);
        this.router.navigate(['/auth/login']);
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
