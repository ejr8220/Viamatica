import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { Contract } from '../../../core/models';
import { environment } from '../../../../environments/environment';

interface PaymentApi {
  paymentId: number;
  contractId: number;
  clientId: number;
  amount: number;
  description: string;
  paymentDate: string;
}

interface ContractApi {
  contractId: number;
  startDate: string;
  endDate: string;
  serviceId: number;
  serviceName: string;
  statusId: string;
  statusDescription: string;
  clientId: number;
  clientName: string;
  methodPaymentId: number;
  methodPaymentDescription: string;
  payments: PaymentApi[];
}

interface PaymentMethodApi {
  methodPaymentId: number;
  description: string;
}

interface ClientApi {
  clientId: number;
  name: string;
  lastName: string;
  identification: string;
}

interface ServiceApi {
  serviceId: number;
  serviceName: string;
  price: number;
}

export interface PaymentMethodOption {
  methodPaymentId: number;
  description: string;
}

export interface ContractClientOption {
  clientId: number;
  displayName: string;
  identification: string;
}

export interface ContractServiceOption {
  serviceId: number;
  displayName: string;
  price: number;
}

export interface ContractViewModel extends Contract {
  methodPaymentId: number;
  methodPaymentDescription: string;
  paymentCount: number;
  totalPaid: number;
}

export interface ContractFormValue {
  clientId: number;
  serviceId: number;
  methodPaymentId: number;
  startDate: string;
  endDate: string;
}

@Injectable({
  providedIn: 'root',
})
export class ContractsService {
  private readonly apiUrl = `${environment.apiUrl}/api/contracts`;
  private readonly clientsApiUrl = `${environment.apiUrl}/api/clients`;
  private readonly servicesApiUrl = `${environment.apiUrl}/api/services`;

  constructor(private readonly http: HttpClient) {}

  getAll(clientId?: number): Observable<ContractViewModel[]> {
    let params = new HttpParams();
    if (clientId) {
      params = params.set('clientId', clientId);
    }

    return this.http.get<ContractApi[]>(this.apiUrl, { params }).pipe(map((items) => items.map((item) => this.toViewModel(item))));
  }

  getPaymentMethods(): Observable<PaymentMethodOption[]> {
    return this.http.get<PaymentMethodApi[]>(`${this.apiUrl}/payment-methods`).pipe(
      map((items) => items.map((item) => ({ methodPaymentId: item.methodPaymentId, description: item.description })))
    );
  }

  getClientOptions(): Observable<ContractClientOption[]> {
    return this.http.get<ClientApi[]>(this.clientsApiUrl).pipe(
      map((items) =>
        items.map((item) => ({
          clientId: item.clientId,
          identification: item.identification,
          displayName: `${item.name} ${item.lastName}`.trim(),
        }))
      )
    );
  }

  getServiceOptions(): Observable<ContractServiceOption[]> {
    return this.http.get<ServiceApi[]>(this.servicesApiUrl).pipe(
      map((items) =>
        items.map((item) => ({
          serviceId: item.serviceId,
          price: item.price,
          displayName: `${item.serviceName} · $${item.price.toFixed(2)}`,
        }))
      )
    );
  }

  create(value: ContractFormValue): Observable<ContractViewModel> {
    return this.http.post<ContractApi>(this.apiUrl, value).pipe(map((item) => this.toViewModel(item)));
  }

  update(contractId: number, value: ContractFormValue): Observable<ContractViewModel> {
    return this.http
      .put<ContractApi>(`${this.apiUrl}/${contractId}`, {
        startDate: value.startDate,
        endDate: value.endDate,
      })
      .pipe(map((item) => this.toViewModel(item)));
  }

  cancel(contractId: number): Observable<ContractViewModel> {
    return this.http.post<ContractApi>(`${this.apiUrl}/${contractId}/cancel`, {}).pipe(map((item) => this.toViewModel(item)));
  }

  private toViewModel(item: ContractApi): ContractViewModel {
    const totalPaid = item.payments.reduce((sum, payment) => sum + Number(payment.amount), 0);
    return {
      contractId: item.contractId,
      contractNumber: `CTR-${String(item.contractId).padStart(4, '0')}`,
      clientId: item.clientId,
      clientName: item.clientName,
      serviceId: item.serviceId,
      serviceName: item.serviceName,
      startDate: item.startDate,
      endDate: item.endDate,
      amount: totalPaid,
      status: item.statusDescription,
      createdAt: item.startDate,
      methodPaymentId: item.methodPaymentId,
      methodPaymentDescription: item.methodPaymentDescription,
      paymentCount: item.payments.length,
      totalPaid,
    };
  }
}
