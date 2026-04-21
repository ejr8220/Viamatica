import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { Turn } from '../../../core/models';
import { environment } from '../../../../environments/environment';

interface TurnApi {
  turnId: number;
  description: string;
  attentionTypeId: string;
  attentionTypeDescription: string;
  date: string;
  cashId: number;
  cashDescription: string;
  userGestorId: number;
  gestorUserName: string;
}

interface CashApi {
  cashId: number;
  cashDescription: string;
}

export interface CashOption {
  cashId: number;
  cashDescription: string;
}

export interface TurnViewModel extends Turn {
  description: string;
  attentionTypeId: string;
  attentionTypeDescription: string;
  date: string;
  cashId: number;
  cashDescription: string;
  gestorUserName: string;
}

export interface TurnFormValue {
  attentionTypeId: string;
  date: string;
  cashId: number;
}

export interface AttentionTypeOption {
  attentionTypeId: string;
  description: string;
}

@Injectable({
  providedIn: 'root',
})
export class TurnsService {
  private readonly apiUrl = `${environment.apiUrl}/api/turns`;
  private readonly cashesApiUrl = `${environment.apiUrl}/api/cashes`;
  private readonly attentionTypeOptions: AttentionTypeOption[] = [
    { attentionTypeId: 'GEN', description: 'Atención general' },
    { attentionTypeId: 'CTR', description: 'Contratación' },
    { attentionTypeId: 'PAY', description: 'Pago de servicio' },
    { attentionTypeId: 'CSV', description: 'Cambio de servicio' },
    { attentionTypeId: 'CFP', description: 'Cambio de forma de pago' },
    { attentionTypeId: 'CAN', description: 'Cancelación' },
  ];

  constructor(private readonly http: HttpClient) {}

  getAll(): Observable<TurnViewModel[]> {
    return this.http.get<TurnApi[]>(this.apiUrl).pipe(map((items) => items.map((item) => this.toViewModel(item))));
  }

  getCashOptions(): Observable<CashOption[]> {
    return this.http.get<CashApi[]>(this.cashesApiUrl).pipe(
      map((items) => items.map((item) => ({ cashId: item.cashId, cashDescription: item.cashDescription })))
    );
  }

  getAttentionTypeOptions(): Observable<AttentionTypeOption[]> {
    return new Observable((subscriber) => {
      subscriber.next(this.attentionTypeOptions);
      subscriber.complete();
    });
  }

  create(value: TurnFormValue): Observable<TurnViewModel> {
    return this.http.post<TurnApi>(this.apiUrl, value).pipe(map((item) => this.toViewModel(item)));
  }

  update(turnId: number, value: TurnFormValue): Observable<TurnViewModel> {
    return this.http.put<TurnApi>(`${this.apiUrl}/${turnId}`, value).pipe(map((item) => this.toViewModel(item)));
  }

  delete(turnId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${turnId}`);
  }

  private toViewModel(item: TurnApi): TurnViewModel {
    return {
      turnId: item.turnId,
      turnNumber: item.description,
      clientId: 0,
      clientName: '',
      serviceId: item.cashId,
      serviceName: item.cashDescription,
      status: 'Registrado',
      priority: 0,
      createdAt: item.date,
      description: item.description,
      attentionTypeId: item.attentionTypeId,
      attentionTypeDescription: item.attentionTypeDescription,
      date: item.date,
      cashId: item.cashId,
      cashDescription: item.cashDescription,
      gestorUserName: item.gestorUserName,
    };
  }
}
