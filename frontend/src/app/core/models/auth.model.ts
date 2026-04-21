export interface LoginRequest {
  userNameOrEmail: string;
  password: string;
}

export interface AuthResponse {
  accessToken: string;
  tokenType: string;
  expiresAtUtc: string;
  userName: string;
  role: string;
}

export interface ForgotPasswordRequest {
  userNameOrEmail: string;
  identification: string;
  newPassword: string;
}

export interface ForgotPasswordResponse {
  message: string;
}

export interface UserInfo {
  userName: string;
  role: string;
  expiresAtUtc: Date;
}
