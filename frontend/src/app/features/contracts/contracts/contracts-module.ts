import { NgModule } from '@angular/core';

import { SharedModule } from '../../../shared/shared/shared-module';
import { ContractsPage } from '../pages/contracts-page/contracts-page';
import { ContractsRoutingModule } from './contracts-routing-module';

@NgModule({
  declarations: [ContractsPage],
  imports: [SharedModule, ContractsRoutingModule],
})
export class ContractsModule {}
