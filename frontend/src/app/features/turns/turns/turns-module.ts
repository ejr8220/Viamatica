import { NgModule } from '@angular/core';

import { SharedModule } from '../../../shared/shared/shared-module';
import { TurnsPage } from '../pages/turns-page/turns-page';
import { TurnsRoutingModule } from './turns-routing-module';

@NgModule({
  declarations: [TurnsPage],
  imports: [SharedModule, TurnsRoutingModule],
})
export class TurnsModule {}
