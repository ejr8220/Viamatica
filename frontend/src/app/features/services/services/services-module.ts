import { NgModule } from '@angular/core';

import { SharedModule } from '../../../shared/shared/shared-module';
import { ServicesPage } from '../pages/services-page/services-page';
import { ServicesRoutingModule } from './services-routing-module';

@NgModule({
  declarations: [ServicesPage],
  imports: [SharedModule, ServicesRoutingModule],
})
export class ServicesModule {}
