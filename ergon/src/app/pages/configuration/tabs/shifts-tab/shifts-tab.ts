import { Component, inject, signal, computed } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MasterService } from '../../../../core/services/master.service';
import { ToastService } from '../../../../core/services/toast.service';
import { Shift } from '../../../../core/models/master.model';

@Component({
  selector: 'app-shifts-tab',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './shifts-tab.html',
  styleUrl: './shifts-tab.css'
})
export class ShiftsTab {
  private masterService = inject(MasterService);
  private toastService = inject(ToastService);

  shifts = this.masterService.shifts;

  search = signal('');
  showForm = signal(false);
  editingShift = signal<Shift | null>(null);
  formName = signal('');
  formStartTime = signal('');
  formEndTime = signal('');
  saving = signal(false);

  filtered = computed(() => {
    const q = this.search().toLowerCase().trim();
    if (!q) return this.shifts();
    return this.shifts().filter(s => s.shiftName.toLowerCase().includes(q));
  });

  isFormValid = computed(() =>
    this.formName().trim().length >= 2 &&
    this.formStartTime().length > 0 &&
    this.formEndTime().length > 0
  );

  openAdd() {
    this.editingShift.set(null);
    this.formName.set('');
    this.formStartTime.set('');
    this.formEndTime.set('');
    this.showForm.set(true);
  }

  openEdit(shift: Shift) {
    this.editingShift.set(shift);
    this.formName.set(shift.shiftName);
    this.formStartTime.set(shift.startTime);
    this.formEndTime.set(shift.endTime);
    this.showForm.set(true);
  }

  closeForm() {
    this.showForm.set(false);
    this.editingShift.set(null);
    this.formName.set('');
    this.formStartTime.set('');
    this.formEndTime.set('');
  }

  save() {
    if (!this.isFormValid()) return;

    this.saving.set(true);
    const editing = this.editingShift();
    const payload = {
      shiftName: this.formName().trim(),
      startTime: this.formStartTime(),
      endTime: this.formEndTime()
    };

    const request$ = editing
      ? this.masterService.updateShift(editing.shiftId, payload)
      : this.masterService.createShift(payload);

    request$.subscribe({
      next: () => {
        this.toastService.success(editing ? 'Shift updated.' : 'Shift created.');
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