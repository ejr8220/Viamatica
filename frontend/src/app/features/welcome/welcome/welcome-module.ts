import { NgModule } from '@angular/core';

import { SharedModule } from '../../../shared/shared/shared-module';
import { WelcomePage } from '../pages/welcome-page/welcome-page';
import { WelcomeRoutingModule } from './welcome-routing-module';

@NgModule({
  declarations: [WelcomePage],
  imports: [SharedModule, WelcomeRoutingModule],
})
export class WelcomeModule {}
