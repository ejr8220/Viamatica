import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, finalize } from 'rxjs/operators';
import { TokenStorageService } from '../services/token-storage.service';
import { LoadingService } from '../services/loading.service';
import { NotificationService } from '../services/notification.service';
import { Router } from '@angular/router';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(
    private tokenStorage: TokenStorageService,
    private loadingService: LoadingService,
    private notificationService: NotificationService,
    private router: Router
  ) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const isAuthRequest = req.url.includes('/auth/login') || req.url.includes('/auth/forgot-password');

    this.loadingService.show();

    const token = this.tokenStorage.getToken();
    let authReq = req;

    if (!isAuthRequest && token && this.tokenStorage.isTokenExpired()) {
      this.loadingService.hide();
      this.tokenStorage.clear();
      this.notificationService.warning('Su sesión expiró. Inicie sesión nuevamente.');
      void this.router.navigate(['/auth/login']);
      return throwError(() => new Error('Session expired'));
    }

    if (token && !isAuthRequest) {
      authReq = req.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`,
        },
      });
    }

    return next.handle(authReq).pipe(
      catchError((error: HttpErrorResponse) => {
        let errorMessage = 'Ha ocurrido un error inesperado';

        if (error.error instanceof ErrorEvent) {
          // Client-side error
          errorMessage = `Error: ${error.error.message}`;
        } else {
          // Server-side error
          switch (error.status) {
            case 401:
              errorMessage = 'No autorizado. Por favor, inicie sesión nuevamente.';
              this.tokenStorage.clear();
              void this.router.navigate(['/auth/login']);
              break;
            case 403:
              errorMessage = 'No tiene permisos para realizar esta acción.';
              break;
            case 404:
              errorMessage = 'Recurso no encontrado.';
              break;
            case 400:
              errorMessage = error.error?.message || 'Datos inválidos.';
              break;
            case 500:
              errorMessage = 'Error interno del servidor.';
              break;
            default:
              errorMessage = error.error?.message || errorMessage;
          }
        }

        this.notificationService.error(errorMessage);
        return throwError(() => error);
      }),
      finalize(() => {
        this.loadingService.hide();
      })
    );
  }
}
