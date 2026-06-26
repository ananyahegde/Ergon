import { Component, inject, signal, computed } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MasterService } from '../../../../core/services/master.service';
import { ToastService } from '../../../../core/services/toast.service';
import { LeaveType } from '../../../../core/models/master.model';

@Component({
  selector: 'app-leaveTypes-tab',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './leave-types-tab.html',
  styleUrl: './leave-types-tab.css'
})
export class LeaveTypesTab {
  private masterService = inject(MasterService);
  private toastService = inject(ToastService);

  leaveTypes = this.masterService.leaveTypes;

  search = signal('');
  showForm = signal(false);
  editingLeaveType = signal<LeaveType | null>(null);
  formName = signal('');
  saving = signal(false);

  filtered = computed(() => {
    const q = this.search().toLowerCase().trim();
    if (!q) return this.leaveTypes();
    return this.leaveTypes().filter(d => d.leaveTypeName.toLowerCase().includes(q));
  });

  openAdd() {
    this.editingLeaveType.set(null);
    this.formName.set('');
    this.showForm.set(true);
  }

  openEdit(leaveType: LeaveType) {
    this.editingLeaveType.set(leaveType);
    this.formName.set(leaveType.leaveTypeName);
    this.showForm.set(true);
  }

  closeForm() {
    this.showForm.set(false);
    this.editingLeaveType.set(null);
    this.formName.set('');
  }

  save() {
    const name = this.formName().trim();
    if (!name || name.length < 2) return;

    this.saving.set(true);
    const editing = this.editingLeaveType();

    const request$ = editing
      ? this.masterService.updateLeaveType(editing.leaveTypeId, { leaveTypeName: name })
      : this.masterService.createLeaveType({ leaveTypeName: name });

    request$.subscribe({
      next: () => {
        this.toastService.success(editing ? 'LeaveType updated.' : 'LeaveType created.');
        this.closeForm();
        this.saving.set(false);
      },
      error: (err) => {
        this.toastService.error(err?.error?.message ?? 'Something went wrong.');
        this.saving.set(false);
      }
    }); 
  }
}