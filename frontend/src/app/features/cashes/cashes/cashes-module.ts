import { NgModule } from '@angular/core';

import { SharedModule } from '../../../shared/shared/shared-module';
import { CashesPage } from '../pages/cashes-page/cashes-page';
import { CashesRoutingModule } from './cashes-routing-module';

@NgModule({
  declarations: [CashesPage],
  imports: [SharedModule, CashesRoutingModule],
})
export class CashesModule {}
