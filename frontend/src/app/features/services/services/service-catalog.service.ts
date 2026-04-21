import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { Service } from '../../../core/models';
import { environment } from '../../../../environments/environment';

interface ServiceApi {
  serviceId: number;
  serviceName: string;
  serviceDescription: string;
  price: number;
}

export interface ServiceViewModel extends Service {
  price: number;
  displayName: string;
}

export interface ServiceFormValue {
  name: string;
  description: string;
  price: number;
}

@Injectable({
  providedIn: 'root',
})
export class ServiceCatalogService {
  private readonly apiUrl = `${environment.apiUrl}/api/services`;

  constructor(private readonly http: HttpClient) {}

  getAll(): Observable<ServiceViewModel[]> {
    return this.http.get<ServiceApi[]>(this.apiUrl).pipe(map((items) => items.map((item) => this.toViewModel(item))));
  }

  create(value: ServiceFormValue): Observable<ServiceViewModel> {
    return this.http
      .post<ServiceApi>(this.apiUrl, {
        serviceName: value.name.trim(),
        serviceDescription: value.description.trim(),
        price: value.price,
      })
      .pipe(map((item) => this.toViewModel(item)));
  }

  update(serviceId: number, value: ServiceFormValue): Observable<ServiceViewModel> {
    return this.http
      .put<ServiceApi>(`${this.apiUrl}/${serviceId}`, {
        serviceName: value.name.trim(),
        serviceDescription: value.description.trim(),
        price: value.price,
      })
      .pipe(map((item) => this.toViewModel(item)));
  }

  delete(serviceId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${serviceId}`);
  }

  private toViewModel(item: ServiceApi): ServiceViewModel {
    return {
      serviceId: item.serviceId,
      name: item.serviceName,
      code: `SRV-${String(item.serviceId).padStart(3, '0')}`,
      description: item.serviceDescription,
      estimatedTimeMinutes: Math.round(Number(item.price)),
      isActive: true,
      createdAt: '',
      price: item.price,
      displayName: `${item.serviceName} · $${item.price.toFixed(2)}`,
    };
  }
}
