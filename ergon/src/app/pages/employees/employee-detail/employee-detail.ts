import { Component, inject, signal, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { forkJoin } from 'rxjs';
import { EmployeeService } from '../../../core/services/employee.service';
import { EmployeeDetailResponse, EmployeeDocument } from '../../../core/models/employee.model';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-employee-detail',
  standalone: true,
  imports: [DatePipe],
  templateUrl: './employee-detail.html',
  styleUrl: './employee-detail.css'
})
export class EmployeeDetail implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private employeeService = inject(EmployeeService);

  employee = signal<EmployeeDetailResponse | null>(null);
  avatarUrl = signal<string | null>(null);
  documents = signal<EmployeeDocument[]>([]);
  isLoading = signal(true);

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

  downloadDocument(employeeId: string, documentId: string, documentName: string) {
    this.employeeService.downloadDocument(employeeId, documentId).subscribe({
      next: (blob) => {
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = documentName;
        a.click();
        URL.revokeObjectURL(url);
      }
    });
  }
}
