import { Component, inject, signal, computed } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MasterService } from '../../../../core/services/master.service';
import { ToastService } from '../../../../core/services/toast.service';
import { Designation } from '../../../../core/models/master.model';

@Component({
  selector: 'app-designations-tab',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './designations-tab.html',
  styleUrl: './designations-tab.css'
})
export class DesignationsTab {
  private masterService = inject(MasterService);
  private toastService = inject(ToastService);

  designations = this.masterService.designations;

  search = signal('');
  showForm = signal(false);
  editingDesignation = signal<Designation | null>(null);
  formName = signal('');
  saving = signal(false);

  filtered = computed(() => {
    const q = this.search().toLowerCase().trim();
    if (!q) return this.designations();
    return this.designations().filter(d => d.designationName.toLowerCase().includes(q));
  });

  openAdd() {
    this.editingDesignation.set(null);
    this.formName.set('');
    this.showForm.set(true);
  }

  openEdit(designation: Designation) {
    this.editingDesignation.set(designation);
    this.formName.set(designation.designationName);
    this.showForm.set(true);
  }

  closeForm() {
    this.showForm.set(false);
    this.editingDesignation.set(null);
    this.formName.set('');
  }

  save() {
    const name = this.formName().trim();
    if (!name || name.length < 2) return;

    this.saving.set(true);
    const editing = this.editingDesignation();

    const request$ = editing
      ? this.masterService.updateDesignation(editing.designationId, { designationName: name })
      : this.masterService.createDesignation({ designationName: name });

    request$.subscribe({
      next: () => {
        this.toastService.success(editing ? 'Designation updated.' : 'Designation created.');
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