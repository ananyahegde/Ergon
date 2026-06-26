import { Component, inject, signal, computed } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MasterService } from '../../../../core/services/master.service';
import { ToastService } from '../../../../core/services/toast.service';
import { Branch } from '../../../../core/models/master.model';

@Component({
  selector: 'app-branches-tab',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './branches-tab.html',
  styleUrl: './branches-tab.css'
})
export class BranchesTab {
  private masterService = inject(MasterService);
  private toastService = inject(ToastService);

  branches = this.masterService.branches;

  search = signal('');
  showForm = signal(false);
  editingBranch = signal<Branch | null>(null);
  formName = signal('');
  saving = signal(false);

  filtered = computed(() => {
    const q = this.search().toLowerCase().trim();
    if (!q) return this.branches();
    return this.branches().filter(b => b.branchName.toLowerCase().includes(q));
  });

  openAdd() {
    this.editingBranch.set(null);
    this.formName.set('');
    this.showForm.set(true);
  }

  openEdit(branch: Branch) {
    this.editingBranch.set(branch);
    this.formName.set(branch.branchName);
    this.showForm.set(true);
  }

  closeForm() {
    this.showForm.set(false);
    this.editingBranch.set(null);
    this.formName.set('');
  }

  save() {
    const name = this.formName().trim();
    if (!name || name.length < 2) return;

    this.saving.set(true);
    const editing = this.editingBranch();

    const request$ = editing
      ? this.masterService.updateBranch(editing.branchId, { branchName: name })
      : this.masterService.createBranch({ branchName: name });

    request$.subscribe({
      next: () => {
        this.toastService.success(editing ? 'Branch updated.' : 'Branch created.');
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