import { Component, inject, signal, OnInit } from '@angular/core';
import { DatePipe } from '@angular/common';
import { DashboardService } from '../../core/services/dashboard.service';
import { HrDashboardSummary } from '../../core/models/dashboard.model';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [DatePipe],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css'
})
export class Dashboard implements OnInit {
  private dashboardService = inject(DashboardService);

  summary = signal<HrDashboardSummary | null>(null);
  isLoading = signal(true);

  today = new Date();

  ngOnInit() {
    this.dashboardService.getHrSummary().subscribe({
      next: (data) => {
        this.summary.set(data);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  getMonthName(month: number): string {
    return new Date(2000, month - 1).toLocaleString('default', { month: 'short' });
  }

  getMaxSalary(): number {
    const items = this.summary()?.payrollSummary ?? [];
    return Math.max(...items.map(p => p.totalSalary));
  }

  getBarHeight(salary: number): number {
    const max = this.getMaxSalary();
    return max === 0 ? 0 : Math.round((salary / max) * 130);
  }

  formatSalary(amount: number): string {
    if (amount >= 100000) return `₹${(amount / 100000).toFixed(1)}L`;
    if (amount >= 1000) return `₹${(amount / 1000).toFixed(0)}K`;
    return `₹${amount}`;
  }

  getPayrollStatusClass(status: string): string {
    switch (status) {
      case 'Approved': return 'status-active';
      case 'Pending': return 'status-notice';
      case 'Rejected': return 'status-resigned';
      default: return '';
    }
  }
}
