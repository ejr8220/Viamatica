import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, catchError, of, tap, throwError } from 'rxjs';

import { environment } from '../../../environments/environment';
import { MenuItem } from '../models';
import { NotificationService } from './notification.service';

@Injectable({
  providedIn: 'root',
})
export class MenuService {
  private readonly apiUrl = `${environment.apiUrl}/api/navigation`;
  private readonly menuItemsSubject = new BehaviorSubject<MenuItem[]>([]);

  readonly menuItems$ = this.menuItemsSubject.asObservable();

  constructor(
    private readonly http: HttpClient,
    private readonly notificationService: NotificationService
  ) {}

  loadMenu(): Observable<MenuItem[]> {
    return this.http.get<MenuItem[]>(`${this.apiUrl}/menu`).pipe(
      tap((items) => this.menuItemsSubject.next(items)),
      catchError((error) => {
        this.menuItemsSubject.next([]);
        this.notificationService.error('No se pudo cargar el menú desde la base de datos.');
        return throwError(() => error);
      })
    );
  }
}
