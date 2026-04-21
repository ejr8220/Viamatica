import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { ActiveUserReport, User } from '../../../core/models';
import { environment } from '../../../../environments/environment';

interface UserApi {
  userId: number;
  userName: string;
  identification: string;
  email: string;
  roleId: number;
  roleName: string;
  statusId: string;
  statusDescription: string;
  approved: boolean;
  dateApproval?: string | null;
}

interface ActiveUserReportApi {
  userId: number;
  userName: string;
  identification: string;
  email: string;
  roleName: string;
  statusDescription: string;
}

interface CreateUserApi {
  userName: string;
  identification: string;
  email: string;
  password: string;
  roleId: number;
}

interface UpdateUserApi {
  userName: string;
  identification: string;
  email: string;
  password?: string;
  roleId: number;
  active: boolean;
}

export interface UserViewModel extends User {
  roleId: number;
  approved: boolean;
  active: boolean;
}

export interface ActiveUserReportViewModel extends ActiveUserReport {
  status: string;
}

export interface UserFormValue {
  userName: string;
  identification: string;
  email: string;
  password?: string;
  role: string;
  status: string;
}

@Injectable({
  providedIn: 'root',
})
export class UsersService {
  private readonly apiUrl = `${environment.apiUrl}/api/users`;

  constructor(private readonly http: HttpClient) {}

  getAll(pendingOnly = false): Observable<UserViewModel[]> {
    const params = new HttpParams().set('pendingOnly', pendingOnly);
    return this.http.get<UserApi[]>(this.apiUrl, { params }).pipe(map((items) => items.map((item) => this.toViewModel(item))));
  }

  getActiveReport(): Observable<ActiveUserReportViewModel[]> {
    return this.http
      .get<ActiveUserReportApi[]>(`${this.apiUrl}/active-report`)
      .pipe(map((items) => items.map((item) => this.toActiveReport(item))));
  }

  create(value: UserFormValue): Observable<UserViewModel> {
    return this.http.post<UserApi>(this.apiUrl, this.toCreateRequest(value)).pipe(map((item) => this.toViewModel(item)));
  }

  update(userId: number, value: UserFormValue): Observable<UserViewModel> {
    return this.http.put<UserApi>(`${this.apiUrl}/${userId}`, this.toUpdateRequest(value)).pipe(map((item) => this.toViewModel(item)));
  }

  approve(userId: number): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${userId}/approve`, {});
  }

  delete(userId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${userId}`);
  }

  private toViewModel(item: UserApi): UserViewModel {
    return {
      userId: item.userId,
      userName: item.userName,
      identification: item.identification,
      email: item.email,
      firstName: '',
      lastName: '',
      role: item.roleName,
      status: item.statusDescription,
      createdAt: item.dateApproval ?? '',
      roleId: item.roleId,
      approved: item.approved,
      active: item.statusId === 'ACT' || item.statusId === 'APR',
    };
  }

  private toActiveReport(item: ActiveUserReportApi): ActiveUserReportViewModel {
    return {
      userId: item.userId,
      userName: item.userName,
      identification: item.identification,
      fullName: item.userName,
      email: item.email,
      role: item.roleName,
      activeSessionCount: 1,
      status: item.statusDescription,
    };
  }

  private toCreateRequest(value: UserFormValue): CreateUserApi {
    return {
      userName: value.userName.trim(),
      identification: value.identification.trim(),
      email: value.email.trim(),
      password: value.password?.trim() ?? '',
      roleId: this.roleToId(value.role),
    };
  }

  private toUpdateRequest(value: UserFormValue): UpdateUserApi {
    const request: UpdateUserApi = {
      userName: value.userName.trim(),
      identification: value.identification.trim(),
      email: value.email.trim(),
      roleId: this.roleToId(value.role),
      active: value.status.toLowerCase() === 'activo',
    };

    if (value.password?.trim()) {
      request.password = value.password.trim();
    }

    return request;
  }

  private roleToId(role: string): number {
    switch (role) {
      case 'Administrador':
      case 'Administrator':
        return 1;
      case 'Cajero':
      case 'Cashier':
        return 3;
      case 'Gestor':
      default:
        return 2;
    }
  }
}
