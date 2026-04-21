import { NgModule } from '@angular/core';

import { SharedModule } from '../../../shared/shared/shared-module';
import { AttentionsPage } from '../pages/attentions-page/attentions-page';
import { AttentionsRoutingModule } from './attentions-routing-module';

@NgModule({
  declarations: [AttentionsPage],
  imports: [SharedModule, AttentionsRoutingModule],
})
export class AttentionsModule {}
