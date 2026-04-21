import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { Client } from '../../../core/models';
import { environment } from '../../../../environments/environment';

interface ClientApi {
  clientId: number;
  name: string;
  lastName: string;
  identification: string;
  email: string;
  phoneNumber: string;
  address: string;
  referenceAddress: string;
}

interface ClientCreateApi {
  name: string;
  lastName: string;
  identification: string;
  email: string;
  phoneNumber: string;
  address: string;
  referenceAddress: string;
}

export interface ClientViewModel extends Client {
  referenceAddress: string;
  displayName: string;
}

export interface ClientFormValue {
  identification: string;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  address: string;
  referenceAddress: string;
}

@Injectable({
  providedIn: 'root',
})
export class ClientsService {
  private readonly apiUrl = `${environment.apiUrl}/api/clients`;

  constructor(private readonly http: HttpClient) {}

  getAll(identification?: string): Observable<ClientViewModel[]> {
    let params = new HttpParams();
    if (identification?.trim()) {
      params = params.set('identification', identification.trim());
    }

    return this.http.get<ClientApi[]>(this.apiUrl, { params }).pipe(map((items) => items.map((item) => this.toViewModel(item))));
  }

  create(value: ClientFormValue): Observable<ClientViewModel> {
    return this.http.post<ClientApi>(this.apiUrl, this.toRequest(value)).pipe(map((item) => this.toViewModel(item)));
  }

  update(clientId: number, value: ClientFormValue): Observable<ClientViewModel> {
    return this.http.put<ClientApi>(`${this.apiUrl}/${clientId}`, this.toRequest(value)).pipe(map((item) => this.toViewModel(item)));
  }

  delete(clientId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${clientId}`);
  }

  private toViewModel(item: ClientApi): ClientViewModel {
    return {
      clientId: item.clientId,
      identification: item.identification,
      firstName: item.name,
      lastName: item.lastName,
      email: item.email,
      phone: item.phoneNumber,
      address: item.address,
      createdAt: '',
      status: 'Activo',
      referenceAddress: item.referenceAddress,
      displayName: `${item.name} ${item.lastName}`.trim(),
    };
  }

  private toRequest(value: ClientFormValue): ClientCreateApi {
    return {
      identification: value.identification.trim(),
      name: value.firstName.trim(),
      lastName: value.lastName.trim(),
      email: value.email.trim(),
      phoneNumber: value.phone.trim(),
      address: value.address.trim(),
      referenceAddress: value.referenceAddress.trim(),
    };
  }
}
