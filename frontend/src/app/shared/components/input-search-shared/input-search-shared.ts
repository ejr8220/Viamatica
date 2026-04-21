import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';

@Component({
  selector: 'app-input-search-shared',
  standalone: false,
  templateUrl: './input-search-shared.html',
  styleUrl: './input-search-shared.scss',
})
export class InputSearchShared implements OnInit, OnChanges {
  @Input() label = 'Buscar';
  @Input() placeholder = 'Seleccione...';
  @Input() disabled = false;
  @Input() required = false;
  @Input() dialogHeader = 'Búsqueda';
  @Input() displayField = 'name';
  @Input() searchFields: string[] = ['name'];
  @Input() columns: { field: string; header: string }[] = [];
  @Input() data: any[] = [];
  @Input() loading = false;
  @Input() value: any | null = null;

  @Output() valueChange = new EventEmitter<any | null>();
  @Output() search = new EventEmitter<void>();

  displayValue = '';
  selectedItem: any | null = null;
  dialogSelection: any | null = null;
  visible = false;
  searchText = '';

  ngOnInit(): void {
    if (this.columns.length === 0) {
      this.columns = [{ field: this.displayField, header: 'Nombre' }];
    }

    this.syncValue(this.value);
  }

  ngOnChanges(changes: SimpleChanges): void {
    if ('value' in changes) {
      this.syncValue(this.value);
    }
  }

  openDialog(): void {
    if (!this.disabled) {
      this.dialogSelection = this.selectedItem;
      this.searchText = '';
      this.visible = true;
      this.search.emit();
    }
  }

  confirmSelection(): void {
    this.selectedItem = this.dialogSelection;
    this.displayValue = this.resolveDisplayValue(this.dialogSelection);
    this.visible = false;
    this.valueChange.emit(this.dialogSelection);
  }

  cancelDialog(): void {
    this.dialogSelection = this.selectedItem;
    this.visible = false;
  }

  clear(): void {
    this.selectedItem = null;
    this.dialogSelection = null;
    this.displayValue = '';
    this.valueChange.emit(null);
  }

  handleAction(): void {
    if (this.selectedItem) {
      this.clear();
      return;
    }

    this.openDialog();
  }

  get actionIcon(): string {
    return this.selectedItem ? 'pi pi-times' : 'pi pi-search';
  }

  get actionButtonClass(): string {
    return this.selectedItem ? 'p-button-danger' : 'p-button-primary';
  }

  get filteredData(): any[] {
    if (!this.searchText) {
      return this.data;
    }

    const search = this.searchText.toLowerCase();
    return this.data.filter((item) =>
      this.searchFields.some((field) => String(item[field] ?? '').toLowerCase().includes(search))
    );
  }

  private syncValue(item: any | null): void {
    this.selectedItem = item;
    this.dialogSelection = item;
    this.displayValue = item ? this.resolveDisplayValue(item) : '';
  }

  private resolveDisplayValue(item: any | null): string {
    return item ? String(item[this.displayField] ?? '') : '';
  }
}
