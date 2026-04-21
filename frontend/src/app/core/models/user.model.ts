export interface User {
  userId: number;
  userName: string;
  identification: string;
  email: string;
  firstName: string;
  lastName: string;
  role: string;
  status: string;
  createdAt: string;
  lastLogin?: string;
}

export interface CreateUserRequest {
  userName: string;
  identification: string;
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  role: string;
}

export interface UpdateUserRequest {
  userName?: string;
  identification?: string;
  email: string;
  firstName: string;
  lastName: string;
  role: string;
  status?: string;
}

export interface ActiveUserReport {
  userId: number;
  userName: string;
  identification?: string;
  fullName: string;
  email: string;
  role: string;
  lastLogin?: string;
  activeSessionCount: number;
}
