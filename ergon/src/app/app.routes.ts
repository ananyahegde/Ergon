import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () => import('./pages/login/login').then(m => m.Login)
  },
  {
    path: '',
    canActivate: [authGuard],
    loadComponent: () => import('./shared/components/shell/shell').then(m => m.Shell),
    children: [
      {
        path: 'dashboard',
        loadComponent: () => import('./pages/dashboard/dashboard').then(m => m.Dashboard)
      },
      {
        path: 'employees',
        loadComponent: () => import('./pages/employees/employees').then(m => m.Employees)
      },
      {
        path: 'employees/:id',
        loadComponent: () => import('./pages/employees/employee-detail/employee-detail').then(m => m.EmployeeDetail)
      },
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      },
      {
        path: 'notifications',
        loadComponent: () => import('./pages/notifications/notifications').then(m => m.Notifications)
      }
    ]
  },
  {
    path: '**',
    redirectTo: 'login'
  }
];
