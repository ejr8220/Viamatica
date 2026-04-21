import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { ConfirmationService } from 'primeng/api';

import { NotificationService } from '../../../../core/services';
import { GridAction, GridColumn } from '../../../../shared/components/grid-shared/grid-shared';
import {
  ContractClientOption,
  ContractFormValue,
  ContractServiceOption,
  ContractsService,
  ContractViewModel,
  PaymentMethodOption,
} from '../../services/contracts.service';

@Component({
  selector: 'app-contracts-page',
  standalone: false,
  templateUrl: './contracts-page.html',
  styleUrl: './contracts-page.scss',
})
export class ContractsPage implements OnInit {
  contracts: ContractViewModel[] = [];
  clientOptions: ContractClientOption[] = [];
  serviceOptions: ContractServiceOption[] = [];
  paymentMethods: PaymentMethodOption[] = [];
  selectedClient: ContractClientOption | null = null;
  selectedService: ContractServiceOption | null = null;
  loading = false;
  dialogVisible = false;
  saving = false;
  searchTerm = '';
  editingContract: ContractViewModel | null = null;
  private readonly fb = inject(FormBuilder);

  readonly columns: GridColumn[] = [
    { field: 'contractNumber', header: 'Contrato', sortable: true, filterable: true },
    { field: 'clientName', header: 'Cliente', sortable: true, filterable: true },
    { field: 'serviceName', header: 'Servicio', sortable: true, filterable: true },
    { field: 'methodPaymentDescription', header: 'Pago', sortable: true },
    { field: 'status', header: 'Estado', sortable: true, filterable: true },
    { field: 'startDate', header: 'Inicio', sortable: true, pipe: 'date' },
  ];

  readonly actions: GridAction[] = [
    {
      icon: 'pi pi-pencil',
      tooltip: 'Editar',
      callback: (row) => this.openEditDialog(row as ContractViewModel),
      styleClass: 'p-button-rounded p-button-text',
    },
    {
      icon: 'pi pi-trash',
      tooltip: 'Cancelar contrato',
      callback: (row) => this.confirmCancel(row as ContractViewModel),
      styleClass: 'p-button-rounded p-button-text p-button-danger',
    },
  ];

  readonly form = this.fb.nonNullable.group({
    clientId: [0, [Validators.required, Validators.min(1)]],
    serviceId: [0, [Validators.required, Validators.min(1)]],
    methodPaymentId: [0, [Validators.required, Validators.min(1)]],
    startDate: ['', [Validators.required]],
    endDate: ['', [Validators.required]],
  });

  constructor(
    private readonly contractsService: ContractsService,
    private readonly notificationService: NotificationService,
    private readonly confirmationService: ConfirmationService
  ) {}

  ngOnInit(): void {
    this.loadData();
    this.contractsService.getClientOptions().subscribe((items) => {
      this.clientOptions = items;
    });
    this.contractsService.getServiceOptions().subscribe((items) => {
      this.serviceOptions = items;
    });
    this.contractsService.getPaymentMethods().subscribe((items) => {
      this.paymentMethods = items;
    });
  }

  get filteredContracts(): ContractViewModel[] {
    const term = this.searchTerm.trim().toLowerCase();
    if (!term) {
      return this.contracts;
    }

    return this.contracts.filter((contract) =>
      [contract.contractNumber, contract.clientName, contract.serviceName, contract.status].some((value) =>
        value.toLowerCase().includes(term)
      )
    );
  }

  get paymentCountTotal(): number {
    return this.contracts.reduce((sum, item) => sum + item.paymentCount, 0);
  }

  get totalPaidAmount(): string {
    return this.contracts.reduce((sum, item) => sum + item.totalPaid, 0).toFixed(2);
  }

  openCreateDialog(): void {
    this.editingContract = null;
    this.selectedClient = null;
    this.selectedService = null;
    this.dialogVisible = true;
    this.form.reset({
      clientId: 0,
      serviceId: 0,
      methodPaymentId: 0,
      startDate: new Date().toISOString().slice(0, 10),
      endDate: new Date(new Date().setMonth(new Date().getMonth() + 12)).toISOString().slice(0, 10),
    });
  }

  openEditDialog(contract: ContractViewModel): void {
    this.editingContract = contract;
    this.selectedClient = this.clientOptions.find((item) => item.clientId === contract.clientId) ?? null;
    this.selectedService = this.serviceOptions.find((item) => item.serviceId === contract.serviceId) ?? null;
    this.dialogVisible = true;
    this.form.reset({
      clientId: contract.clientId,
      serviceId: contract.serviceId,
      methodPaymentId: contract.methodPaymentId,
      startDate: contract.startDate.slice(0, 10),
      endDate: contract.endDate.slice(0, 10),
    });
  }

  onClientSelected(item: ContractClientOption | null): void {
    this.selectedClient = item;
    this.form.controls.clientId.setValue(item?.clientId ?? 0);
  }

  onServiceSelected(item: ContractServiceOption | null): void {
    this.selectedService = item;
    this.form.controls.serviceId.setValue(item?.serviceId ?? 0);
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.saving = true;
    const payload = this.form.getRawValue() as ContractFormValue;
    const request$ = this.editingContract
      ? this.contractsService.update(this.editingContract.contractId, payload)
      : this.contractsService.create(payload);

    request$.subscribe({
      next: () => {
        this.notificationService.success(this.editingContract ? 'Contrato actualizado.' : 'Contrato creado.');
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

  confirmCancel(contract: ContractViewModel): void {
    this.confirmationService.confirm({
      header: 'Cancelar contrato',
      message: `¿Desea cancelar ${contract.contractNumber}?`,
      accept: () => {
        this.contractsService.cancel(contract.contractId).subscribe(() => {
          this.notificationService.success('Contrato cancelado correctamente.');
          this.loadData();
        });
      },
    });
  }

  private loadData(): void {
    this.loading = true;
    this.contractsService.getAll().subscribe({
      next: (contracts) => {
        this.contracts = contracts;
      },
      complete: () => {
        this.loading = false;
      },
    });
  }
}
