import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { ConfirmationService } from 'primeng/api';

import { AuthService, NotificationService } from '../../../../core/services';
import { GridAction, GridColumn } from '../../../../shared/components/grid-shared/grid-shared';
import { CashesService, CashFormValue, CashierOption, CashViewModel } from '../../services/cashes.service';

@Component({
  selector: 'app-cashes-page',
  standalone: false,
  templateUrl: './cashes-page.html',
  styleUrl: './cashes-page.scss',
})
export class CashesPage implements OnInit {
  cashes: CashViewModel[] = [];
  cashierOptions: CashierOption[] = [];
  loading = false;
  dialogVisible = false;
  assignmentDialogVisible = false;
  saving = false;
  searchTerm = '';
  selectedCashierId: number | null = null;
  editingCash: CashViewModel | null = null;
  assignmentCash: CashViewModel | null = null;
  readonly currentRole: string | null;
  readonly currentUserName: string | null;
  private readonly fb = inject(FormBuilder);

  readonly columns: GridColumn[] = [
    { field: 'name', header: 'Caja', sortable: true, filterable: true },
    { field: 'code', header: 'Código', sortable: true },
    { field: 'description', header: 'Descripción', sortable: true, filterable: true },
    { field: 'assignedCount', header: 'Cajeros asignados', sortable: true },
    { field: 'sessionStatus', header: 'Sesión', sortable: true },
  ];

  readonly actions: GridAction[] = [
    {
      icon: 'pi pi-pencil',
      tooltip: 'Editar',
      callback: (row) => this.openEditDialog(row as CashViewModel),
      visible: () => this.canManageCashes,
      styleClass: 'p-button-rounded p-button-text',
    },
    {
      icon: 'pi pi-user-plus',
      tooltip: 'Asignar cajero',
      callback: (row) => this.openAssignmentDialog(row as CashViewModel),
      visible: () => this.canManageCashes,
      styleClass: 'p-button-rounded p-button-text p-button-info',
    },
    {
      icon: 'pi pi-lock-open',
      tooltip: 'Abrir sesión',
      callback: (row) => this.openSession(row as CashViewModel),
      visible: (row) => this.canOperateCash(row as CashViewModel) && !(row as CashViewModel).activeSession?.isActive,
      styleClass: 'p-button-rounded p-button-text p-button-success',
    },
    {
      icon: 'pi pi-lock',
      tooltip: 'Cerrar sesión',
      callback: (row) => this.closeSession(row as CashViewModel),
      visible: (row) => this.canOperateCash(row as CashViewModel) && Boolean((row as CashViewModel).activeSession?.isActive),
      styleClass: 'p-button-rounded p-button-text p-button-warning',
    },
    {
      icon: 'pi pi-trash',
      tooltip: 'Eliminar',
      callback: (row) => this.confirmDelete(row as CashViewModel),
      visible: () => this.canManageCashes,
      styleClass: 'p-button-rounded p-button-text p-button-danger',
    },
  ];

  readonly form = this.fb.nonNullable.group({
    description: ['', [Validators.required, Validators.minLength(3)]],
    active: [true],
  });

  constructor(
    private readonly cashesService: CashesService,
    private readonly authService: AuthService,
    private readonly notificationService: NotificationService,
    private readonly confirmationService: ConfirmationService
  ) {
    this.currentRole = this.authService.getUserRole();
    this.currentUserName = this.authService.getUserName();
  }

  ngOnInit(): void {
    this.loadData();
    if (this.canManageCashes) {
      this.cashesService.getCashierOptions().subscribe((cashiers) => {
        this.cashierOptions = cashiers;
      });
    }
  }

