import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { MenuItem } from '../../../core/models';
import { environment } from '../../../../environments/environment';

export interface WelcomeShortcut {
  title: string;
  route: string;
  icon: string;
}

@Injectable({
  providedIn: 'root',
})
export class WelcomeService {
  private readonly apiUrl = `${environment.apiUrl}/api/navigation/menu`;

  constructor(private readonly http: HttpClient) {}

  getShortcuts(): Observable<WelcomeShortcut[]> {
    return this.http.get<MenuItem[]>(this.apiUrl).pipe(
      map((items) => this.flatten(items))
    );
  }

  private flatten(items: MenuItem[]): WelcomeShortcut[] {
    return items.flatMap((item) => {
      if (item.children?.length) {
        return this.flatten(item.children);
      }

      return item.route ? [{ title: item.label, route: item.route, icon: item.icon }] : [];
    });
  }
}
