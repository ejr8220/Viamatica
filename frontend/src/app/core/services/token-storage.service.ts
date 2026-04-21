import { Injectable } from '@angular/core';

const TOKEN_KEY = 'access_token';
const USER_INFO_KEY = 'user_info';

@Injectable({
  providedIn: 'root',
})
export class TokenStorageService {
  saveToken(token: string): void {
    localStorage.setItem(TOKEN_KEY, token);
  }

  getToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  }

  removeToken(): void {
    localStorage.removeItem(TOKEN_KEY);
  }

  saveUserInfo(userInfo: { userName: string; role: string; expiresAtUtc: Date }): void {
    localStorage.setItem(USER_INFO_KEY, JSON.stringify(userInfo));
  }

  getUserInfo(): { userName: string; role: string; expiresAtUtc: Date } | null {
    const data = localStorage.getItem(USER_INFO_KEY);
    return data ? JSON.parse(data) : null;
  }

  removeUserInfo(): void {
    localStorage.removeItem(USER_INFO_KEY);
  }

  clear(): void {
    this.removeToken();
    this.removeUserInfo();
  }

  isTokenExpired(): boolean {
    const userInfo = this.getUserInfo();
    if (!userInfo || !userInfo.expiresAtUtc) {
      return true;
    }
    const expiresAt = new Date(userInfo.expiresAtUtc);
    return expiresAt.getTime() < Date.now();
  }
}
