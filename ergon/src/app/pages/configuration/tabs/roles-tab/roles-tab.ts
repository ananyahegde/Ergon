import { Component, inject, signal, computed } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MasterService } from '../../../../core/services/master.service';
import { ToastService } from '../../../../core/services/toast.service';
import { Role } from '../../../../core/models/master.model';

@Component({
  selector: 'app-roles-tab',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './roles-tab.html',
  styleUrl: './roles-tab.css'
})
export class RolesTab {
  private masterService = inject(MasterService);
  private toastService = inject(ToastService);

  roles = this.masterService.roles;

  search = signal('');
  showForm = signal(false);
  editingRole = signal<Role | null>(null);
  formName = signal('');
  saving = signal(false);

  filtered = computed(() => {
    const q = this.search().toLowerCase().trim();
    if (!q) return this.roles();
    return this.roles().filter(b => b.roleName.toLowerCase().includes(q));
  });

  openAdd() {
    this.editingRole.set(null);
    this.formName.set('');
    this.showForm.set(true);
  }

  openEdit(role: Role) {
    this.editingRole.set(role);
    this.formName.set(role.roleName);
    this.showForm.set(true);
  }

  closeForm() {
    this.showForm.set(false);
    this.editingRole.set(null);
    this.formName.set('');
  }

  save() {
    const name = this.formName().trim();
    if (!name || name.length < 2) return;

    this.saving.set(true);
    const editing = this.editingRole();

    const request$ = editing
      ? this.masterService.updateRole(editing.roleId, { roleName: name })
      : this.masterService.createRole({ roleName: name });

    request$.subscribe({
      next: () => {
        this.toastService.success(editing ? 'Role updated.' : 'Role created.');
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