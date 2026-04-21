import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { TurnsPage } from '../pages/turns-page/turns-page';

const routes: Routes = [{ path: '', component: TurnsPage }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class TurnsRoutingModule {}
