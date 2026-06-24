import { Component, input, output, signal, computed, HostListener, ElementRef, inject } from '@angular/core';

@Component({
  selector: 'app-multi-select-dropdown',
  standalone: true,
  imports: [],
  templateUrl: './multi-select-dropdown.html',
  styleUrl: './multi-select-dropdown.css'
})
export class MultiSelectDropdown {
  private elementRef = inject(ElementRef);

  options = input<any[]>([]);
  labelKey = input<string>('label');
  valueKey = input<string>('value');
  label = input<string>('Select');
  selectedValues = input<any[]>([]);

  selectionChange = output<any[]>();

  isOpen = signal(false);

  buttonLabel = computed(() => {
    const count = this.selectedValues().length;
    return count > 0 ? `${this.label()} (${count})` : this.label();
  });

  hasSelection = computed(() => this.selectedValues().length > 0);

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent) {
    if (!this.elementRef.nativeElement.contains(event.target)) {
      this.isOpen.set(false);
    }
  }

  toggle() {
    this.isOpen.update(v => !v);
  }

  isSelected(value: any): boolean {
    return this.selectedValues().includes(value);
  }

  onToggleOption(value: any) {
    const current = this.selectedValues();
    const updated = current.includes(value)
      ? current.filter(v => v !== value)
      : [...current, value];
    this.selectionChange.emit(updated);
  }

  clearAll() {
    this.selectionChange.emit([]);
  }
}
