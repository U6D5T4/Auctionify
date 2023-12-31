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
import { ForgotPasswordComponent } from './forgot-password/forgot-password.component';
import { isLoggedInGuard } from '../guards/is-logged-in.guard';

@NgModule({
  declarations: [LoginComponent, RegisterComponent, ForgotPasswordComponent],
  imports: [
    CommonModule,
    HttpClientModule,
    RouterModule.forChild([
      {
        path: ApplicationPaths.Login,
        component: LoginComponent,
        canActivate: [isLoggedInGuard],
      },
      {
        path: ApplicationPaths.Register,
        component: RegisterComponent,
        canActivate: [isLoggedInGuard],
      },
      {
        path: ApplicationPaths.ForgotPassword,
        component: ForgotPasswordComponent,
      },
    ]),
    UiElementsModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    ReactiveFormsModule,
    DialogModule,
    MatButtonModule,
    MatProgressSpinnerModule,
  ],
})
export class ApiAuthorizationModule {}
