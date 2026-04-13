import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { adminGuard } from './core/guards/admin.guard';

export const routes: Routes = [
  { path: '', redirectTo: '/devices', pathMatch: 'full' },
  {
    path: 'login',
    loadComponent: () =>
      import('./features/auth/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'register',
    loadComponent: () =>
      import('./features/auth/register/register.component').then(m => m.RegisterComponent)
  },
  {
    path: 'devices',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/devices/device-list/device-list.component').then(m => m.DeviceListComponent)
  },
  {
    path: 'devices/new',
    canActivate: [authGuard, adminGuard],
    loadComponent: () =>
      import('./features/devices/device-form/device-form.component').then(m => m.DeviceFormComponent)
  },
  {
    path: 'devices/:id',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/devices/device-detail/device-detail.component').then(m => m.DeviceDetailComponent)
  },
  {
    path: 'devices/:id/edit',
    canActivate: [authGuard, adminGuard],
    loadComponent: () =>
      import('./features/devices/device-form/device-form.component').then(m => m.DeviceFormComponent)
  },
  {
    path: 'users',
    canActivate: [authGuard, adminGuard],
    loadComponent: () =>
      import('./features/users/user-list/user-list.component').then(m => m.UserListComponent)
  },
  {
    path: 'users/new',
    canActivate: [authGuard, adminGuard],
    loadComponent: () =>
      import('./features/users/user-form/user-form.component').then(m => m.UserFormComponent)
  },
  {
    path: 'users/:id/edit',
    canActivate: [authGuard, adminGuard],
    loadComponent: () =>
      import('./features/users/user-form/user-form.component').then(m => m.UserFormComponent)
  },
  { path: '**', redirectTo: '/devices' }
];