  get filteredCashes(): Array<CashViewModel & { assignedCount: number; sessionStatus: string }> {
    const source = this.canManageCashes
      ? this.cashes
      : this.cashes.filter((cash) => this.isAssignedToCurrentUser(cash));

    const mapped = source.map((cash) => ({
      ...cash,
      assignedCount: cash.assignedCashiers.length,
      sessionStatus: cash.activeSession?.isActive ? `Activa · ${cash.activeSession.userName}` : 'Sin sesión activa',
    }));

    const term = this.searchTerm.trim().toLowerCase();
    if (!term) {
      return mapped;
    }

    return mapped.filter((cash) =>
      [cash.name, cash.code, cash.description, cash.sessionStatus].some((value) => value.toLowerCase().includes(term))
    );
  }

  get activeCount(): number {
    return this.cashes.filter((cash) => cash.isActive).length;
  }

  get sessionCount(): number {
    return this.cashes.filter((cash) => Boolean(cash.activeSession?.isActive)).length;
  }

  get canManageCashes(): boolean {
    return this.currentRole === 'Administrador' || this.currentRole === 'Gestor';
  }

  openCreateDialog(): void {
    this.editingCash = null;
    this.dialogVisible = true;
    this.form.reset({ description: '', active: true });
  }

  openEditDialog(cash: CashViewModel): void {
    this.editingCash = cash;
    this.dialogVisible = true;
    this.form.reset({ description: cash.description, active: cash.isActive });
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.saving = true;
    const payload = this.form.getRawValue() as CashFormValue;
    const request$ = this.editingCash
      ? this.cashesService.update(this.editingCash.cashId, payload)
      : this.cashesService.create(payload);

    request$.subscribe({
      next: () => {
        this.notificationService.success(this.editingCash ? 'Caja actualizada correctamente.' : 'Caja creada correctamente.');
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

  openAssignmentDialog(cash: CashViewModel): void {
    this.assignmentCash = cash;
    this.selectedCashierId = null;
    this.assignmentDialogVisible = true;
  }

  assignCashier(): void {
    if (!this.assignmentCash || !this.selectedCashierId) {
      return;
    }

    this.cashesService.assignCashier(this.assignmentCash.cashId, this.selectedCashierId).subscribe(() => {
      this.notificationService.success('Cajero asignado correctamente.');
      this.selectedCashierId = null;
      this.loadData(this.assignmentCash?.cashId ?? null);
    });
  }

  removeCashier(userId: number): void {
    if (!this.assignmentCash) {
      return;
    }

    this.cashesService.unassignCashier(this.assignmentCash.cashId, userId).subscribe(() => {
      this.notificationService.success('Asignación eliminada correctamente.');
      this.loadData(this.assignmentCash?.cashId ?? null);
    });
  }

  openSession(cash: CashViewModel): void {
    this.cashesService.openSession(cash.cashId).subscribe(() => {
      this.notificationService.success('Sesión abierta correctamente.');
      this.loadData();
    });
  }

  closeSession(cash: CashViewModel): void {
    this.cashesService.closeSession(cash.cashId).subscribe(() => {
      this.notificationService.success('Sesión cerrada correctamente.');
      this.loadData();
    });
  }

  confirmDelete(cash: CashViewModel): void {
    this.confirmationService.confirm({
      header: 'Eliminar caja',
      message: `¿Desea eliminar ${cash.name}?`,
      accept: () => {
        this.cashesService.delete(cash.cashId).subscribe(() => {
          this.notificationService.success('Caja eliminada correctamente.');
          this.loadData();
        });
      },
    });
  }

  private loadData(focusCashId?: number | null): void {
    this.loading = true;
    this.cashesService.getAll().subscribe({
      next: (cashes) => {
        this.cashes = cashes;
        if (focusCashId && this.assignmentDialogVisible) {
          this.assignmentCash = this.cashes.find((cash) => cash.cashId == focusCashId) ?? null;
        }
      },
      complete: () => {
        this.loading = false;
      },
    });
  }

  private canOperateCash(cash: CashViewModel): boolean {
    return this.canManageCashes || this.isAssignedToCurrentUser(cash);
  }

  private isAssignedToCurrentUser(cash: CashViewModel): boolean {
    if (!this.currentUserName) {
      return false;
    }

    return cash.assignedCashiers.some((cashier) => cashier.userName === this.currentUserName);
  }
}
