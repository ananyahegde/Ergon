import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { roleGuard } from './core/guards/role.guard';

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
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      {
        path: 'dashboard',
        loadComponent: () => import('./pages/dashboard/dashboard').then(m => m.Dashboard)
      },
      {
        path: 'employees',
        canActivate: [roleGuard],
        data: { roles: ['HR Admin', 'HR'] },
        loadComponent: () => import('./pages/employees/employees').then(m => m.Employees)
      },
      {
        path: 'employees/new',
        canActivate: [roleGuard],
        data: { roles: ['HR Admin', 'HR'] },
        loadComponent: () => import('./pages/employees/employee-form/employee-form').then(m => m.EmployeeForm)
      },
      {
        path: 'employees/:id',
        canActivate: [roleGuard],
        data: { roles: ['HR Admin', 'HR'] },
        loadComponent: () => import('./pages/employees/employee-detail/employee-detail').then(m => m.EmployeeDetail)
      },
      {
        path: 'employees/:id/edit',
        canActivate: [roleGuard],
        data: { roles: ['HR Admin', 'HR'] },
        loadComponent: () => import('./pages/employees/employee-form/employee-form').then(m => m.EmployeeForm)
      },
      {
        path: 'notifications',
        loadComponent: () => import('./pages/notifications/notifications').then(m => m.Notifications)
      },
      {
        path: 'attendance',
        loadComponent: () => import('./pages/attendance/attendance').then(m => m.Attendance)
      },
      {
        path: 'leave',
        loadComponent: () => import('./pages/leave/leave').then(m => m.Leave)
      },
      {
        path: 'payroll',
        loadComponent: () => import('./pages/payroll/payroll').then(m => m.Payroll)
      },
      {
        path: 'performance',
        loadComponent: () => import('./pages/performance/performance').then(m => m.Performance)
      },
      {
        path: 'performance/:reviewCycleId',
        loadComponent: () => import('./pages/performance/performance-details/performance-details').then(m => m.PerformanceDetail)
      },
      {
        path: 'my-team',
        canActivate: [roleGuard],
        data: { roles: ['Manager'] },
        loadComponent: () => import('./pages/my-team/my-team').then(m => m.MyTeam)
      },
      {
        path: 'configuration',
        canActivate: [roleGuard],
        data: { roles: ['HR Admin'] },
        loadComponent: () => import('./pages/configuration/configuration').then(m => m.Configuration)
      },
      {
        path: 'profile',
        loadComponent: () => import('./pages/profile/profile').then(m => m.Profile)
      },
      {
        path: 'unauthorized',
        loadComponent: () => import('./pages/unauthorized/unauthorized').then(m => m.Unauthorized)
      },
    ]
  },
  {
    path: '**',
    redirectTo: 'login'
  }
];