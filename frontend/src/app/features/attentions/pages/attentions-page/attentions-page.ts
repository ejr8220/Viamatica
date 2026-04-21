import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';

import { NotificationService } from '../../../../core/services';
import { GridAction, GridColumn } from '../../../../shared/components/grid-shared/grid-shared';
import {
  AttentionClientOption,
  AttentionContractOption,
  AttentionTurnOption,
  AttentionsService,
  AttentionTypeOption,
  AttentionViewModel,
  StartAttentionFormValue,
} from '../../services/attentions.service';

@Component({
  selector: 'app-attentions-page',
  standalone: false,
  templateUrl: './attentions-page.html',
  styleUrl: './attentions-page.scss',
})
export class AttentionsPage implements OnInit {
  attentions: AttentionViewModel[] = [];
  typeOptions: AttentionTypeOption[] = [];
  turnOptions: AttentionTurnOption[] = [];
  clientOptions: AttentionClientOption[] = [];
  contractOptions: AttentionContractOption[] = [];
  loading = false;
  dialogVisible = false;
  closeDialogVisible = false;
  saving = false;
  closeSaving = false;
  searchTerm = '';
  closeMode: 'complete' | 'cancel' = 'complete';
  selectedAttention: AttentionViewModel | null = null;
  private readonly fb = inject(FormBuilder);

  readonly columns: GridColumn[] = [
    { field: 'turnNumber', header: 'Turno', sortable: true, filterable: true },
    { field: 'clientName', header: 'Cliente', sortable: true, filterable: true },
    { field: 'serviceName', header: 'Tipo atención', sortable: true, filterable: true },
    { field: 'userName', header: 'Cajero', sortable: true, filterable: true },
    { field: 'status', header: 'Estado', sortable: true, filterable: true },
    { field: 'startTime', header: 'Inicio', sortable: true, pipe: 'date' },
  ];

  readonly actions: GridAction[] = [
    {
      icon: 'pi pi-check',
      tooltip: 'Completar',
      callback: (row) => this.openCloseDialog(row as AttentionViewModel, 'complete'),
      visible: (row) => !(row as AttentionViewModel).isClosed,
      styleClass: 'p-button-rounded p-button-text p-button-success',
    },
    {
      icon: 'pi pi-ban',
      tooltip: 'Cancelar',
      callback: (row) => this.openCloseDialog(row as AttentionViewModel, 'cancel'),
      visible: (row) => !(row as AttentionViewModel).isClosed,
      styleClass: 'p-button-rounded p-button-text p-button-danger',
    },
  ];

  readonly form = this.fb.nonNullable.group({
    turnId: [0, [Validators.required, Validators.min(1)]],
    clientId: [0, [Validators.required, Validators.min(1)]],
    contractId: [0],
    attentionTypeId: ['', [Validators.required]],
    notes: [''],
  });

  readonly closeForm = this.fb.nonNullable.group({
    notes: [''],
  });

  constructor(
    private readonly attentionsService: AttentionsService,
    private readonly notificationService: NotificationService
  ) {}

  ngOnInit(): void {
    this.loadData();
    this.attentionsService.getTypes().subscribe((items) => {
      this.typeOptions = items;
    });
    this.attentionsService.getTurnOptions().subscribe((items) => {
      this.turnOptions = items;
    });
    this.attentionsService.getClientOptions().subscribe((items) => {
      this.clientOptions = items;
    });
  }

  get filteredAttentions(): AttentionViewModel[] {
    const term = this.searchTerm.trim().toLowerCase();
    if (!term) {
      return this.attentions;
    }

    return this.attentions.filter((attention) =>
      [attention.turnNumber, attention.clientName, attention.serviceName, attention.userName, attention.status].some(
        (value) => value.toLowerCase().includes(term)
      )
    );
  }

  get openCount(): number {
    return this.attentions.filter((item) => !item.isClosed).length;
  }

  get closedCount(): number {
    return this.attentions.filter((item) => item.isClosed).length;
  }

  openCreateDialog(): void {
    this.dialogVisible = true;
    this.form.reset({ turnId: 0, clientId: 0, contractId: 0, attentionTypeId: '', notes: '' });
    this.contractOptions = [];
  }

  onClientChange(): void {
    const clientId = this.form.controls.clientId.value;
    this.form.controls.contractId.setValue(0);
    if (!clientId) {
      this.contractOptions = [];
      return;
    }

    this.attentionsService.getContractOptions(clientId).subscribe((items) => {
      this.contractOptions = items.filter((item) => item.clientId === clientId);
    });
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.saving = true;
    const rawValue = this.form.getRawValue();
    const payload: StartAttentionFormValue = {
      turnId: rawValue.turnId,
      clientId: rawValue.clientId,
      contractId: rawValue.contractId || null,
      attentionTypeId: rawValue.attentionTypeId,
      notes: rawValue.notes,
    };

    this.attentionsService.start(payload).subscribe({
      next: () => {
        this.notificationService.success('Atención iniciada correctamente.');
        this.dialogVisible = false;
        this.loadData();
      },
      error: () => {
        this.saving = false;
      },
      complete: () => {
        this.saving = false;
      },
    });
  }

  openCloseDialog(attention: AttentionViewModel, mode: 'complete' | 'cancel'): void {
    this.selectedAttention = attention;
    this.closeMode = mode;
    this.closeDialogVisible = true;
    this.closeForm.reset({ notes: attention.notes ?? '' });
  }

  submitClose(): void {
    if (!this.selectedAttention) {
      return;
    }

    this.closeSaving = true;
    const request$ = this.closeMode === 'complete'
      ? this.attentionsService.complete(this.selectedAttention.attentionId, this.closeForm.controls.notes.value)
      : this.attentionsService.cancel(this.selectedAttention.attentionId, this.closeForm.controls.notes.value);

    request$.subscribe({
      next: () => {
        this.notificationService.success(
          this.closeMode === 'complete' ? 'Atención completada correctamente.' : 'Atención cancelada correctamente.'
        );
        this.closeDialogVisible = false;
        this.loadData();
      },
      error: () => {
        this.closeSaving = false;
      },
      complete: () => {
        this.closeSaving = false;
      },
    });
  }

  private loadData(): void {
    this.loading = true;
    this.attentionsService.getAll().subscribe({
      next: (attentions) => {
        this.attentions = attentions;
      },
      complete: () => {
        this.loading = false;
      },
    });
  }
}
