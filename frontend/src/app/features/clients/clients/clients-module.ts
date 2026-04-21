import { NgModule } from '@angular/core';

import { SharedModule } from '../../../shared/shared/shared-module';
import { ClientsPage } from '../pages/clients-page/clients-page';
import { ClientsRoutingModule } from './clients-routing-module';

@NgModule({
  declarations: [ClientsPage],
  imports: [SharedModule, ClientsRoutingModule],
})
export class ClientsModule {}
