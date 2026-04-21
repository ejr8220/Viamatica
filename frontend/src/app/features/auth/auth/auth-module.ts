import { NgModule } from '@angular/core';

import { SharedModule } from '../../../shared/shared/shared-module';
import { AuthRoutingModule } from './auth-routing-module';
import { ForgotPasswordPage } from '../pages/forgot-password-page/forgot-password-page';
import { LoginPage } from '../pages/login-page/login-page';

@NgModule({
  declarations: [LoginPage, ForgotPasswordPage],
  imports: [SharedModule, AuthRoutingModule],
})
export class AuthModule {}
