import { Component, inject, signal, computed } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MasterService } from '../../../../core/services/master.service';
import { ToastService } from '../../../../core/services/toast.service';
import { Country } from '../../../../core/models/master.model';

@Component({
  selector: 'app-countries-tab',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './countries-tab.html',
  styleUrl: './countries-tab.css'
})
export class CountriesTab {
  private masterService = inject(MasterService);
  private toastService = inject(ToastService);

  countries = this.masterService.countries;

  search = signal('');
  showForm = signal(false);
  editingCountry = signal<Country | null>(null);
  formName = signal('');
  saving = signal(false);

  filtered = computed(() => {
    const q = this.search().toLowerCase().trim();
    if (!q) return this.countries();
    return this.countries().filter(d => d.countryName.toLowerCase().includes(q));
  });

  openAdd() {
    this.editingCountry.set(null);
    this.formName.set('');
    this.showForm.set(true);
  }

  openEdit(country: Country) {
    this.editingCountry.set(country);
    this.formName.set(country.countryName);
    this.showForm.set(true);
  }

  closeForm() {
    this.showForm.set(false);
    this.editingCountry.set(null);
    this.formName.set('');
  }

  save() {
    const name = this.formName().trim();
    if (!name || name.length < 2) return;

    this.saving.set(true);
    const editing = this.editingCountry();

    const request$ = editing
      ? this.masterService.updateCountry(editing.countryId, { countryName: name })
      : this.masterService.createCountry({ countryName: name });

    request$.subscribe({
      next: () => {
        this.toastService.success(editing ? 'Country updated.' : 'Country created.');
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