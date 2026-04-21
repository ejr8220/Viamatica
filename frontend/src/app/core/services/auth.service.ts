import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap, BehaviorSubject } from 'rxjs';
import { Router } from '@angular/router';
import { TokenStorageService } from './token-storage.service';
import { LoginRequest, AuthResponse, ForgotPasswordRequest, ForgotPasswordResponse } from '../models';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly apiUrl = `${environment.apiUrl}/api/auth`;
  private readonly isAuthenticatedSubject: BehaviorSubject<boolean>;
  public readonly isAuthenticated$;

  constructor(
    private http: HttpClient,
    private tokenStorage: TokenStorageService,
    private router: Router
  ) {
    this.isAuthenticatedSubject = new BehaviorSubject<boolean>(this.hasValidToken());
    this.isAuthenticated$ = this.isAuthenticatedSubject.asObservable();
  }

  login(credentials: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, credentials).pipe(
      tap((response) => {
        this.tokenStorage.saveToken(response.accessToken);
        this.tokenStorage.saveUserInfo({
          userName: response.userName,
          role: response.role,
          expiresAtUtc: new Date(response.expiresAtUtc),
        });
        this.isAuthenticatedSubject.next(true);
      })
    );
  }

  forgotPassword(request: ForgotPasswordRequest): Observable<ForgotPasswordResponse> {
    return this.http.post<ForgotPasswordResponse>(`${this.apiUrl}/forgot-password`, request);
  }

  logout(): void {
    this.tokenStorage.clear();
    this.isAuthenticatedSubject.next(false);
    this.router.navigate(['/auth/login']);
  }

  isLoggedIn(): boolean {
    return this.hasValidToken();
  }

  getUserRole(): string | null {
    const userInfo = this.tokenStorage.getUserInfo();
    return userInfo?.role || null;
  }

  getUserName(): string | null {
    const userInfo = this.tokenStorage.getUserInfo();
    return userInfo?.userName || null;
  }

  hasRole(role: string): boolean {
    return this.getUserRole() === role;
  }

  private hasValidToken(): boolean {
    const token = this.tokenStorage.getToken();
    return !!token && !this.tokenStorage.isTokenExpired();
  }
}
