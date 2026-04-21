import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { MainLayout } from './layout/main-layout/main-layout';
import { authGuard, roleGuard } from './core/guards';

const routes: Routes = [
  {
    path: 'auth',
    loadChildren: () => import('./features/auth/auth/auth-module').then((m) => m.AuthModule),
  },
  {
    path: '',
    component: MainLayout,
    canActivate: [authGuard],
    children: [
      {
        path: '',
        pathMatch: 'full',
        redirectTo: 'welcome',
      },
      {
        path: 'welcome',
        loadChildren: () => import('./features/welcome/welcome/welcome-module').then((m) => m.WelcomeModule),
      },
      {
        path: 'dashboard',
        canActivate: [roleGuard],
        data: { roles: ['Administrador'] },
        loadChildren: () => import('./features/dashboard/dashboard/dashboard-module').then((m) => m.DashboardModule),
      },
      {
        path: 'users',
        loadChildren: () => import('./features/users/users/users-module').then((m) => m.UsersModule),
      },
      {
        path: 'clients',
        loadChildren: () => import('./features/clients/clients/clients-module').then((m) => m.ClientsModule),
      },
      {
        path: 'cashes',
        loadChildren: () => import('./features/cashes/cashes/cashes-module').then((m) => m.CashesModule),
      },
      {
        path: 'turns',
        loadChildren: () => import('./features/turns/turns/turns-module').then((m) => m.TurnsModule),
      },
      {
        path: 'services',
        loadChildren: () => import('./features/services/services/services-module').then((m) => m.ServicesModule),
      },
      {
        path: 'contracts',
        loadChildren: () => import('./features/contracts/contracts/contracts-module').then((m) => m.ContractsModule),
      },
      {
        path: 'attentions',
        loadChildren: () => import('./features/attentions/attentions/attentions-module').then((m) => m.AttentionsModule),
      },
    ],
  },
  {
    path: '**',
    redirectTo: 'welcome',
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
