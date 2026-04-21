import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { AuthService } from '../services';

export const roleGuard: CanActivateFn = (route) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  const requiredRoles = route.data['roles'] as string[];

  if (!authService.isLoggedIn()) {
    router.navigate(['/auth/login']);
    return false;
  }

  const userRole = authService.getUserRole();
  if (requiredRoles && userRole && requiredRoles.includes(userRole)) {
    return true;
  }

  router.navigate(['/welcome']);
  return false;
};
