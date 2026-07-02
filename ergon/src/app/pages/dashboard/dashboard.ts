import { Component, inject, signal, computed, OnInit, OnDestroy } from '@angular/core';
import { DatePipe } from '@angular/common';
import { Router } from '@angular/router';
import { DashboardService } from '../../core/services/dashboard.service';
import { AttendanceService } from '../../core/services/attendance.service';
import { MasterService } from '../../core/services/master.service';
import { LeaveService } from '../../core/services/leave.service';
import { AuthService } from '../../core/auth/auth.service';
import { ToastService } from '../../core/services/toast.service';
import { HrDashboardSummary, EmployeeDashboardSummary } from '../../core/models/dashboard.model';
import { MyTodayAttendance } from '../../core/models/attendance.model';
import { LeaveModel } from '../../core/models/leave.model';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [DatePipe],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css'
})
export class Dashboard implements OnInit, OnDestroy {
  private dashboardService = inject(DashboardService);
  private attendanceService = inject(AttendanceService);
  private masterService = inject(MasterService);
  private leaveService = inject(LeaveService);
  private authService = inject(AuthService);
  private toastService = inject(ToastService);
  private router = inject(Router);

  currentUser = this.authService.currentUser;
  isHROrAbove = computed(() => this.currentUser()?.role === 'HR Admin' || this.currentUser()?.role === 'HR');

  hrSummary = signal<HrDashboardSummary | null>(null);
  empSummary = signal<EmployeeDashboardSummary | null>(null);
  myTodayAttendance = signal<MyTodayAttendance | null>(null);
  unapprovedLeaves = signal<LeaveModel[]>([]);
  isLoading = signal(true);
  isClockLoading = signal(false);

  today = new Date();
  currentTime = signal(new Date());
  private clockInterval: ReturnType<typeof setInterval> | null = null;

  isWeekend = this.today.getDay() === 0 || this.today.getDay() === 6;

  isPublicHoliday = computed(() => {
    const todayStr = this.today.toISOString().split('T')[0];
    return this.masterService.publicHolidays().some(h => h.publicHolidayDate === todayStr);
  });

  publicHolidayName = computed(() => {
    const todayStr = this.today.toISOString().split('T')[0];
    return this.masterService.publicHolidays().find(h => h.publicHolidayDate === todayStr)?.publicHolidayName ?? null;
  });

  isNonWorkingDay = computed(() => this.isWeekend || this.isPublicHoliday());

  isClockedIn = computed(() => {
    const t = this.myTodayAttendance();
    return t !== null && t.clockOutTime === null;
  });

  isCompletedToday = computed(() => {
    const t = this.myTodayAttendance();
    return t !== null && t.clockOutTime !== null;
  });

  formattedTime = computed(() => {
    const d = this.currentTime();
    let h = d.getHours();
    const m = d.getMinutes().toString().padStart(2, '0');
    const s = d.getSeconds().toString().padStart(2, '0');
    const suffix = h >= 12 ? 'PM' : 'AM';
    h = h % 12 || 12;
    return { time: `${h}:${m}`, seconds: s, suffix };
  });

  ngOnInit() {
    this.clockInterval = setInterval(() => this.currentTime.set(new Date()), 1000);

    if (this.isHROrAbove()) {
      this.dashboardService.getHrSummary().subscribe({
        next: data => {
          this.hrSummary.set(data);
          this.isLoading.set(false);
        },
        error: () => this.isLoading.set(false)
      });

      this.leaveService.getAll({ statuses: ['Open'], pageNumber: 1, pageSize: 5 }).subscribe({
        next: data => this.unapprovedLeaves.set(data.items),
        error: () => {}
      });
    } else {
      this.dashboardService.getEmployeeSummary().subscribe({
        next: data => {
          this.empSummary.set(data);
          this.isLoading.set(false);
        },
        error: () => this.isLoading.set(false)
      });
      this.attendanceService.getMyToday().subscribe({
        next: data => this.myTodayAttendance.set(data),
        error: () => {}
      });
    }
  }

  ngOnDestroy() {
    if (this.clockInterval) clearInterval(this.clockInterval);
  }

  clockIn() {
    this.isClockLoading.set(true);
    this.attendanceService.clockIn().subscribe({
      next: res => {
        this.myTodayAttendance.set({
          attendanceId: res.attendanceId,
          clockInTime: res.clockInTime,
          clockOutTime: res.clockOutTime,
          attendanceStatus: res.attendanceStatus,
          date: res.date
        });
        this.toastService.success('Clocked in successfully.');
        this.isClockLoading.set(false);
      },
      error: () => this.isClockLoading.set(false)
    });
  }

  clockOut() {
    const today = this.myTodayAttendance();
    if (!today) return;
    this.isClockLoading.set(true);
    this.attendanceService.clockOut(today.attendanceId).subscribe({
      next: res => {
        this.myTodayAttendance.set({
          attendanceId: res.attendanceId,
          clockInTime: res.clockInTime,
          clockOutTime: res.clockOutTime,
          attendanceStatus: res.attendanceStatus,
          date: res.date
        });
        this.toastService.success('Clocked out successfully.');
        this.isClockLoading.set(false);
      },
      error: () => this.isClockLoading.set(false)
    });
  }

  getLeaveBarWidth(used: number, total: number): number {
    if (total === 0) return 0;
    return Math.min(Math.round((used / total) * 100), 100);
  }

  getMonthName(month: number): string {
    return new Date(2000, month - 1).toLocaleString('default', { month: 'long' });
  }

  formatSalary(amount: number): string {
    if (amount >= 100000) return `₹${(amount / 100000).toFixed(1)}L`;
    if (amount >= 1000) return `₹${(amount / 1000).toFixed(0)}K`;
    return `₹${amount}`;
  }

  formatTime(time: string | null): string {
    if (!time) return '-';
    const [h, m] = time.split(':');
    const hour = parseInt(h);
    const suffix = hour >= 12 ? 'PM' : 'AM';
    const display = hour % 12 || 12;
    return `${display}:${m} ${suffix}`;
  }

  getMonthName2(month: number): string {
    return new Date(2000, month - 1).toLocaleString('default', { month: 'long' });
  }

  getPayrollStatusClass(status: string): string {
    switch (status) {
      case 'Approved': return 'status-active';
      case 'Pending': return 'status-notice';
      case 'Rejected': return 'status-resigned';
      default: return '';
    }
  }

  getMaxSalary(): number {
    const items = this.hrSummary()?.payrollSummary ?? [];
    return Math.max(...items.map(p => p.totalSalary));
  }

  getBarHeight(salary: number): number {
    const max = this.getMaxSalary();
    return max === 0 ? 0 : Math.round((salary / max) * 130);
  }

  goToLeaves() {
    this.router.navigate(['/leave']);
  }
}