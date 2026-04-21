import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { forkJoin, Observable, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

import { environment } from '../../../../environments/environment';

export interface DashboardStat {
  title: string;
  value: number;
  icon: string;
  color: string;
  description: string;
}

@Injectable({
  providedIn: 'root',
})
export class DashboardService {
  private readonly apiUrl = `${environment.apiUrl}/api`;

  constructor(private readonly http: HttpClient) {}

  loadStats(): Observable<DashboardStat[]> {
    return forkJoin({
      users: this.http.get<unknown[]>(`${this.apiUrl}/users/active-report`).pipe(catchError(() => of([]))),
      clients: this.http.get<unknown[]>(`${this.apiUrl}/clients`).pipe(catchError(() => of([]))),
      contracts: this.http.get<unknown[]>(`${this.apiUrl}/contracts`).pipe(catchError(() => of([]))),
      attentions: this.http.get<unknown[]>(`${this.apiUrl}/attentions`).pipe(catchError(() => of([]))),
      cashes: this.http.get<unknown[]>(`${this.apiUrl}/cashes`).pipe(catchError(() => of([]))),
    }).pipe(
      map(({ users, clients, contracts, attentions, cashes }) => [
        {
          title: 'Usuarios activos',
          value: users.length,
          icon: 'pi pi-users',
          color: 'primary',
          description: 'Personal con acceso vigente al sistema.',
        },
        {
          title: 'Clientes registrados',
          value: clients.length,
          icon: 'pi pi-id-card',
          color: 'success',
          description: 'Base de clientes disponible para operación.',
        },
        {
          title: 'Contratos vigentes',
          value: contracts.length,
          icon: 'pi pi-file',
          color: 'warning',
          description: 'Contratos administrados desde el panel.',
        },
        {
          title: 'Atenciones generadas',
          value: attentions.length,
          icon: 'pi pi-comments',
          color: 'danger',
          description: 'Seguimiento operativo de atenciones registradas.',
        },
        {
          title: 'Cajas configuradas',
          value: cashes.length,
          icon: 'pi pi-shop',
          color: 'primary',
          description: 'Puntos de atención y caja disponibles.',
        },
      ])
    );
  }
}
