import { NgModule } from '@angular/core';

import { SharedModule } from '../../../shared/shared/shared-module';
import { UsersPage } from '../pages/users-page/users-page';
import { UsersRoutingModule } from './users-routing-module';

@NgModule({
  declarations: [UsersPage],
  imports: [SharedModule, UsersRoutingModule],
})
export class UsersModule {}
