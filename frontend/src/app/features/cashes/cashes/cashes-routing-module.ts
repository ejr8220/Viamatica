import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { CashesPage } from '../pages/cashes-page/cashes-page';

const routes: Routes = [{ path: '', component: CashesPage }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class CashesRoutingModule {}
