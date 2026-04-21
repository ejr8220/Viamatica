import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { AttentionsPage } from '../pages/attentions-page/attentions-page';

const routes: Routes = [{ path: '', component: AttentionsPage }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AttentionsRoutingModule {}
