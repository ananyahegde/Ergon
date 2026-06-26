import { Component, inject, signal, computed } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MasterService } from '../../../../core/services/master.service';
import { ToastService } from '../../../../core/services/toast.service';
import { Department } from '../../../../core/models/master.model';

@Component({
  selector: 'app-departments-tab',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './departments-tab.html',
  styleUrl: './departments-tab.css'
})
export class DepartmentsTab {
  private masterService = inject(MasterService);
  private toastService = inject(ToastService);

  departments = this.masterService.departments;

  search = signal('');
  showForm = signal(false);
  editingDepartment = signal<Department | null>(null);
  formName = signal('');
  saving = signal(false);

  filtered = computed(() => {
    const q = this.search().toLowerCase().trim();
    if (!q) return this.departments();
    return this.departments().filter(d => d.departmentName.toLowerCase().includes(q));
  });

  openAdd() {
    this.editingDepartment.set(null);
    this.formName.set('');
    this.showForm.set(true);
  }

  openEdit(department: Department) {
    this.editingDepartment.set(department);
    this.formName.set(department.departmentName);
    this.showForm.set(true);
  }

  closeForm() {
    this.showForm.set(false);
    this.editingDepartment.set(null);
    this.formName.set('');
  }

  save() {
    const name = this.formName().trim();
    if (!name || name.length < 2) return;

    this.saving.set(true);
    const editing = this.editingDepartment();

    const request$ = editing
      ? this.masterService.updateDepartment(editing.departmentId, { departmentName: name })
      : this.masterService.createDepartment({ departmentName: name });

    request$.subscribe({
      next: () => {
        this.toastService.success(editing ? 'Department updated.' : 'Department created.');
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