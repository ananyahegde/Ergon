import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { EmployeeService } from '../../core/services/employee.service';
import { EmployeeListItem } from '../../core/models/employee.model';

@Component({
  selector: 'app-my-team',
  standalone: true,
  imports: [],
  templateUrl: './my-team.html',
  styleUrl: './my-team.css'
})
export class MyTeam implements OnInit {
  private employeeService = inject(EmployeeService);

  team = signal<EmployeeListItem[]>([]);
  isLoading = signal(false);
  search = signal('');

  ngOnInit() {
    this.isLoading.set(true);
    this.employeeService.getMyTeam().subscribe({
      next: (data) => {
        this.team.set(data);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  filtered = computed(() => {
    const q = this.search().toLowerCase().trim();
    if (!q) return this.team();
    return this.team().filter(m =>
      `${m.firstName} ${m.lastName}`.toLowerCase().includes(q) ||
      m.designationName.toLowerCase().includes(q) ||
      m.workEmail.toLowerCase().includes(q)
    );
  });

  activeCount = computed(() => this.team().filter(m => m.employmentStatus === 'Active').length);
  onLeaveCount = computed(() => this.team().filter(m => m.employmentStatus === 'OnLeave').length);

  getInitials(firstName: string, lastName: string) {
    return `${firstName.charAt(0)}${lastName.charAt(0)}`.toUpperCase();
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
}