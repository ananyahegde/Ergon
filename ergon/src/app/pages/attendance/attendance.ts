import { Component, inject, signal, computed, OnInit, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { AuthService } from '../../core/auth/auth.service';
import { EmployeeService } from '../../core/services/employee.service';
import { AttendanceService } from '../../core/services/attendance.service';
import { ToastService } from '../../core/services/toast.service';
import { MasterService } from '../../core/services/master.service';
import { AttendanceRecord, MyTodayAttendance, ATTENDANCE_STATUSES } from '../../core/models/attendance.model';
import { EmployeeDashboardSummary, TodayAttendanceSnapshot, AttendanceTodaySummary } from '../../core/models/attendance.model';
import { MultiSelectDropdown } from '../../shared/components/multi-select-dropdown/multi-select-dropdown';

@Component({
  selector: 'app-attendance',
  standalone: true,
  imports: [FormsModule, MultiSelectDropdown],
  templateUrl: './attendance.html',
  styleUrl: './attendance.css',
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class Attendance implements OnInit {
  private authService = inject(AuthService);
  private attendanceService = inject(AttendanceService);
  private toastService = inject(ToastService);  
  private employeeService = inject(EmployeeService);
  private masterService = inject(MasterService);

  currentUser = this.authService.currentUser;

  isHRAdmin = computed(() => this.currentUser()?.role === 'HR Admin');
  isHR = computed(() => this.currentUser()?.role === 'HR');
  isManager = computed(() => this.currentUser()?.role === 'Manager');
  isEmployee = computed(() => this.currentUser()?.role === 'Employee');
  isHROrAbove = computed(() => this.isHRAdmin() || this.isHR());
  isEmpOrManager = computed(() => this.isEmployee() || this.isManager());

  isWeekend = new Date().getDay() === 0 || new Date().getDay() === 6;

  isDayWeekend(day: number): boolean {
    const d = new Date(this.selectedYear(), this.selectedMonth() - 1, day).getDay();
    return d === 0 || d === 6;
  }

  isPublicHolidayDate(day: number): boolean {
    const date = new Date(this.selectedYear(), this.selectedMonth() - 1, day)
      .toISOString().split('T')[0];
    return this.masterService.publicHolidays().some(h => h.publicHolidayDate === date);
  }

  isPublicHoliday = computed(() => {
    const today = new Date().toISOString().split('T')[0];
    return this.masterService.publicHolidays().some(h => h.publicHolidayDate === today);
  });

  isNonWorkingDay = computed(() => this.isWeekend || this.isPublicHoliday());

  isLoading = signal(false);
  isClockLoading = signal(false);
  attendanceRecords = signal<AttendanceRecord[]>([]);
  totalCount = signal(0);
  totalPages = signal(0);
  totalEmployees = signal(0);
  myTodayAttendance = signal<MyTodayAttendance | null>(null);

  todaySnapshot = signal<AttendanceTodaySummary | null>(null);
  employeeSummary = signal<EmployeeDashboardSummary | null>(null);

  private searchSubject = new Subject<string>();
  search = signal('');
  selectedStatuses = signal<string[]>([]);
  selectedMonth = signal(new Date().getMonth() + 1);
  selectedYear = signal(new Date().getFullYear());
  pageNumber = signal(1);
  pageSize = signal(50);

  attendanceStatuses = ATTENDANCE_STATUSES;
  statusOptions = ATTENDANCE_STATUSES.map(s => ({ label: s, value: s }));

  months = [
    { label: 'January', value: 1 },
    { label: 'February', value: 2 },
    { label: 'March', value: 3 },
    { label: 'April', value: 4 },
    { label: 'May', value: 5 },
    { label: 'June', value: 6 },
    { label: 'July', value: 7 },
    { label: 'August', value: 8 },
    { label: 'September', value: 9 },
    { label: 'October', value: 10 },
    { label: 'November', value: 11 },
    { label: 'December', value: 12 }
  ];

  years = Array.from({ length: 5 }, (_, i) => new Date().getFullYear() - i);

  daysInMonth = computed(() => {
    return new Date(this.selectedYear(), this.selectedMonth(), 0).getDate();
  });

  dateColumns = computed(() => {
    return Array.from({ length: this.daysInMonth() }, (_, i) => i + 1);
  });

  employeeRows = computed(() => {
    const records = this.attendanceRecords();
    const map = new Map<string, { employeeName: string; records: Map<number, AttendanceRecord> }>();
    for (const r of records) {
      const day = new Date(r.date).getDate();
      if (!map.has(r.employeeId)) {
        map.set(r.employeeId, { employeeName: r.employeeName, records: new Map() });
      }
      map.get(r.employeeId)!.records.set(day, r);
    }
    return Array.from(map.values());
  });

  chartData = computed(() => {
    const records = this.attendanceRecords();
    const days = this.daysInMonth();
    return Array.from({ length: days }, (_, i) => {
      const day = i + 1;
      const count = records.filter(r => new Date(r.date).getDate() === day && r.attendanceStatus === 'Present').length;
      return { day, count };
    });
  });

  isClockedIn = computed(() => {
    const t = this.myTodayAttendance();
    return t !== null && t.clockOutTime === null;
  });

  hasActiveFilters = computed(() =>
    this.selectedStatuses().length > 0 || this.search().length > 0
  );

  ngOnInit() {
    this.searchSubject.pipe(
      debounceTime(400),
      distinctUntilChanged()
    ).subscribe(() => {
      this.pageNumber.set(1);
      this.loadAttendances();
    });

    if (this.isHROrAbove()) {
      this.loadHRView();
    } else {
      this.loadEmpView();
    }
  }

  loadHRView() {
    this.isLoading.set(true);
    forkJoin({
      snapshot: this.attendanceService.getToday(),
      attendances: this.attendanceService.getAll({
        month: this.selectedMonth(),
        year: this.selectedYear(),
        pageSize: this.pageSize(),
        pageNumber: this.pageNumber()
      }),
      ...(this.isHR() ? { myToday: this.attendanceService.getMyToday() } : {}),
      employeeCount: this.employeeService.getAll({ pageNumber: 1, pageSize: 1, employmentStatuses: ["Active"]})
    }).subscribe({
      next: res => {
        this.todaySnapshot.set(res.snapshot as AttendanceTodaySummary);
        this.attendanceRecords.set(res.attendances.items);
        this.totalCount.set(res.attendances.totalCount);
        this.totalPages.set(res.attendances.totalPages);
        this.totalEmployees.set(res.employeeCount.totalCount);
        if ('myToday' in res) {
          this.myTodayAttendance.set(res.myToday as MyTodayAttendance | null);
        }
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  loadEmpView() {
    this.isLoading.set(true);
    forkJoin({
      summary: this.attendanceService.getEmployeeSummary(),
      myToday: this.attendanceService.getMyToday(),
      attendances: this.attendanceService.getAll({
        month: this.selectedMonth(),
        year: this.selectedYear(),
        pageSize: this.pageSize(),
        pageNumber: this.pageNumber()
      })
    }).subscribe({
      next: res => {
        this.employeeSummary.set(res.summary);
        this.myTodayAttendance.set(res.myToday);
        this.attendanceRecords.set(res.attendances.items);
        this.totalCount.set(res.attendances.totalCount);
        this.totalPages.set(res.attendances.totalPages);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  loadAttendances() {
    this.isLoading.set(true);
    this.attendanceService.getAll({
      month: this.selectedMonth(),
      year: this.selectedYear(),
      status: this.selectedStatuses().length ? this.selectedStatuses() : undefined,
      pageSize: this.isHROrAbove() ? 200 : 50,
      pageNumber: this.pageNumber()
    }).subscribe({
      next: res => {
        this.attendanceRecords.set(res.items);
        this.totalCount.set(res.totalCount);
        this.totalPages.set(res.totalPages);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  onMonthChange(month: number) {
    this.selectedMonth.set(month);
    this.pageNumber.set(1);
    this.loadAttendances();
  }

  onYearChange(year: number) {
    this.selectedYear.set(year);
    this.pageNumber.set(1);
    this.loadAttendances();
  }

  onSearch(value: string) {
    this.search.set(value);
    this.searchSubject.next(value);
  }

  onStatusChange(statuses: string[]) {
    this.selectedStatuses.set(statuses);
    this.pageNumber.set(1);
    this.loadAttendances();
  }

  clearFilters() {
    this.search.set('');
    this.selectedStatuses.set([]);
    this.pageNumber.set(1);
    this.loadAttendances();
  }

  goToPage(page: number) {
    if (page < 1 || page > this.totalPages()) return;
    this.pageNumber.set(page);
    this.loadAttendances();
  }

  pages = computed(() => Array.from({ length: this.totalPages() }, (_, i) => i + 1));

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
      error: (err) => {
        this.toastService.error(err?.error?.message ?? 'Failed to clock in.');
        this.isClockLoading.set(false);
      }
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
        error: (err) => {
        this.toastService.error(err?.error?.message ?? 'Failed to clock out.');
        this.isClockLoading.set(false);
      }
    });
  }

  getStatusClass(status: string): string {
    switch (status) {
      case 'Present': return 'status-present';
      case 'Absent': return 'status-absent';
      case 'OnLeave': return 'status-on-leave';
      case 'HalfDay': return 'status-half-day';
      case 'Incomplete': return 'status-incomplete';
      default: return '';
    }
  }

  getStatusCellClass(status: string): string {
    switch (status) {
      case 'Present': return 'cell-present';
      case 'Absent': return 'cell-absent';
      case 'OnLeave': return 'cell-on-leave';
      case 'HalfDay': return 'cell-half-day';
      case 'Incomplete': return 'cell-incomplete';
      default: return 'cell-empty';
    }
  }

  getCellLabel(status: string): string {
    switch (status) {
      case 'Present': return 'P';
      case 'Absent': return 'A';
      case 'OnLeave': return 'L';
      case 'HalfDay': return 'H';
      case 'Incomplete': return 'I';
      default: return '-';
    }
  }

  formatTime(time: string | null): string {
    if (!time) return '-';
    const [h, m] = time.split(':');
    const hour = parseInt(h);
    const suffix = hour >= 12 ? 'PM' : 'AM';
    const display = hour % 12 || 12;
    return `${display}:${m} ${suffix}`;
  }

  // SVG line chart helpers
  chartWidth = 600;
  chartHeight = 160;
  chartPadding = { top: 10, right: 10, bottom: 30, left: 35 };

  chartPoints = computed(() => {
    const data = this.chartData();
    if (!data.length) return '';
    const maxCount = Math.max(this.totalEmployees(), 1);
    const w = this.chartWidth - this.chartPadding.left - this.chartPadding.right;
    const h = this.chartHeight - this.chartPadding.top - this.chartPadding.bottom;
    return data.map((d, i) => {
      const x = this.chartPadding.left + (i / (data.length - 1 || 1)) * w;
      const y = this.chartPadding.top + h - (d.count / maxCount) * h;
      return `${x},${y}`;
    }).join(' ');
  });

  chartXLabels = computed(() => {
    const data = this.chartData();
    const w = this.chartWidth - this.chartPadding.left - this.chartPadding.right;
    return data
      .filter((_, i) => i % 5 === 0 || i === data.length - 1)
      .map(d => {
        const i = d.day - 1;
        const x = this.chartPadding.left + (i / (data.length - 1 || 1)) * w;
        return { x, label: d.day };
      });
  });

  chartYLabels = computed(() => {
    const data = this.chartData();
    const maxCount = Math.max(this.totalEmployees(), 1);
    const h = this.chartHeight - this.chartPadding.top - this.chartPadding.bottom;
    return [0, Math.round(maxCount / 2), maxCount].map(v => {
      const y = this.chartPadding.top + h - (v / maxCount) * h;
      return { y, label: v };
    });
  });

  hoveredPoint = signal<{ x: number; y: number; day: number; count: number } | null>(null);

  chartDots = computed(() => {
    const data = this.chartData();
    if (!data.length) return [];
    const maxCount = Math.max(this.totalEmployees(), 1);    
    const w = this.chartWidth - this.chartPadding.left - this.chartPadding.right;
    const h = this.chartHeight - this.chartPadding.top - this.chartPadding.bottom;
    return data.map((d, i) => ({
      x: this.chartPadding.left + (i / (data.length - 1 || 1)) * w,
      y: this.chartPadding.top + h - (d.count / maxCount) * h,
      day: d.day,
      count: d.count
    }));
  });

  onChartHover(event: MouseEvent, svgEl: HTMLElement) {
    const rect = svgEl.getBoundingClientRect();
    const scaleX = this.chartWidth / rect.width;
    const mouseX = (event.clientX - rect.left) * scaleX;
    const dots = this.chartDots();
    if (!dots.length) return;
    const closest = dots.reduce((prev, curr) =>
      Math.abs(curr.x - mouseX) < Math.abs(prev.x - mouseX) ? curr : prev
    );
    this.hoveredPoint.set(closest);
  }
}