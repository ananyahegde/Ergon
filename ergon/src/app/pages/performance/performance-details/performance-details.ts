import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../../core/auth/auth.service';
import { PerformanceService } from '../../../core/services/performance.service';
import { ToastService } from '../../../core/services/toast.service';
import { EmployeeService } from '../../../core/services/employee.service';
import { ReviewCycle, ReviewCycleDetails, REVIEW_CYCLE_STATUS_LABELS } from '../../../core/models/performance.model';
import { EmployeeListItem } from '../../../core/models/employee.model';

@Component({
  selector: 'app-performance-detail',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './performance-details.html',
  styleUrl: './performance-details.css'
})
export class PerformanceDetail implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private authService = inject(AuthService);
  private performanceService = inject(PerformanceService);
  private toastService = inject(ToastService);
  private employeeService = inject(EmployeeService);

  currentUser = this.authService.currentUser;

  isHRAdmin = computed(() => this.currentUser()?.role === 'HR Admin');
  isHR = computed(() => this.currentUser()?.role === 'HR');
  isManager = computed(() => this.currentUser()?.role === 'Manager');
  isEmployee = computed(() => this.currentUser()?.role === 'Employee');

  reviewCycleId = signal('');
  cycle = signal<ReviewCycle | null>(null);
  details = signal<ReviewCycleDetails[]>([]);
  totalCount = signal(0);
  totalPages = signal(0);
  pageNumber = signal(1);
  pageSize = signal(20);

  isLoading = signal(false);
  isSubmitting = signal(false);

  // Add employee modal
  showAddEmployeeModal = signal(false);
  employees = signal<EmployeeListItem[]>([]);
  selectedEmployeeId = signal('');
  isLoadingEmployees = signal(false);

  // Inline feedback state: detailId -> { feedbackScore, managerComments }
  feedbackForms = signal<Record<string, { feedbackScore: number | null; managerComments: string }>>({});
  submittingFeedback = signal<Record<string, boolean>>({});

  // Self score state
  selfScoreForm = signal<Record<string, number | null>>({});
  submittingSelfScore = signal<Record<string, boolean>>({});

  statusLabels = REVIEW_CYCLE_STATUS_LABELS;
  pages = computed(() => Array.from({ length: this.totalPages() }, (_, i) => i + 1));

  myDetail = computed(() => {
    const name = this.currentUser()?.firstName ?? '';
    return this.details().find(d => d.employeeName === name) ?? null;
  });

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('reviewCycleId') ?? '';
    this.reviewCycleId.set(id);
    this.loadCycle();
    this.loadDetails();
  }

  loadCycle() {
    this.performanceService.getById(this.reviewCycleId()).subscribe({
      next: cycle => this.cycle.set(cycle)
    });
  }

  loadDetails() {
    this.isLoading.set(true);
    const load$ = (this.isHRAdmin() || this.isHR())
      ? this.performanceService.getDetails(this.reviewCycleId(), { pageNumber: this.pageNumber(), pageSize: this.pageSize() })
      : this.isManager()
        ? this.performanceService.getMyTeamDetails(this.reviewCycleId(), { pageNumber: this.pageNumber(), pageSize: this.pageSize() })
        : this.performanceService.getDetails(this.reviewCycleId(), { pageNumber: this.pageNumber(), pageSize: this.pageSize() });

    load$.subscribe({
      next: res => {
        this.details.set(res.items);
        this.totalCount.set(res.totalCount);
        this.totalPages.set(res.totalPages);
        this.initForms(res.items);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  initForms(items: ReviewCycleDetails[]) {
    const feedbackMap: Record<string, { feedbackScore: number | null; managerComments: string }> = {};
    const selfMap: Record<string, number | null> = {};
    for (const item of items) {
      feedbackMap[item.reviewCycleDetailsId] = {
        feedbackScore: item.feedbackScore || null,
        managerComments: item.managerComments || ''
      };
      selfMap[item.reviewCycleDetailsId] = item.selfScore || null;
    }
    this.feedbackForms.set(feedbackMap);
    this.selfScoreForm.set(selfMap);
  }

  updateFeedbackForm(detailId: string, field: 'feedbackScore' | 'managerComments', value: string | number) {
    this.feedbackForms.update(map => ({
      ...map,
      [detailId]: { ...map[detailId], [field]: value }
    }));
  }

  updateSelfScoreForm(detailId: string, value: number) {
    this.selfScoreForm.update(map => ({ ...map, [detailId]: value }));
  }

  submitFeedback(detail: ReviewCycleDetails) {
    const form = this.feedbackForms()[detail.reviewCycleDetailsId];
    if (form.feedbackScore === null) {
      this.toastService.error('Feedback score is required.');
      return;
    }
    this.submittingFeedback.update(m => ({ ...m, [detail.reviewCycleDetailsId]: true }));
    this.performanceService.submitFeedback(this.reviewCycleId(), detail.reviewCycleDetailsId, {
      feedbackScore: form.feedbackScore!,
      managerComments: form.managerComments
    }).subscribe({
      next: updated => {
        this.details.update(list => list.map(d => d.reviewCycleDetailsId === updated.reviewCycleDetailsId ? updated : d));
        this.toastService.success('Feedback submitted.');
        this.submittingFeedback.update(m => ({ ...m, [detail.reviewCycleDetailsId]: false }));
      },
      error: err => {
        this.toastService.error(err?.error?.message ?? 'Failed to submit feedback.');
        this.submittingFeedback.update(m => ({ ...m, [detail.reviewCycleDetailsId]: false }));
      }
    });
  }

  submitSelfScore(detail: ReviewCycleDetails) {
    const score = this.selfScoreForm()[detail.reviewCycleDetailsId];
    if (score === null) {
      this.toastService.error('Self score is required.');
      return;
    }
    this.submittingSelfScore.update(m => ({ ...m, [detail.reviewCycleDetailsId]: true }));
    this.performanceService.submitSelfScore(this.reviewCycleId(), detail.reviewCycleDetailsId, {
      selfScore: score!
    }).subscribe({
      next: updated => {
        this.details.update(list => list.map(d => d.reviewCycleDetailsId === updated.reviewCycleDetailsId ? updated : d));
        this.toastService.success('Self score submitted.');
        this.submittingSelfScore.update(m => ({ ...m, [detail.reviewCycleDetailsId]: false }));
      },
      error: err => {
        this.toastService.error(err?.error?.message ?? 'Failed to submit self score.');
        this.submittingSelfScore.update(m => ({ ...m, [detail.reviewCycleDetailsId]: false }));
      }
    });
  }

  openAddEmployeeModal() {
    this.selectedEmployeeId.set('');
    this.showAddEmployeeModal.set(true);
    this.isLoadingEmployees.set(true);
    this.employeeService.getAll({ pageNumber: 1, pageSize: 200, employmentStatuses: ['Active'] }).subscribe({
      next: res => {
        this.employees.set(res.items);
        this.isLoadingEmployees.set(false);
      },
      error: () => this.isLoadingEmployees.set(false)
    });
  }

  closeAddEmployeeModal() {
    this.showAddEmployeeModal.set(false);
  }

  submitAddEmployee() {
    const empId = this.selectedEmployeeId();
    if (!empId) {
      this.toastService.error('Please select an employee.');
      return;
    }
    this.isSubmitting.set(true);
    this.performanceService.createDetail(this.reviewCycleId(), { employeeId: empId }).subscribe({
      next: created => {
        this.details.update(list => [...list, created]);
        this.initForms([...this.details()]);
        this.toastService.success('Employee added to review cycle.');
        this.showAddEmployeeModal.set(false);
        this.isSubmitting.set(false);
      },
      error: err => {
        this.toastService.error(err?.error?.message ?? 'Failed to add employee.');
        this.isSubmitting.set(false);
      }
    });
  }

  goToPage(page: number) {
    if (page < 1 || page > this.totalPages()) return;
    this.pageNumber.set(page);
    this.loadDetails();
  }

  goBack() {
    this.router.navigate(['/performance']);
  }

  formatDate(date: string): string {
    return new Date(date).toLocaleDateString('en-US', { year: 'numeric', month: 'short', day: 'numeric' });
  }

  scoreColor(score: number): string {
    if (score === 0) return 'text-gray-300';
    if (score >= 8) return 'text-green-600';
    if (score >= 5) return 'text-yellow-600';
    return 'text-red-500';
  }
}