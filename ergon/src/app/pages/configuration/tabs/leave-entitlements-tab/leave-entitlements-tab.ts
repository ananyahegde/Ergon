import { Component, inject, signal, computed } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MasterService } from '../../../../core/services/master.service';
import { ToastService } from '../../../../core/services/toast.service';
import { LeaveEntitlement, LeaveEntitlementComponent } from '../../../../core/models/master.model';
import { SingleSelectDropdown } from '../../../../shared/components/single-select-dropdown/single-select-dropdown';

@Component({
  selector: 'app-leave-entitlements-tab',
  standalone: true,
  imports: [FormsModule, SingleSelectDropdown],
  templateUrl: './leave-entitlements-tab.html',
  styleUrl: './leave-entitlements-tab.css'
})
export class LeaveEntitlementsTab {
  private masterService = inject(MasterService);
  private toastService = inject(ToastService);

  leaveEntitlements = this.masterService.leaveEntitlements;
  leaveEntitlementComponents = this.masterService.leaveEntitlementComponents;
  leaveTypes = this.masterService.leaveTypes;

  search = signal('');
  showForm = signal(false);
  editingEntitlement = signal<LeaveEntitlement | null>(null);
  formName = signal('');
  saving = signal(false);

  expandedId = signal<number | null>(null);

  showComponentForm = signal(false);
  editingComponent = signal<LeaveEntitlementComponent | null>(null);
  formLeaveTypeId = signal<number | null>(null);
  formTotalDays = signal<number | null>(null);
  savingComponent = signal(false);

  filtered = computed(() => {
    const q = this.search().toLowerCase().trim();
    if (!q) return this.leaveEntitlements();
    return this.leaveEntitlements().filter(l => l.leaveEntitlementName.toLowerCase().includes(q));
  });

  componentsFor(id: number) {
    return this.leaveEntitlementComponents()[id] ?? [];
  }

  isComponentFormValid = computed(() =>
    this.formLeaveTypeId() !== null && this.formTotalDays() !== null && this.formTotalDays()! >= 1
  );

  openAdd() {
    this.editingEntitlement.set(null);
    this.formName.set('');
    this.showForm.set(true);
  }

  openEdit(entitlement: LeaveEntitlement) {
    this.editingEntitlement.set(entitlement);
    this.formName.set(entitlement.leaveEntitlementName);
    this.showForm.set(true);
  }

  closeForm() {
    this.showForm.set(false);
    this.editingEntitlement.set(null);
    this.formName.set('');
  }

  save() {
    const name = this.formName().trim();
    if (!name || name.length < 2) return;

    this.saving.set(true);
    const editing = this.editingEntitlement();

    const request$ = editing
      ? this.masterService.updateLeaveEntitlement(editing.leaveEntitlementId, { leaveEntitlementName: name })
      : this.masterService.createLeaveEntitlement({ leaveEntitlementName: name });

    request$.subscribe({
      next: () => {
        this.toastService.success(editing ? 'Leave entitlement updated.' : 'Leave entitlement created.');
        this.closeForm();
        this.saving.set(false);
      },
      error: (err) => {
        this.toastService.error(err?.error?.message ?? 'Something went wrong.');
        this.saving.set(false);
      }
    });
  }

  toggleExpand(id: number) {
    this.expandedId.set(this.expandedId() === id ? null : id);
    this.closeComponentForm();
  }

  openAddComponent() {
    this.editingComponent.set(null);
    this.formLeaveTypeId.set(null);
    this.formTotalDays.set(null);
    this.showComponentForm.set(true);
  }

  openEditComponent(component: LeaveEntitlementComponent) {
    this.editingComponent.set(component);
    this.formLeaveTypeId.set(null);
    this.formTotalDays.set(component.totalDays);
    this.showComponentForm.set(true);
  }

  closeComponentForm() {
    this.showComponentForm.set(false);
    this.editingComponent.set(null);
    this.formLeaveTypeId.set(null);
    this.formTotalDays.set(null);
  }

  saveComponent() {
    if (!this.isComponentFormValid()) return;
    const entitlementId = this.expandedId();
    if (!entitlementId) return;

    this.savingComponent.set(true);
    const editing = this.editingComponent();

    const request$ = editing
      ? this.masterService.updateLeaveEntitlementComponent(entitlementId, editing.leaveEntitlementComponentId, { totalDays: this.formTotalDays()! })
      : this.masterService.createLeaveEntitlementComponent(entitlementId, { leaveTypeId: this.formLeaveTypeId()!, totalDays: this.formTotalDays()! });

    request$.subscribe({
      next: () => {
        this.toastService.success(editing ? 'Component updated.' : 'Component added.');
        this.closeComponentForm();
        this.savingComponent.set(false);
      },
      error: (err) => {
        this.toastService.error(err?.error?.message ?? 'Something went wrong.');
        this.savingComponent.set(false);
      }
    });
  }
}