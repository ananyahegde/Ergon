import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { AuthService } from '../../core/auth/auth.service';
import { PerformanceService } from '../../core/services/performance.service';
import { ToastService } from '../../core/services/toast.service';
import {
  ReviewCycle,
  ReviewCycleDetails,
  REVIEW_CYCLE_STATUS_LABELS,
  CreateReviewCycleRequest
} from '../../core/models/performance.model';

@Component({
  selector: 'app-performance',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './performance.html',
  styleUrl: './performance.css'
})
export class Performance implements OnInit {
  private authService = inject(AuthService);
  private performanceService = inject(PerformanceService);
  private toastService = inject(ToastService);
  private router = inject(Router);

  currentUser = this.authService.currentUser;

  isHRAdmin = computed(() => this.currentUser()?.role === 'HR Admin');
  isHR = computed(() => this.currentUser()?.role === 'HR');
  isManager = computed(() => this.currentUser()?.role === 'Manager');
  isEmployee = computed(() => this.currentUser()?.role === 'Employee');

  isLoading = signal(false);
  isSubmitting = signal(false);
  showCreateModal = signal(false);

  reviewCycles = signal<ReviewCycle[]>([]);
  totalCount = signal(0);
  totalPages = signal(0);
  pageNumber = signal(1);
  pageSize = signal(10);
  statusFilter = signal('');

  latestCycleDetails = signal<ReviewCycleDetails[]>([]);

  createForm = signal<CreateReviewCycleRequest>({ reviewName: '', startDate: '', endDate: '' });

  statusLabels = REVIEW_CYCLE_STATUS_LABELS;
  statusOptions = ['Draft', 'Active', 'Closed'];

  pages = computed(() => Array.from({ length: this.totalPages() }, (_, i) => i + 1));

  radarDepartments = computed(() => {
    const details = this.latestCycleDetails();
    const map = new Map<string, { selfTotal: number; feedbackTotal: number; count: number }>();
    for (const d of details) {
      const dept = d.department || 'Unknown';
      const existing = map.get(dept) ?? { selfTotal: 0, feedbackTotal: 0, count: 0 };
      map.set(dept, {
        selfTotal: existing.selfTotal + d.selfScore,
        feedbackTotal: existing.feedbackTotal + d.feedbackScore,
        count: existing.count + 1
      });
    }
    return Array.from(map.entries()).map(([dept, val]) => ({
      dept,
      avgSelf: val.count ? val.selfTotal / val.count : 0,
      avgFeedback: val.count ? val.feedbackTotal / val.count : 0
    }));
  });

  lineChartData = computed(() => {
    const cycles = this.reviewCycles();
    const details = this.latestCycleDetails();
    const empName = this.currentUser()?.firstName ?? '';
    return cycles.map(rc => {
      const match = details.find(d => d.reviewCycleName === rc.reviewName && d.employeeName === empName);
      return {
        label: rc.reviewName,
        selfScore: match?.selfScore ?? null,
        feedbackScore: match?.feedbackScore ?? null
      };
    }).filter(d => d.selfScore !== null || d.feedbackScore !== null);
  });

  teamBarData = computed(() => {
    return this.latestCycleDetails().map(d => ({
      name: d.employeeName,
      selfScore: d.selfScore,
      feedbackScore: d.feedbackScore
    }));
  });

  chartWidth = 600;
  chartHeight = 160;
  chartPadding = { top: 10, right: 10, bottom: 30, left: 35 };

  hoveredLinePoint = signal<{ x: number; y: number; label: string; self: number | null; feedback: number | null } | null>(null);
  hoveredRadarPoint = signal<{ x: number; y: number; dept: string; avgSelf: number; avgFeedback: number } | null>(null);
  hoveredBarIndex = signal<number | null>(null);

  onRadarHover(index: number) {
    const data = this.radarDepartments();
    const n = data.length;
    if (!n) return;
    const cx = this.radarCx();
    const cy = this.radarCy();
    const r = this.radarRadius;
    const angle = (Math.PI * 2 * index) / n - Math.PI / 2;
    const d = data[index];
    this.hoveredRadarPoint.set({
      x: cx + (r + 18) * Math.cos(angle),
      y: cy + (r + 18) * Math.sin(angle),
      dept: d.dept,
      avgSelf: Math.round(d.avgSelf * 10) / 10,
      avgFeedback: Math.round(d.avgFeedback * 10) / 10
    });
  }

