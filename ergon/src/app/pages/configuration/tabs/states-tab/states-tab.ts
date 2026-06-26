import { Component, inject, signal, computed } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MasterService } from '../../../../core/services/master.service';
import { ToastService } from '../../../../core/services/toast.service';
import { State } from '../../../../core/models/master.model';

@Component({
  selector: 'app-states-tab',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './states-tab.html',
  styleUrl: './states-tab.css'
})
export class StatesTab {
  private masterService = inject(MasterService);
  private toastService = inject(ToastService);

  states = this.masterService.states;

  search = signal('');
  showForm = signal(false);
  editingState = signal<State | null>(null);
  formName = signal('');
  saving = signal(false);

  filtered = computed(() => {
    const q = this.search().toLowerCase().trim();
    if (!q) return this.states();
    return this.states().filter(d => d.stateName.toLowerCase().includes(q));
  });

  openAdd() {
    this.editingState.set(null);
    this.formName.set('');
    this.showForm.set(true);
  }

  openEdit(state: State) {
    this.editingState.set(state);
    this.formName.set(state.stateName);
    this.showForm.set(true);
  }

  closeForm() {
    this.showForm.set(false);
    this.editingState.set(null);
    this.formName.set('');
  }

  save() {
    const name = this.formName().trim();
    if (!name || name.length < 2) return;

    this.saving.set(true);
    const editing = this.editingState();

    const request$ = editing
      ? this.masterService.updateState(editing.stateId, { stateName: name })
      : this.masterService.createState({ stateName: name });

    request$.subscribe({
      next: () => {
        this.toastService.success(editing ? 'State updated.' : 'State created.');
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