export interface NavItem {
  label: string;
  route: string;
  roles: string[];
  icon: string;
}

export const NAV_ITEMS: NavItem[] = [
  { label: 'Dashboard', route: '/dashboard', roles: ['HR Admin', 'HR', 'Manager', 'Employee'], icon: 'dashboard' },
  { label: 'Employees', route: '/employees', roles: ['HR Admin', 'HR'], icon: 'employees' },
  { label: 'Attendance', route: '/attendance', roles: ['HR Admin', 'HR', 'Manager', 'Employee'], icon: 'attendance' },
  { label: 'Leave', route: '/leave', roles: ['HR Admin', 'HR', 'Manager', 'Employee'], icon: 'leave' },
  { label: 'Payroll', route: '/payroll', roles: ['HR Admin', 'HR', 'Manager', 'Employee'], icon: 'payroll' },
  { label: 'Performance', route: '/performance', roles: ['HR Admin', 'HR', 'Manager', 'Employee'], icon: 'performance' },
  { label: 'My Team', route: '/my-team', roles: ['Manager'], icon: 'team' },
  { label: 'Configuration', route: '/configuration', roles: ['HR Admin'], icon: 'configuration' },
  { label: 'Profile', route: '/profile', roles: ['HR Admin', 'HR', 'Manager', 'Employee'], icon: 'profile' },
];
