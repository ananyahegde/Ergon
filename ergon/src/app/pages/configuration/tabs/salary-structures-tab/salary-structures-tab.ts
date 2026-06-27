import { Component, inject, signal, computed } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MasterService } from '../../../../core/services/master.service';
import { ToastService } from '../../../../core/services/toast.service';
import { SalaryStructure, SalaryComponent } from '../../../../core/models/master.model';
import { SingleSelectDropdown } from '../../../../shared/components/single-select-dropdown/single-select-dropdown';
import { DecimalPipe } from '@angular/common';

const COMPONENT_TYPES = [
  { label: 'Earning', value: 'Earning' },
  { label: 'Deduction', value: 'Deduction' }
];

@Component({
  selector: 'app-salary-structures-tab',
  standalone: true,
  imports: [FormsModule, SingleSelectDropdown, DecimalPipe],
  templateUrl: './salary-structures-tab.html',
  styleUrl: './salary-structures-tab.css'
})
export class SalaryStructuresTab {
  private masterService = inject(MasterService);
  private toastService = inject(ToastService);

  salaryStructures = this.masterService.salaryStructures;
  salaryComponents = this.masterService.salaryComponents;

  readonly componentTypes = COMPONENT_TYPES;

  search = signal('');
  showForm = signal(false);
  editingStructure = signal<SalaryStructure | null>(null);
  formName = signal('');
  saving = signal(false);

  expandedId = signal<number | null>(null);

  showComponentForm = signal(false);
  editingComponent = signal<SalaryComponent | null>(null);
  formComponentName = signal('');
  formComponentType = signal<string | null>(null);
  formAmount = signal<number | null>(null);
  savingComponent = signal(false);

  filtered = computed(() => {
    const q = this.search().toLowerCase().trim();
    if (!q) return this.salaryStructures();
    return this.salaryStructures().filter(s => s.salaryStructureName.toLowerCase().includes(q));
  });

  componentsFor(id: number) {
    return this.salaryComponents()[id] ?? [];
  }

  isComponentFormValid = computed(() =>
    this.formComponentName().trim().length >= 2 &&
    this.formComponentType() !== null &&
    this.formAmount() !== null &&
    this.formAmount()! >= 0
  );

  openAdd() {
    this.editingStructure.set(null);
    this.formName.set('');
    this.showForm.set(true);
  }

  openEdit(structure: SalaryStructure) {
    this.editingStructure.set(structure);
    this.formName.set(structure.salaryStructureName);
    this.showForm.set(true);
  }

  closeForm() {
    this.showForm.set(false);
    this.editingStructure.set(null);
    this.formName.set('');
  }

  save() {
    const name = this.formName().trim();
    if (!name || name.length < 2) return;

    this.saving.set(true);
    const editing = this.editingStructure();

    const request$ = editing
      ? this.masterService.updateSalaryStructure(editing.salaryStructureId, { salaryStructureName: name })
      : this.masterService.createSalaryStructure({ salaryStructureName: name });

    request$.subscribe({
      next: () => {
        this.toastService.success(editing ? 'Salary structure updated.' : 'Salary structure created.');
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
    this.formComponentName.set('');
    this.formComponentType.set(null);
    this.formAmount.set(null);
    this.showComponentForm.set(true);
  }

  openEditComponent(component: SalaryComponent) {
    this.editingComponent.set(component);
    this.formComponentName.set(component.componentName);
    this.formComponentType.set(component.componentType);
    this.formAmount.set(component.amount);
    this.showComponentForm.set(true);
  }

  closeComponentForm() {
    this.showComponentForm.set(false);
    this.editingComponent.set(null);
    this.formComponentName.set('');
    this.formComponentType.set(null);
    this.formAmount.set(null);
  }

  saveComponent() {
    if (!this.isComponentFormValid()) return;
    const structureId = this.expandedId();
    if (!structureId) return;

    this.savingComponent.set(true);
    const editing = this.editingComponent();
    const payload = {
      componentName: this.formComponentName().trim(),
      componentType: this.formComponentType()!,
      amount: this.formAmount()!
    };

    const request$ = editing
      ? this.masterService.updateSalaryComponent(structureId, editing.salaryComponentId, payload)
      : this.masterService.createSalaryComponent(structureId, payload);

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