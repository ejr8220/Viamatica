import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { CheckboxModule } from 'primeng/checkbox';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { ProgressBarModule } from 'primeng/progressbar';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { TableModule } from 'primeng/table';
import { ToastModule } from 'primeng/toast';
import { TooltipModule } from 'primeng/tooltip';

import { GridShared } from '../components/grid-shared/grid-shared';
import { InputSearchShared } from '../components/input-search-shared/input-search-shared';
import { LoadingOverlay } from '../components/loading-overlay/loading-overlay';
import { PageHeader } from '../components/page-header/page-header';
import { StatCard } from '../components/stat-card/stat-card';
import { StatsSection } from '../components/stats-section/stats-section';

const COMPONENTS = [GridShared, InputSearchShared, PageHeader, StatCard, StatsSection, LoadingOverlay];
const PRIMENG_MODULES = [
  ButtonModule,
  CardModule,
  CheckboxModule,
  ConfirmDialogModule,
  DialogModule,
  InputTextModule,
  ProgressBarModule,
  ProgressSpinnerModule,
  TableModule,
  ToastModule,
  TooltipModule,
];

@NgModule({
  declarations: COMPONENTS,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, ...PRIMENG_MODULES],
  exports: [
    ...COMPONENTS,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    ...PRIMENG_MODULES,
  ],
})
export class SharedModule {}
