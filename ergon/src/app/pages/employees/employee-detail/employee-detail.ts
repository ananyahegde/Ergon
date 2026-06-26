import { Component, inject, signal, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { forkJoin } from 'rxjs';
import { DatePipe } from '@angular/common';
import { EmployeeService } from '../../../core/services/employee.service';
import { ToastService } from '../../../core/services/toast.service';
import { EmployeeDetailResponse, EmployeeDocument, EMPLOYMENT_STATUSES } from '../../../core/models/employee.model';
import { SingleSelectDropdown } from '../../../shared/components/single-select-dropdown/single-select-dropdown';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-employee-detail',
  standalone: true,
  imports: [DatePipe, SingleSelectDropdown, RouterLink],
  templateUrl: './employee-detail.html',
  styleUrl: './employee-detail.css'
})
export class EmployeeDetail implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private employeeService = inject(EmployeeService);
  private toastService = inject(ToastService);

  employee = signal<EmployeeDetailResponse | null>(null);
  avatarUrl = signal<string | null>(null);
  documents = signal<EmployeeDocument[]>([]);
  isLoading = signal(true);
  isUpdatingStatus = signal(false);

  showStatusPanel = signal(false);
  selectedStatus = signal<string>('');

  private statusMap: Record<string, number> = {
    'Active': 0,
    'OnNoticePeriod': 1,
    'Resigned': 2,
    'Terminated': 3,
    'Suspended': 4
  };

  statusOptions = EMPLOYMENT_STATUSES.map(s => ({ label: s, value: s }));

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      this.router.navigate(['/employees']);
      return;
    }

    forkJoin({
      employee: this.employeeService.getById(id),
      documents: this.employeeService.getDocuments(id)
    }).subscribe({
      next: ({ employee, documents }) => {
        this.employee.set(employee);
        this.documents.set(documents);
        this.selectedStatus.set(employee.employmentStatus);
        this.isLoading.set(false);

        if (employee.pfp) {
          this.employeeService.getPfp(id).subscribe({
            next: (blob) => this.avatarUrl.set(URL.createObjectURL(blob)),
            error: () => {}
          });
        }
      },
      error: () => {
        this.isLoading.set(false);
        this.router.navigate(['/employees']);
      }
    });
  }

  updateStatus() {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id || !this.selectedStatus()) return;

    this.isUpdatingStatus.set(true);
    this.employeeService.updateStatus(id, this.statusMap[this.selectedStatus()]).subscribe({
      next: () => {
        this.employee.update(emp => emp ? { ...emp, employmentStatus: this.selectedStatus() } : emp);
        this.showStatusPanel.set(false);
        this.toastService.success('Employee status updated successfully.');
        this.isUpdatingStatus.set(false);
      },
      error: (err) => {
        this.toastService.error(this.getBackendError(err));
        this.isUpdatingStatus.set(false);
      }
    });
  }

  goBack() {
    this.router.navigate(['/employees']);
  }

  getInitials(): string {
    const emp = this.employee();
    if (!emp) return '';
    return `${emp.firstName.charAt(0)}${emp.lastName.charAt(0)}`.toUpperCase();
  }

  getStatusClass(status: string): string {
    switch (status) {
      case 'Active': return 'status-active';
      case 'Resigned': return 'status-resigned';
      case 'Suspended': return 'status-suspended';
      case 'Terminated': return 'status-terminated';
      case 'OnNoticePeriod': return 'status-notice';
      default: return '';
    }
  }

  getBackendError(err: any): string {
    if (err?.error?.errors) {
      const messages = Object.values(err.error.errors).flat() as string[];
      return messages[0] ?? 'Something went wrong.';
    }
    return err?.error?.message ?? err?.error?.title ?? 'Something went wrong.';
  }

  downloadDocument(employeeId: string, documentId: string, documentName: string) {
    this.employeeService.downloadDocument(employeeId, documentId).subscribe({
      next: (blob) => {
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = documentName;
        a.click();
        URL.revokeObjectURL(url);
      },
      error: (err) => {
        this.toastService.error(this.getBackendError(err));
      }
    });
  }
}