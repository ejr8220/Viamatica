import { Component, Input, Output, EventEmitter, OnInit, ViewChild } from '@angular/core';

export interface GridColumn {
  field: string;
  header: string;
  sortable?: boolean;
  filterable?: boolean;
  filterType?: 'text' | 'date' | 'number';
  pipe?: any;
  width?: string;
}

export interface GridAction {
  icon: string;
  tooltip: string;
  callback: (row: any) => void;
  visible?: (row: any) => boolean;
  styleClass?: string;
}

@Component({
  selector: 'app-grid-shared',
  standalone: false,
  templateUrl: './grid-shared.html',
  styleUrl: './grid-shared.scss',
})
export class GridShared implements OnInit {
  @ViewChild('dt') table: any;

  @Input() columns: GridColumn[] = [];
  @Input() data: any[] = [];
  @Input() loading: boolean = false;
  @Input() paginator: boolean = true;
  @Input() rows: number = 10;
  @Input() rowsPerPageOptions: number[] = [5, 10, 20, 50];
  @Input() actions: GridAction[] = [];
  @Input() globalFilterFields: string[] = [];
  @Input() selectionMode: 'single' | 'multiple' | null = null;

  @Output() rowSelect = new EventEmitter<any>();
  @Output() rowUnselect = new EventEmitter<any>();
  @Output() selectionChange = new EventEmitter<any>();

  selectedRows: any = null;
  columnFilters: { [key: string]: string } = {};

  ngOnInit(): void {
    if (this.globalFilterFields.length === 0) {
      this.globalFilterFields = this.columns.map((col) => col.field);
    }
  }

  onFilter(event: any, field: string): void {
    this.table.filter(event.target.value, field, 'contains');
  }

  clearFilters(): void {
    this.columnFilters = {};
    if (this.table) {
      this.table.clear();
    }
  }

  onRowSelect(event: any): void {
    this.rowSelect.emit(event.data);
  }

  onRowUnselect(event: any): void {
    this.rowUnselect.emit(event.data);
  }

  onSelectionChange(value: any): void {
    this.selectionChange.emit(value);
  }

  isActionVisible(action: GridAction, row: any): boolean {
    return action.visible ? action.visible(row) : true;
  }

  executeAction(action: GridAction, row: any, event: Event): void {
    event.stopPropagation();
    action.callback(row);
  }
}
