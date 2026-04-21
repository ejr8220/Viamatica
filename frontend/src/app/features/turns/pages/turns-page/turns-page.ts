import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { ConfirmationService } from 'primeng/api';

import { NotificationService } from '../../../../core/services';
import { GridAction, GridColumn } from '../../../../shared/components/grid-shared/grid-shared';
import { AttentionTypeOption, CashOption, TurnFormValue, TurnsService, TurnViewModel } from '../../services/turns.service';

@Component({
  selector: 'app-turns-page',
  standalone: false,
  templateUrl: './turns-page.html',
  styleUrl: './turns-page.scss',
})
export class TurnsPage implements OnInit {
  turns: TurnViewModel[] = [];
  cashOptions: CashOption[] = [];
  attentionTypeOptions: AttentionTypeOption[] = [];
  loading = false;
  dialogVisible = false;
  saving = false;
  searchTerm = '';
  editingTurn: TurnViewModel | null = null;
  private readonly fb = inject(FormBuilder);

  readonly columns: GridColumn[] = [
    { field: 'description', header: 'Turno', sortable: true, filterable: true },
    { field: 'attentionTypeDescription', header: 'Tipo atención', sortable: true, filterable: true },
    { field: 'date', header: 'Fecha', sortable: true, pipe: 'date' },
    { field: 'cashDescription', header: 'Caja', sortable: true, filterable: true },
    { field: 'gestorUserName', header: 'Gestor', sortable: true, filterable: true },
  ];

  readonly actions: GridAction[] = [
    {
      icon: 'pi pi-pencil',
      tooltip: 'Editar',
      callback: (row) => this.openEditDialog(row as TurnViewModel),
      styleClass: 'p-button-rounded p-button-text',
    },
    {
      icon: 'pi pi-trash',
      tooltip: 'Eliminar',
      callback: (row) => this.confirmDelete(row as TurnViewModel),
      styleClass: 'p-button-rounded p-button-text p-button-danger',
    },
  ];

  readonly form = this.fb.nonNullable.group({
    attentionTypeId: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(3)]],
    date: ['', [Validators.required]],
    cashId: [0, [Validators.required, Validators.min(1)]],
  });

  constructor(
    private readonly turnsService: TurnsService,
    private readonly notificationService: NotificationService,
    private readonly confirmationService: ConfirmationService
  ) {}

  ngOnInit(): void {
    this.loadData();
    this.turnsService.getCashOptions().subscribe((items) => {
      this.cashOptions = items;
    });
    this.turnsService.getAttentionTypeOptions().subscribe((items) => {
      this.attentionTypeOptions = items;
    });
  }

  get filteredTurns(): TurnViewModel[] {
    const term = this.searchTerm.trim().toLowerCase();
    if (!term) {
      return this.turns;
    }

    return this.turns.filter((turn) =>
      [turn.description, turn.attentionTypeDescription, turn.cashDescription, turn.gestorUserName].some((value) =>
        value.toLowerCase().includes(term)
      )
    );
  }

  openCreateDialog(): void {
    this.editingTurn = null;
    this.dialogVisible = true;
    this.form.reset({ attentionTypeId: '', date: new Date().toISOString().slice(0, 10), cashId: 0 });
  }

  openEditDialog(turn: TurnViewModel): void {
    this.editingTurn = turn;
    this.dialogVisible = true;
    this.form.reset({
      attentionTypeId: turn.attentionTypeId,
      date: turn.date.slice(0, 10),
      cashId: turn.cashId,
    });
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.saving = true;
    const payload = this.form.getRawValue() as TurnFormValue;
    const request$ = this.editingTurn
      ? this.turnsService.update(this.editingTurn.turnId, payload)
      : this.turnsService.create(payload);

    request$.subscribe({
      next: () => {
        this.notificationService.success(this.editingTurn ? 'Turno actualizado.' : 'Turno creado.');
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

  confirmDelete(turn: TurnViewModel): void {
    this.confirmationService.confirm({
      header: 'Eliminar turno',
      message: `¿Desea eliminar ${turn.description}?`,
      accept: () => {
        this.turnsService.delete(turn.turnId).subscribe(() => {
          this.notificationService.success('Turno eliminado correctamente.');
          this.loadData();
        });
      },
    });
  }

  private loadData(): void {
    this.loading = true;
    this.turnsService.getAll().subscribe({
      next: (turns) => {
        this.turns = turns;
      },
      complete: () => {
        this.loading = false;
      },
    });
  }
}
