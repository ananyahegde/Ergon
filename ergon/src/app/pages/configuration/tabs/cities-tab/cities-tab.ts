import { Component, inject, signal, computed } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MasterService } from '../../../../core/services/master.service';
import { ToastService } from '../../../../core/services/toast.service';
import { City } from '../../../../core/models/master.model';

@Component({
  selector: 'app-cities-tab',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './cities-tab.html',
  styleUrl: './cities-tab.css'
})
export class CitiesTab {
  private masterService = inject(MasterService);
  private toastService = inject(ToastService);

  cities = this.masterService.cities;

  search = signal('');
  showForm = signal(false);
  editingCity = signal<City | null>(null);
  formName = signal('');
  saving = signal(false);

  filtered = computed(() => {
    const q = this.search().toLowerCase().trim();
    if (!q) return this.cities();
    return this.cities().filter(d => d.cityName.toLowerCase().includes(q));
  });

  openAdd() {
    this.editingCity.set(null);
    this.formName.set('');
    this.showForm.set(true);
  }

  openEdit(city: City) {
    this.editingCity.set(city);
    this.formName.set(city.cityName);
    this.showForm.set(true);
  }

  closeForm() {
    this.showForm.set(false);
    this.editingCity.set(null);
    this.formName.set('');
  }

  save() {
    const name = this.formName().trim();
    if (!name || name.length < 2) return;

    this.saving.set(true);
    const editing = this.editingCity();

    const request$ = editing
      ? this.masterService.updateCity(editing.cityId, { cityName: name })
      : this.masterService.createCity({ cityName: name });

    request$.subscribe({
      next: () => {
        this.toastService.success(editing ? 'City updated.' : 'City created.');
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