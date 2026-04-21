import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { ConfirmationService } from 'primeng/api';
import { firstValueFrom } from 'rxjs';

import { BulkImportService, NotificationService } from '../../../../core/services';
import { GridAction, GridColumn } from '../../../../shared/components/grid-shared/grid-shared';
import { ClientFormValue, ClientsService, ClientViewModel } from '../../services/clients.service';

interface ImportResult {
  rowNumber: number;
  status: 'success' | 'error';
  message: string;
}

@Component({
  selector: 'app-clients-page',
  standalone: false,
  templateUrl: './clients-page.html',
  styleUrl: './clients-page.scss',
})
export class ClientsPage implements OnInit {
  clients: ClientViewModel[] = [];
  loading = false;
  dialogVisible = false;
  importDialogVisible = false;
  saving = false;
  importing = false;
  bulkProgress = 0;
  bulkResults: ImportResult[] = [];
  searchTerm = '';
  editingClient: ClientViewModel | null = null;
  private readonly fb = inject(FormBuilder);

  readonly columns: GridColumn[] = [
    { field: 'identification', header: 'Identificación', sortable: true, filterable: true },
    { field: 'displayName', header: 'Cliente', sortable: true, filterable: true },
    { field: 'email', header: 'Correo', sortable: true, filterable: true },
    { field: 'phone', header: 'Teléfono', sortable: true, filterable: true },
    { field: 'status', header: 'Estado', sortable: true },
  ];

  readonly actions: GridAction[] = [
    {
      icon: 'pi pi-pencil',
      tooltip: 'Editar',
      callback: (row) => this.openEditDialog(row as ClientViewModel),
      styleClass: 'p-button-rounded p-button-text',
    },
    {
      icon: 'pi pi-trash',
      tooltip: 'Eliminar',
      callback: (row) => this.confirmDelete(row as ClientViewModel),
      styleClass: 'p-button-rounded p-button-text p-button-danger',
    },
  ];

  readonly form = this.fb.nonNullable.group({
    identification: ['', [Validators.required, Validators.minLength(10)]],
    firstName: ['', [Validators.required]],
    lastName: ['', [Validators.required]],
    email: ['', [Validators.required, Validators.email]],
    phone: ['', [Validators.required, Validators.minLength(10)]],
    address: ['', [Validators.required, Validators.minLength(20)]],
    referenceAddress: ['', [Validators.required, Validators.minLength(20)]],
  });

  constructor(
    private readonly clientsService: ClientsService,
    private readonly notificationService: NotificationService,
    private readonly confirmationService: ConfirmationService,
    private readonly bulkImportService: BulkImportService
  ) {}

  ngOnInit(): void {
    this.loadClients();
  }

  get filteredClients(): ClientViewModel[] {
    const term = this.searchTerm.trim().toLowerCase();
    if (!term) {
      return this.clients;
    }

    return this.clients.filter((client) =>
      [client.displayName, client.email, client.identification, client.phone].some((value) =>
        value.toLowerCase().includes(term)
      )
    );
  }

  openCreateDialog(): void {
    this.editingClient = null;
    this.dialogVisible = true;
    this.form.reset({
      identification: '',
      firstName: '',
      lastName: '',
      email: '',
      phone: '',
      address: '',
      referenceAddress: '',
    });
  }

  openEditDialog(client: ClientViewModel): void {
    this.editingClient = client;
    this.dialogVisible = true;
    this.form.reset({
      identification: client.identification,
      firstName: client.firstName,
      lastName: client.lastName,
      email: client.email,
      phone: client.phone,
      address: client.address,
      referenceAddress: client.referenceAddress,
    });
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.notificationService.warning('Revise el formulario. La dirección y la referencia deben tener al menos 20 caracteres.');
      return;
    }

    this.saving = true;
    const payload = this.form.getRawValue() as ClientFormValue;
    const request$ = this.editingClient
      ? this.clientsService.update(this.editingClient.clientId, payload)
      : this.clientsService.create(payload);

