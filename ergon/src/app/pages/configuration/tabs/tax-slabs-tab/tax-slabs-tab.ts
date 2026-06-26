import { Component, inject, signal, computed } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MasterService } from '../../../../core/services/master.service';
import { ToastService } from '../../../../core/services/toast.service';
import { TaxSlab } from '../../../../core/models/master.model';
import { DecimalPipe } from '@angular/common';

@Component({
  selector: 'app-tax-slabs-tab',
  standalone: true,
  imports: [FormsModule, DecimalPipe],
  templateUrl: './tax-slabs-tab.html',
  styleUrl: './tax-slabs-tab.css'
})
export class TaxSlabsTab {
  private masterService = inject(MasterService);
  private toastService = inject(ToastService);

  taxSlabs = this.masterService.taxSlabs;

  showForm = signal(false);
  editingSlab = signal<TaxSlab | null>(null);
  formMinIncome = signal<number | null>(null);
  formMaxIncome = signal<number | null>(null);
  formTaxPercentage = signal<number | null>(null);
  saving = signal(false);

  isFormValid = computed(() =>
    this.formMinIncome() !== null &&
    this.formMaxIncome() !== null &&
    this.formTaxPercentage() !== null &&
    this.formMinIncome()! >= 0 &&
    this.formMaxIncome()! > this.formMinIncome()! &&
    this.formTaxPercentage()! >= 0 &&
    this.formTaxPercentage()! <= 100
  );

  openAdd() {
    this.editingSlab.set(null);
    this.formMinIncome.set(null);
    this.formMaxIncome.set(null);
    this.formTaxPercentage.set(null);
    this.showForm.set(true);
  }

  openEdit(slab: TaxSlab) {
    this.editingSlab.set(slab);
    this.formMinIncome.set(slab.minIncome);
    this.formMaxIncome.set(slab.maxIncome);
    this.formTaxPercentage.set(slab.taxPercentage);
    this.showForm.set(true);
  }

  closeForm() {
    this.showForm.set(false);
    this.editingSlab.set(null);
    this.formMinIncome.set(null);
    this.formMaxIncome.set(null);
    this.formTaxPercentage.set(null);
  }

  save() {
    if (!this.isFormValid()) return;

    this.saving.set(true);
    const editing = this.editingSlab();
    const payload = {
      minIncome: this.formMinIncome()!,
      maxIncome: this.formMaxIncome()!,
      taxPercentage: this.formTaxPercentage()!
    };

    const request$ = editing
      ? this.masterService.updateTaxSlab(editing.taxSlabId, payload)
      : this.masterService.createTaxSlab(payload);

    request$.subscribe({
      next: () => {
        this.toastService.success(editing ? 'Tax slab updated.' : 'Tax slab created.');
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