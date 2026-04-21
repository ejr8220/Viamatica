import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { ConfirmationService } from 'primeng/api';

import { NotificationService } from '../../../../core/services';
import { GridAction, GridColumn } from '../../../../shared/components/grid-shared/grid-shared';
import { ServiceCatalogService, ServiceFormValue, ServiceViewModel } from '../../services/service-catalog.service';

@Component({
  selector: 'app-services-page',
  standalone: false,
  templateUrl: './services-page.html',
  styleUrl: './services-page.scss',
})
export class ServicesPage implements OnInit {
  services: ServiceViewModel[] = [];
  loading = false;
  dialogVisible = false;
  saving = false;
  searchTerm = '';
  editingService: ServiceViewModel | null = null;
  private readonly fb = inject(FormBuilder);

  readonly columns: GridColumn[] = [
    { field: 'name', header: 'Servicio', sortable: true, filterable: true },
    { field: 'description', header: 'Descripción', sortable: true, filterable: true },
    { field: 'price', header: 'Precio', sortable: true },
    { field: 'code', header: 'Código', sortable: true },
  ];

  readonly actions: GridAction[] = [
    {
      icon: 'pi pi-pencil',
      tooltip: 'Editar',
      callback: (row) => this.openEditDialog(row as ServiceViewModel),
      styleClass: 'p-button-rounded p-button-text',
    },
    {
      icon: 'pi pi-trash',
      tooltip: 'Eliminar',
      callback: (row) => this.confirmDelete(row as ServiceViewModel),
      styleClass: 'p-button-rounded p-button-text p-button-danger',
    },
  ];

  readonly form = this.fb.nonNullable.group({
    name: ['', [Validators.required]],
    description: ['', [Validators.required, Validators.minLength(5)]],
    price: [0, [Validators.required, Validators.min(0.01)]],
  });

  constructor(
    private readonly serviceCatalogService: ServiceCatalogService,
    private readonly notificationService: NotificationService,
    private readonly confirmationService: ConfirmationService
  ) {}

  ngOnInit(): void {
    this.loadServices();
  }

  get filteredServices(): ServiceViewModel[] {
    const term = this.searchTerm.trim().toLowerCase();
    if (!term) {
      return this.services;
    }

    return this.services.filter((service) =>
      [service.name, service.description, service.code].some((value) => value.toLowerCase().includes(term))
    );
  }

  get averagePrice(): string {
    const total = this.services.reduce((sum, item) => sum + item.price, 0);
    return (total / (this.services.length || 1)).toFixed(2);
  }

  openCreateDialog(): void {
    this.editingService = null;
    this.dialogVisible = true;
    this.form.reset({ name: '', description: '', price: 0 });
  }

  openEditDialog(service: ServiceViewModel): void {
    this.editingService = service;
    this.dialogVisible = true;
    this.form.reset({ name: service.name, description: service.description, price: service.price });
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.saving = true;
    const payload = this.form.getRawValue() as ServiceFormValue;
    const request$ = this.editingService
      ? this.serviceCatalogService.update(this.editingService.serviceId, payload)
      : this.serviceCatalogService.create(payload);

    request$.subscribe({
      next: () => {
        this.notificationService.success(this.editingService ? 'Servicio actualizado.' : 'Servicio creado.');
        this.dialogVisible = false;
        this.loadServices();
      },
      error: () => {
        this.saving = false;
      },
      complete: () => {
        this.saving = false;
      },
    });
  }

  confirmDelete(service: ServiceViewModel): void {
    this.confirmationService.confirm({
      header: 'Eliminar servicio',
      message: `¿Desea eliminar ${service.name}?`,
      accept: () => {
        this.serviceCatalogService.delete(service.serviceId).subscribe(() => {
          this.notificationService.success('Servicio eliminado correctamente.');
          this.loadServices();
        });
      },
    });
  }

  private loadServices(): void {
    this.loading = true;
    this.serviceCatalogService.getAll().subscribe({
      next: (services) => {
        this.services = services;
      },
      complete: () => {
        this.loading = false;
      },
    });
  }
}