  linePoints = computed(() => {
    const data = this.lineChartData();
    if (data.length < 2) return { self: '', feedback: '' };
    const w = this.chartWidth - this.chartPadding.left - this.chartPadding.right;
    const h = this.chartHeight - this.chartPadding.top - this.chartPadding.bottom;
    const selfLine = data.map((d, i) => {
      const x = this.chartPadding.left + (i / (data.length - 1)) * w;
      const y = this.chartPadding.top + h - ((d.selfScore ?? 0) / 10) * h;
      return `${x},${y}`;
    }).join(' ');
    const feedbackLine = data.map((d, i) => {
      const x = this.chartPadding.left + (i / (data.length - 1)) * w;
      const y = this.chartPadding.top + h - ((d.feedbackScore ?? 0) / 10) * h;
      return `${x},${y}`;
    }).join(' ');
    return { self: selfLine, feedback: feedbackLine };
  });

  lineDots = computed(() => {
    const data = this.lineChartData();
    if (!data.length) return [];
    const w = this.chartWidth - this.chartPadding.left - this.chartPadding.right;
    const h = this.chartHeight - this.chartPadding.top - this.chartPadding.bottom;
    return data.map((d, i) => ({
      x: this.chartPadding.left + (i / (data.length - 1 || 1)) * w,
      selfY: this.chartPadding.top + h - ((d.selfScore ?? 0) / 10) * h,
      feedbackY: this.chartPadding.top + h - ((d.feedbackScore ?? 0) / 10) * h,
      label: d.label,
      selfScore: d.selfScore,
      feedbackScore: d.feedbackScore
    }));
  });

  lineYLabels = computed(() => {
    const h = this.chartHeight - this.chartPadding.top - this.chartPadding.bottom;
    return [0, 5, 10].map(v => ({
      y: this.chartPadding.top + h - (v / 10) * h,
      label: v
    }));
  });

  onLineHover(event: MouseEvent, svgEl: HTMLElement) {
    const rect = svgEl.getBoundingClientRect();
    const scaleX = this.chartWidth / rect.width;
    const mouseX = (event.clientX - rect.left) * scaleX;
    const dots = this.lineDots();
    if (!dots.length) return;
    const closest = dots.reduce((prev, curr) =>
      Math.abs(curr.x - mouseX) < Math.abs(prev.x - mouseX) ? curr : prev
    );
    this.hoveredLinePoint.set({
      x: closest.x,
      y: closest.selfY,
      label: closest.label,
      self: closest.selfScore,
      feedback: closest.feedbackScore
    });
  }

  radarSize = 260;
  radarCx = computed(() => this.radarSize / 2);
  radarCy = computed(() => this.radarSize / 2);
  radarRadius = 90;
  radarLevels = [2, 4, 6, 8, 10];

  radarPoints = computed(() => {
    const data = this.radarDepartments();
    const n = data.length;
    if (n < 3) return { self: '', feedback: '', axes: [] as { x1: number; y1: number; x2: number; y2: number; label: string; lx: number; ly: number }[] };
    const cx = this.radarCx();
    const cy = this.radarCy();
    const r = this.radarRadius;
    const axes = data.map((d, i) => {
      const angle = (Math.PI * 2 * i) / n - Math.PI / 2;
      return {
        x1: cx, y1: cy,
        x2: cx + r * Math.cos(angle),
        y2: cy + r * Math.sin(angle),
        label: d.dept,
        lx: cx + (r + 18) * Math.cos(angle),
        ly: cy + (r + 18) * Math.sin(angle)
      };
    });
    const selfPoly = data.map((d, i) => {
      const angle = (Math.PI * 2 * i) / n - Math.PI / 2;
      const val = (d.avgSelf / 10) * r;
      return `${cx + val * Math.cos(angle)},${cy + val * Math.sin(angle)}`;
    }).join(' ');
    const feedbackPoly = data.map((d, i) => {
      const angle = (Math.PI * 2 * i) / n - Math.PI / 2;
      const val = (d.avgFeedback / 10) * r;
      return `${cx + val * Math.cos(angle)},${cy + val * Math.sin(angle)}`;
    }).join(' ');
    return { self: selfPoly, feedback: feedbackPoly, axes };
  });

  radarLevelPolygons = computed(() => {
    const data = this.radarDepartments();
    const n = data.length;
    if (n < 3) return [];
    const cx = this.radarCx();
    const cy = this.radarCy();
    const r = this.radarRadius;
    return this.radarLevels.map(level => {
      const fraction = level / 10;
      return data.map((_, i) => {
        const angle = (Math.PI * 2 * i) / n - Math.PI / 2;
        return `${cx + fraction * r * Math.cos(angle)},${cy + fraction * r * Math.sin(angle)}`;
      }).join(' ');
    });
  });

  barChartWidth = 600;
  barChartHeight = 160;
  barPadding = { top: 10, right: 10, bottom: 40, left: 35 };