    request$.subscribe({
      next: () => {
        this.notificationService.success(
          this.editingClient ? 'Cliente actualizado correctamente.' : 'Cliente creado correctamente.'
        );
        this.dialogVisible = false;
        this.loadClients();
      },
      error: () => {
        this.saving = false;
      },
      complete: () => {
        this.saving = false;
      },
    });
  }

  confirmDelete(client: ClientViewModel): void {
    this.confirmationService.confirm({
      header: 'Eliminar cliente',
      message: `¿Desea eliminar a ${client.displayName}?`,
      accept: () => {
        this.clientsService.delete(client.clientId).subscribe(() => {
          this.notificationService.success('Cliente eliminado correctamente.');
          this.loadClients();
        });
      },
    });
  }

  async onFileSelected(event: Event): Promise<void> {
    const input = event.target as HTMLInputElement;
    const file = input.files?.item(0);
    if (!file) {
      return;
    }

    this.importDialogVisible = true;
    this.bulkResults = [];
    this.bulkProgress = 0;

    try {
      const parsed = await this.bulkImportService.parseFile(file);
      const requiredHeaders = ['identification', 'firstname', 'lastname', 'email', 'phone', 'address'];
      const hasValidStructure = requiredHeaders.every((header) => parsed.headers.includes(header));

      if (!hasValidStructure) {
        throw new Error(
          'La estructura del archivo no es válida. Encabezados requeridos: identification, firstName, lastName, email, phone, address.'
        );
      }

      await this.processImport(parsed.rows);
    } catch (error) {
      this.importing = false;
      this.notificationService.error(error instanceof Error ? error.message : 'No se pudo procesar el archivo.');
    } finally {
      input.value = '';
    }
  }

  private async processImport(rows: Record<string, string>[]): Promise<void> {
    this.importing = true;

    for (let index = 0; index < rows.length; index += 1) {
      const row = rows[index];
      try {
        const payload = this.mapImportRow(row);
        await firstValueFrom(this.clientsService.create(payload));
        this.bulkResults = [...this.bulkResults, { rowNumber: index + 1, status: 'success', message: 'Registro importado.' }];
      } catch (error) {
        this.bulkResults = [
          ...this.bulkResults,
          {
            rowNumber: index + 1,
            status: 'error',
            message: error instanceof Error ? error.message : this.extractError(error),
          },
        ];
      }
      this.bulkProgress = Math.round(((index + 1) / rows.length) * 100);
    }

    this.importing = false;
    this.notificationService.info('Importación masiva finalizada.');
    this.loadClients();
  }

  private loadClients(): void {
    this.loading = true;
    this.clientsService.getAll().subscribe({
      next: (clients) => {
        this.clients = clients;
      },
      complete: () => {
        this.loading = false;
      },
    });
  }

  private mapImportRow(row: Record<string, string>): ClientFormValue {
    const payload: ClientFormValue = {
      identification: row['identification']?.trim() ?? '',
      firstName: row['firstname']?.trim() ?? '',
      lastName: row['lastname']?.trim() ?? '',
      email: row['email']?.trim() ?? '',
      phone: row['phone']?.trim() ?? '',
      address: row['address']?.trim() ?? '',
      referenceAddress: row['referenceaddress']?.trim() || row['address']?.trim() || '',
    };

    if (Object.values(payload).some((value) => !value)) {
      throw new Error('Todos los campos son obligatorios para la importación.');
    }

    if (payload.address.length < 20 || payload.referenceAddress.length < 20) {
      throw new Error('La dirección y referencia deben tener al menos 20 caracteres.');
    }

    return payload;
  }

  private extractError(error: unknown): string {
    if (typeof error === 'object' && error !== null && 'error' in error) {
      const apiError = (error as { error?: { message?: string } }).error;
      return apiError?.message ?? 'No se pudo importar la fila.';
    }

    return 'No se pudo importar la fila.';
  }

  hasError(controlName: keyof ClientFormValue, errorCode?: string): boolean {
    const control = this.form.controls[controlName];
    if (!control) {
      return false;
    }

    if (!control.touched) {
      return false;
    }

    return errorCode ? control.hasError(errorCode) : control.invalid;
  }
}
