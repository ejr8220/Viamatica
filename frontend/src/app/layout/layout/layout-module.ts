import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { TreeModule } from 'primeng/tree';

import { MainLayout } from '../main-layout/main-layout';
import { Topbar } from '../topbar/topbar';
import { Sidebar } from '../sidebar/sidebar';
import { Footer } from '../footer/footer';

@NgModule({
  declarations: [
    MainLayout,
    Topbar,
    Sidebar,
    Footer
  ],
  imports: [
    CommonModule,
    RouterModule,
    ButtonModule,
    TreeModule
  ],
  exports: [
    MainLayout,
    Topbar,
    Sidebar,
    Footer
  ]
})
export class LayoutModule { }