  barRects = computed(() => {
    const data = this.teamBarData();
    if (!data.length) return [];
    const w = this.barChartWidth - this.barPadding.left - this.barPadding.right;
    const h = this.barChartHeight - this.barPadding.top - this.barPadding.bottom;
    const groupWidth = w / data.length;
    const barWidth = Math.min(groupWidth * 0.35, 20);
    return data.map((d, i) => {
      const groupX = this.barPadding.left + i * groupWidth + groupWidth / 2;
      return {
        name: d.name ?? '',
        selfScore: d.selfScore,
        feedbackScore: d.feedbackScore,
        selfX: groupX - barWidth - 2,
        feedbackX: groupX + 2,
        selfH: (d.selfScore / 10) * h,
        feedbackH: (d.feedbackScore / 10) * h,
        barW: barWidth,
        selfY: this.barPadding.top + h - (d.selfScore / 10) * h,
        feedbackY: this.barPadding.top + h - (d.feedbackScore / 10) * h,
        labelX: groupX,
        labelY: this.barPadding.top + h + 14
      };
    });
  });

  barYLabels = computed(() => {
    const h = this.barChartHeight - this.barPadding.top - this.barPadding.bottom;
    return [0, 5, 10].map(v => ({
      y: this.barPadding.top + h - (v / 10) * h,
      label: v
    }));
  });

  ngOnInit() {
    this.loadPage();
  }

  loadPage() {
    this.isLoading.set(true);
    this.performanceService.getAll({
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
      status: this.statusFilter() || undefined
    }).subscribe({
      next: res => {
        this.reviewCycles.set(res.items);
        this.totalCount.set(res.totalCount);
        this.totalPages.set(res.totalPages);
        this.loadChartData(res.items);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  loadChartData(cycles: ReviewCycle[]) {
    const latestActive = cycles.find(c => c.reviewCycleStatus === 1)
      ?? cycles.find(c => c.reviewCycleStatus === 2);
    if (!latestActive) return;

    if (this.isHRAdmin() || this.isHR()) {
      this.performanceService.getDetails(latestActive.reviewCycleId, { pageNumber: 1, pageSize: 200 }).subscribe({
        next: res => this.latestCycleDetails.set(res.items)
      });
    } else if (this.isManager()) {
      this.performanceService.getMyTeamDetails(latestActive.reviewCycleId, { pageNumber: 1, pageSize: 200 }).subscribe({
        next: res => this.latestCycleDetails.set(res.items)
      });
    }
  }

  onStatusFilterChange(value: string) {
    this.statusFilter.set(value);
    this.pageNumber.set(1);
    this.loadPage();
  }

  goToPage(page: number) {
    if (page < 1 || page > this.totalPages()) return;
    this.pageNumber.set(page);
    this.loadPage();
  }

  openDetail(cycleId: string) {
    this.router.navigate(['/performance', cycleId]);
  }

  openCreateModal() {
    this.createForm.set({ reviewName: '', startDate: '', endDate: '' });
    this.showCreateModal.set(true);
  }

  closeCreateModal() {
    this.showCreateModal.set(false);
  }

  submitCreate() {
    const form = this.createForm();
    if (!form.reviewName || !form.startDate || !form.endDate) {
      this.toastService.error('All fields are required.');
      return;
    }
    this.isSubmitting.set(true);
    this.performanceService.create(form).subscribe({
      next: created => {
        this.reviewCycles.update(list => [created, ...list]);
        this.toastService.success('Review cycle created.');
        this.showCreateModal.set(false);
        this.isSubmitting.set(false);
      },
      error: err => {
        this.toastService.error(err?.error?.message ?? 'Failed to create review cycle.');
        this.isSubmitting.set(false);
      }
    });
  }

  startCycle(cycle: ReviewCycle, event: MouseEvent) {
    event.stopPropagation();
    this.performanceService.startCycle(cycle.reviewCycleId).subscribe({
      next: updated => {
        this.reviewCycles.update(list => list.map(c => c.reviewCycleId === updated.reviewCycleId ? updated : c));
        this.toastService.success('Review cycle started.');
      },
      error: err => this.toastService.error(err?.error?.message ?? 'Failed to start cycle.')
    });
  }

  closeCycle(cycle: ReviewCycle, event: MouseEvent) {
    event.stopPropagation();
    this.performanceService.closeCycle(cycle.reviewCycleId).subscribe({
      next: res => {
        this.reviewCycles.update(list => list.map(c => c.reviewCycleId === res.data.reviewCycleId ? res.data : c));
        this.toastService.success('Review cycle closed.');
      },
      error: err => this.toastService.error(err?.error?.message ?? 'Failed to close cycle.')
    });
  }

  getStatusClass(status: number): string {
    switch (status) {
      case 0: return 'status-draft';
      case 1: return 'status-active';
      case 2: return 'status-closed';
      default: return '';
    }
  }

  formatDate(date: string): string {
    return new Date(date).toLocaleDateString('en-US', { year: 'numeric', month: 'short', day: 'numeric' });
  }

  updateForm(field: keyof CreateReviewCycleRequest, value: string) {
    this.createForm.update(f => ({ ...f, [field]: value }));
  }
}