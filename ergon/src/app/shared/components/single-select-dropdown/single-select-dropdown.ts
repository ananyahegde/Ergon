import { Component, input, output, signal, computed, HostListener, ElementRef, inject } from '@angular/core';

@Component({
  selector: 'app-single-select-dropdown',
  standalone: true,
  imports: [],
  templateUrl: './single-select-dropdown.html',
  styleUrl: './single-select-dropdown.css'
})
export class SingleSelectDropdown {
  private elementRef = inject(ElementRef);

  options = input<any[]>([]);
  labelKey = input<string>('label');
  valueKey = input<string>('value');
  label = input<string>('Select');
  selectedValue = input<any>(null);
  hasError = input<boolean>(false);

  selectionChange = output<any>();

  isOpen = signal(false);
  clearable = input<boolean>(true);

  buttonLabel = computed(() => {
    const val = this.selectedValue();
    if (val === null || val === undefined || val === '') return this.label();
    const found = this.options().find(o => o[this.valueKey()] === val);
    return found ? found[this.labelKey()] : this.label();
  });

  hasSelection = computed(() => {
    const val = this.selectedValue();
    return val !== null && val !== undefined && val !== '';
  });

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent) {
    if (!this.elementRef.nativeElement.contains(event.target)) {
      this.isOpen.set(false);
    }
  }

  toggle() {
    this.isOpen.update(v => !v);
  }

  select(value: any) {
    this.selectionChange.emit(value);
    this.isOpen.set(false);
  }

  clear() {
    this.selectionChange.emit(null);
    this.isOpen.set(false);
  }
}