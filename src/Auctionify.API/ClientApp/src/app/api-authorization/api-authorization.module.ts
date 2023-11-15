import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { ApplicationPaths } from '../api-authorization/api-authorization.constants';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { UiElementsModule } from '../ui-elements/ui-elements.module';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ReactiveFormsModule } from '@angular/forms';
import { DialogModule } from '@angular/cdk/dialog';
import { ResetPasswordComponent } from './reset-password/reset-password.component';
import { ForgotPasswordComponent } from './forgot-password/forgot-password.component';
import { isLoggedInGuard } from '../guards/is-logged-in.guard';
import { isBuyerGuard } from '../guards/buyer/is-buyer.guard';
import { isSellerGuard } from '../guards/seller/is-seller.guard';

@NgModule({
  declarations: [LoginComponent, RegisterComponent, ResetPasswordComponent, ForgetPasswordComponent],
  imports: [
    CommonModule,
    HttpClientModule,
    RouterModule.forChild([
      { path: ApplicationPaths.Login, component: LoginComponent },
      { path: ApplicationPaths.Register, component: RegisterComponent },
      { path: ApplicationPaths.ResetPassword, component: ResetPasswordComponent},
      { path: ApplicationPaths.ForgetPassword, component: ForgetPasswordComponent}
    ]),
    UiElementsModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    ReactiveFormsModule,
    DialogModule,
    MatButtonModule,
    MatProgressSpinnerModule
  ]
})
export class ApiAuthorizationModule {}
