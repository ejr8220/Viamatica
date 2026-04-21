import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { FormBuilder, Validators } from '@angular/forms';
import { ConfirmationService } from 'primeng/api';
import { firstValueFrom } from 'rxjs';

import { BulkImportService, NotificationService } from '../../../../core/services';
import { GridAction, GridColumn } from '../../../../shared/components/grid-shared/grid-shared';
import { ActiveUserReportViewModel, UserFormValue, UsersService, UserViewModel } from '../../services/users.service';

interface ImportResult {
  rowNumber: number;
  status: 'success' | 'error';
  message: string;
}

@Component({
  selector: 'app-users-page',
  standalone: false,
  templateUrl: './users-page.html',
  styleUrl: './users-page.scss',
})
export class UsersPage implements OnInit {
  users: UserViewModel[] = [];
  activeReport: ActiveUserReportViewModel[] = [];
  loading = false;
  dialogVisible = false;
  importDialogVisible = false;
  saving = false;
  importing = false;
  searchTerm = '';
  bulkProgress = 0;
  bulkResults: ImportResult[] = [];
  editingUser: UserViewModel | null = null;
  activeView = false;
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);

  readonly columns: GridColumn[] = [
    { field: 'userName', header: 'Usuario', sortable: true, filterable: true },
    { field: 'identification', header: 'Identificación', sortable: true, filterable: true },
    { field: 'email', header: 'Correo', sortable: true, filterable: true },
    { field: 'role', header: 'Rol', sortable: true, filterable: true },
    { field: 'status', header: 'Estado', sortable: true, filterable: true },
    { field: 'createdAt', header: 'Fecha aprobación', sortable: true, pipe: 'date' },
  ];

  readonly activeReportColumns: GridColumn[] = [
    { field: 'userName', header: 'Usuario', sortable: true, filterable: true },
    { field: 'identification', header: 'Identificación', sortable: true, filterable: true },
    { field: 'email', header: 'Correo', sortable: true, filterable: true },
    { field: 'role', header: 'Rol', sortable: true, filterable: true },
    { field: 'status', header: 'Estado', sortable: true, filterable: true },
    { field: 'activeSessionCount', header: 'Sesiones activas', sortable: true, filterable: true },
  ];

  readonly actions: GridAction[] = [
    {
      icon: 'pi pi-pencil',
      tooltip: 'Editar',
      callback: (row) => this.openEditDialog(row as UserViewModel),
      styleClass: 'p-button-rounded p-button-text',
    },
    {
      icon: 'pi pi-check',
      tooltip: 'Aprobar',
      callback: (row) => this.approve(row as UserViewModel),
      visible: (row) => !(row as UserViewModel).approved,
      styleClass: 'p-button-rounded p-button-text p-button-success',
    },
    {
      icon: 'pi pi-trash',
      tooltip: 'Eliminar',
      callback: (row) => this.confirmDelete(row as UserViewModel),
      styleClass: 'p-button-rounded p-button-text p-button-danger',
    },
  ];

  readonly roleOptions = ['Administrador', 'Gestor', 'Cajero'];
  readonly statusOptions = ['Activo', 'Inactivo'];
  readonly form = this.fb.nonNullable.group({
    userName: ['', [Validators.required, Validators.minLength(8)]],
    identification: ['', [Validators.required, Validators.pattern(/^\d{10,13}$/)]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.minLength(8)]],
    role: ['Gestor', [Validators.required]],
    status: ['Activo', [Validators.required]],
  });

  constructor(
    private readonly usersService: UsersService,
    private readonly notificationService: NotificationService,
    private readonly confirmationService: ConfirmationService,
    private readonly bulkImportService: BulkImportService
  ) {}

  ngOnInit(): void {
    this.route.queryParamMap.subscribe((params) => {
      this.activeView = params.get('view') === 'active-report';
    });
    this.loadData();
  }

  get filteredUsers(): UserViewModel[] {
    const term = this.searchTerm.trim().toLowerCase();
    if (!term) {
      return this.users;
    }

    return this.users.filter((user) =>
      [user.userName, user.identification, user.email, user.role, user.status].some((value) => value.toLowerCase().includes(term))
    );
  }

  get filteredActiveReport(): ActiveUserReportViewModel[] {
    const term = this.searchTerm.trim().toLowerCase();
    if (!term) {
      return this.activeReport;
    }

    return this.activeReport.filter((user) =>
      [user.userName, user.identification, user.email, user.role, user.status, String(user.activeSessionCount)].some((value) =>
        String(value ?? '').toLowerCase().includes(term)
      )
    );
  }

  get pendingApprovals(): number {
    return this.users.filter((user) => !user.approved).length;
  }

  get pageTitle(): string {
    return this.activeView ? 'Reporte usuarios activos' : 'Usuarios';
  }

  get pageSubtitle(): string {
    return this.activeView
      ? 'Consulta de usuarios activos obtenidos desde el reporte del backend.'
      : 'Administra accesos, aprobación y carga masiva.';
  }

  get displayedColumns(): GridColumn[] {
    return this.activeView ? this.activeReportColumns : this.columns;
  }

  get displayedData(): Array<UserViewModel | ActiveUserReportViewModel> {
    return this.activeView ? this.filteredActiveReport : this.filteredUsers;
  }

  openCreateDialog(): void {
    this.editingUser = null;
    this.dialogVisible = true;
    this.form.reset({
      userName: '',
      identification: '',
      email: '',
      password: '',
      role: 'Gestor',
      status: 'Activo',
    });
  }

  openEditDialog(user: UserViewModel): void {
    this.editingUser = user;
    this.dialogVisible = true;
    this.form.reset({
      userName: user.userName,
      identification: user.identification,
      email: user.email,
      password: '',
      role: user.role,
      status: user.active ? 'Activo' : 'Inactivo',
    });
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    if (!this.editingUser && !this.form.controls.password.value.trim()) {
      this.notificationService.warning('La contraseña es obligatoria para crear un usuario.');
      return;
    }

    if (this.form.controls.userName.invalid) {
      this.form.controls.userName.markAsTouched();
      this.notificationService.warning('El usuario debe tener al menos 8 caracteres.');
      return;
    }

    if (this.form.controls.identification.invalid) {
      this.form.controls.identification.markAsTouched();
      this.notificationService.warning('La identificación debe tener entre 10 y 13 dígitos.');
      return;
    }

    if (this.form.controls.password.invalid) {
      this.form.controls.password.markAsTouched();
      this.notificationService.warning('La contraseña debe tener al menos 8 caracteres.');
      return;
    }

    this.saving = true;
    const payload = this.form.getRawValue() as UserFormValue;
    const request$ = this.editingUser
      ? this.usersService.update(this.editingUser.userId, payload)
      : this.usersService.create(payload);

    request$.subscribe({
      next: () => {
        this.notificationService.success(
          this.editingUser ? 'Usuario actualizado correctamente.' : 'Usuario creado correctamente.'
        );
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

  approve(user: UserViewModel): void {
    this.confirmationService.confirm({
      header: 'Aprobar usuario',
      message: `¿Desea aprobar a ${user.userName}?`,
      accept: () => {
        this.usersService.approve(user.userId).subscribe(() => {
          this.notificationService.success('Usuario aprobado correctamente.');
          this.loadData();
        });
      },
    });
  }

  confirmDelete(user: UserViewModel): void {
    this.confirmationService.confirm({
      header: 'Eliminar usuario',
      message: `¿Desea eliminar a ${user.userName}?`,
      accept: () => {
        this.usersService.delete(user.userId).subscribe(() => {
          this.notificationService.success('Usuario eliminado correctamente.');
          this.loadData();
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
      const requiredHeaders = ['username', 'identification', 'email', 'password', 'role'];
      const hasValidStructure = requiredHeaders.every((header) => parsed.headers.includes(header));

      if (!hasValidStructure) {
        throw new Error('La estructura del archivo no es válida. Encabezados requeridos: userName, identification, email, password, role.');
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
        await firstValueFrom(this.usersService.create(payload));
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
    this.loadData();
  }

  private loadData(): void {
    this.loading = true;
    this.usersService.getAll().subscribe({
      next: (users) => {
        this.users = users;
      },
      complete: () => {
        this.loading = false;
      },
    });

    this.usersService.getActiveReport().subscribe((report) => {
      this.activeReport = report;
    });
  }

  private mapImportRow(row: Record<string, string>): UserFormValue {
    const userName = row['username']?.trim() ?? '';
    const identification = row['identification']?.trim() ?? '';
    const email = row['email']?.trim() ?? '';
    const password = row['password']?.trim() ?? '';
    const role = this.normalizeRole(row['role']?.trim() ?? '');

    if (!userName || !identification || !email || !password || !role) {
      throw new Error('Fila inválida. Revise userName, identification, email, password y role.');
    }

    if (userName.length < 8) {
        throw new Error('El usuario debe tener al menos 8 caracteres.');
    }

    if (!/^\d{10,13}$/.test(identification)) {
      throw new Error('La identificación debe tener entre 10 y 13 dígitos.');
    }

    if (password.length < 8) {
      throw new Error('La contraseña debe tener al menos 8 caracteres.');
    }

    return {
      userName,
      identification,
      email,
      password,
      role,
      status: 'Activo',
    };
  }

  private normalizeRole(value: string): string {
    const normalized = value.toLowerCase();
    if (normalized === 'administrador' || normalized === 'administrator' || normalized === 'admin') {
      return 'Administrador';
    }
    if (normalized === 'cashier' || normalized === 'cajero') {
      return 'Cajero';
    }
    if (normalized === 'gestor' || normalized === 'manager') {
      return 'Gestor';
    }
    throw new Error(`Rol no soportado: ${value}`);
  }

  private extractError(error: unknown): string {
    if (typeof error === 'object' && error !== null && 'error' in error) {
      const apiError = (error as { error?: { message?: string } }).error;
      return apiError?.message ?? 'No se pudo importar la fila.';
    }

    return 'No se pudo importar la fila.';
  }
}
