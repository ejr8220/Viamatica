import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { Attention } from '../../../core/models';
import { environment } from '../../../../environments/environment';

interface AttentionApi {
  attentionId: number;
  turnId: number;
  turnDescription: string;
  clientId: number;
  clientName: string;
  contractId?: number | null;
  cashierUserId: number;
  cashierUserName: string;
  attentionTypeId: string;
  attentionTypeDescription: string;
  statusId: number;
  statusDescription: string;
  notes: string;
  createdAt: string;
  completedAt?: string | null;
}

interface AttentionTypeApi {
  attentionTypeId: string;
  description: string;
}

interface TurnApi {
  turnId: number;
  description: string;
}

interface ClientApi {
  clientId: number;
  name: string;
  lastName: string;
}

interface ContractApi {
  contractId: number;
  clientId: number;
  serviceName: string;
  statusDescription: string;
}

export interface AttentionTypeOption {
  attentionTypeId: string;
  description: string;
}

export interface AttentionTurnOption {
  turnId: number;
  description: string;
}

export interface AttentionClientOption {
  clientId: number;
  displayName: string;
}

export interface AttentionContractOption {
  contractId: number;
  clientId: number;
  description: string;
}

export interface AttentionViewModel extends Attention {
  clientId: number;
  contractId?: number | null;
  attentionTypeId: string;
  statusId: number;
  isClosed: boolean;
}

export interface StartAttentionFormValue {
  turnId: number;
  clientId: number;
  contractId?: number | null;
  attentionTypeId: string;
  notes: string;
}

@Injectable({
  providedIn: 'root',
})
export class AttentionsService {
  private readonly apiUrl = `${environment.apiUrl}/api/attentions`;
  private readonly turnsApiUrl = `${environment.apiUrl}/api/turns`;
  private readonly clientsApiUrl = `${environment.apiUrl}/api/clients`;
  private readonly contractsApiUrl = `${environment.apiUrl}/api/contracts`;

  constructor(private readonly http: HttpClient) {}

  getAll(): Observable<AttentionViewModel[]> {
    return this.http.get<AttentionApi[]>(this.apiUrl).pipe(map((items) => items.map((item) => this.toViewModel(item))));
  }

  getTypes(): Observable<AttentionTypeOption[]> {
    return this.http.get<AttentionTypeApi[]>(`${this.apiUrl}/types`).pipe(
      map((items) => items.map((item) => ({ attentionTypeId: item.attentionTypeId, description: item.description })))
    );
  }

  getTurnOptions(): Observable<AttentionTurnOption[]> {
    return this.http
      .get<TurnApi[]>(this.turnsApiUrl)
      .pipe(map((items) => items.map((item) => ({ turnId: item.turnId, description: item.description }))));
  }

  getClientOptions(): Observable<AttentionClientOption[]> {
    return this.http.get<ClientApi[]>(this.clientsApiUrl).pipe(
      map((items) =>
        items.map((item) => ({ clientId: item.clientId, displayName: `${item.name} ${item.lastName}`.trim() }))
      )
    );
  }

  getContractOptions(clientId?: number): Observable<AttentionContractOption[]> {
    let params = new HttpParams();
    if (clientId) {
      params = params.set('clientId', clientId);
    }

    return this.http.get<ContractApi[]>(this.contractsApiUrl, { params }).pipe(
      map((items) =>
        items.map((item) => ({
          contractId: item.contractId,
          clientId: item.clientId,
          description: `CTR-${String(item.contractId).padStart(4, '0')} · ${item.serviceName} · ${item.statusDescription}`,
        }))
      )
    );
  }

  start(value: StartAttentionFormValue): Observable<AttentionViewModel> {
    return this.http.post<AttentionApi>(`${this.apiUrl}/start`, value).pipe(map((item) => this.toViewModel(item)));
  }

  complete(attentionId: number, notes: string): Observable<AttentionViewModel> {
    return this.http
      .post<AttentionApi>(`${this.apiUrl}/${attentionId}/complete`, { notes: notes.trim() })
      .pipe(map((item) => this.toViewModel(item)));
  }

  cancel(attentionId: number, notes: string): Observable<AttentionViewModel> {
    return this.http
      .post<AttentionApi>(`${this.apiUrl}/${attentionId}/cancel`, { notes: notes.trim() })
      .pipe(map((item) => this.toViewModel(item)));
  }

  private toViewModel(item: AttentionApi): AttentionViewModel {
    const isClosed = Boolean(item.completedAt) || /complet|cancel/i.test(item.statusDescription);
    return {
      attentionId: item.attentionId,
      turnId: item.turnId,
      turnNumber: item.turnDescription,
      cashId: 0,
      cashName: '',
      userId: item.cashierUserId,
      userName: item.cashierUserName,
      clientName: item.clientName,
      serviceName: item.attentionTypeDescription,
      startTime: item.createdAt,
      endTime: item.completedAt ?? undefined,
      status: item.statusDescription,
      notes: item.notes,
      clientId: item.clientId,
      contractId: item.contractId ?? null,
      attentionTypeId: item.attentionTypeId,
      statusId: item.statusId,
      isClosed,
    };
  }
}
