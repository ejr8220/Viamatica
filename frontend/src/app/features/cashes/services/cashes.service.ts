import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { Cash } from '../../../core/models';
import { environment } from '../../../../environments/environment';

interface CashAssignmentApi {
  userId: number;
  userName: string;
}

interface CashSessionApi {
  cashSessionId: number;
  cashId: number;
  userId: number;
  userName: string;
  startedAt: string;
  endedAt?: string | null;
  isActive: boolean;
}

interface CashApi {
  cashId: number;
  cashDescription: string;
  active: string;
  assignedCashiers: CashAssignmentApi[];
  activeSession?: CashSessionApi | null;
}

interface UserApi {
  userId: number;
  userName: string;
  roleName: string;
  approved: boolean;
}

export interface CashAssignmentViewModel {
  userId: number;
  userName: string;
}

export interface CashSessionViewModel {
  cashSessionId: number;
  cashId: number;
  userId: number;
  userName: string;
  startedAt: string;
  endedAt?: string;
  isActive: boolean;
}

export interface CashViewModel extends Cash {
  activeFlag: string;
  assignedCashiers: CashAssignmentViewModel[];
  activeSession?: CashSessionViewModel;
}

export interface CashFormValue {
  description: string;
  active: boolean;
}

export interface CashierOption {
  userId: number;
  userName: string;
}

@Injectable({
  providedIn: 'root',
})
export class CashesService {
  private readonly apiUrl = `${environment.apiUrl}/api/cashes`;
  private readonly usersApiUrl = `${environment.apiUrl}/api/users`;

  constructor(private readonly http: HttpClient) {}

  getAll(): Observable<CashViewModel[]> {
    return this.http.get<CashApi[]>(this.apiUrl).pipe(map((items) => items.map((item) => this.toViewModel(item))));
  }

  getCashierOptions(): Observable<CashierOption[]> {
    return this.http.get<UserApi[]>(this.usersApiUrl).pipe(
      map((items) =>
        items
          .filter((item) => item.approved && item.roleName === 'Cajero')
          .map((item) => ({ userId: item.userId, userName: item.userName }))
      )
    );
  }

  create(value: CashFormValue): Observable<CashViewModel> {
    return this.http.post<CashApi>(this.apiUrl, { cashDescription: value.description.trim() }).pipe(map((item) => this.toViewModel(item)));
  }

  update(cashId: number, value: CashFormValue): Observable<CashViewModel> {
    return this.http
      .put<CashApi>(`${this.apiUrl}/${cashId}`, {
        cashDescription: value.description.trim(),
        active: value.active ? 'Y' : 'N',
      })
      .pipe(map((item) => this.toViewModel(item)));
  }

  delete(cashId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${cashId}`);
  }

  assignCashier(cashId: number, userId: number): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${cashId}/assignments/${userId}`, {});
  }

  unassignCashier(cashId: number, userId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${cashId}/assignments/${userId}`);
  }

  openSession(cashId: number): Observable<CashSessionViewModel> {
    return this.http.post<CashSessionApi>(`${this.apiUrl}/${cashId}/sessions/open`, {}).pipe(map((item) => this.toSession(item)));
  }

  closeSession(cashId: number): Observable<CashSessionViewModel> {
    return this.http.post<CashSessionApi>(`${this.apiUrl}/${cashId}/sessions/close`, {}).pipe(map((item) => this.toSession(item)));
  }

  private toViewModel(item: CashApi): CashViewModel {
    return {
      cashId: item.cashId,
      name: item.cashDescription,
      code: `CAJ-${String(item.cashId).padStart(3, '0')}`,
      description: item.cashDescription,
      isActive: item.active === 'Y',
      createdAt: '',
      activeFlag: item.active,
      assignedCashiers: item.assignedCashiers.map((cashier) => ({ userId: cashier.userId, userName: cashier.userName })),
      activeSession: item.activeSession ? this.toSession(item.activeSession) : undefined,
    };
  }

  private toSession(item: CashSessionApi): CashSessionViewModel {
    return {
      cashSessionId: item.cashSessionId,
      cashId: item.cashId,
      userId: item.userId,
      userName: item.userName,
      startedAt: item.startedAt,
      endedAt: item.endedAt ?? undefined,
      isActive: item.isActive,
    };
  }
}
