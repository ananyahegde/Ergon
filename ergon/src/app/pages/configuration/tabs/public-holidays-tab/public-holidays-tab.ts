import { Component, inject, signal, computed } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MasterService } from '../../../../core/services/master.service';
import { ToastService } from '../../../../core/services/toast.service';
import { PublicHoliday } from '../../../../core/models/master.model';

@Component({
  selector: 'app-public-holidays-tab',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './public-holidays-tab.html',
  styleUrl: './public-holidays-tab.css'
})
export class PublicHolidaysTab {
  private masterService = inject(MasterService);
  private toastService = inject(ToastService);

  publicHolidays = this.masterService.publicHolidays;

  search = signal('');
  showForm = signal(false);
  editingHoliday = signal<PublicHoliday | null>(null);
  formName = signal('');
  formDate = signal('');
  saving = signal(false);

  filtered = computed(() => {
    const q = this.search().toLowerCase().trim();
    if (!q) return this.publicHolidays();
    return this.publicHolidays().filter(h => h.publicHolidayName.toLowerCase().includes(q));
  });

  isFormValid = computed(() =>
    this.formName().trim().length >= 2 && this.formDate().length > 0
  );

  openAdd() {
    this.editingHoliday.set(null);
    this.formName.set('');
    this.formDate.set('');
    this.showForm.set(true);
  }

  openEdit(holiday: PublicHoliday) {
    this.editingHoliday.set(holiday);
    this.formName.set(holiday.publicHolidayName);
    this.formDate.set(holiday.publicHolidayDate);
    this.showForm.set(true);
  }

  closeForm() {
    this.showForm.set(false);
    this.editingHoliday.set(null);
    this.formName.set('');
    this.formDate.set('');
  }

  save() {
    if (!this.isFormValid()) return;

    this.saving.set(true);
    const editing = this.editingHoliday();
    const payload = {
      publicHolidayName: this.formName().trim(),
      publicHolidayDate: this.formDate()
    };

    const request$ = editing
      ? this.masterService.updatePublicHoliday(editing.publicHolidayId, payload)
      : this.masterService.createPublicHoliday(payload);

    request$.subscribe({
      next: () => {
        this.toastService.success(editing ? 'Public holiday updated.' : 'Public holiday created.');
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