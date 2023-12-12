import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { ReactiveFormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { DialogModule } from '@angular/cdk/dialog';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialogModule } from '@angular/material/dialog';

import { UserProfileComponent } from './user-profile/user-profile.component';
import { ProfileNavbarComponent } from './profile-navbar/profile-navbar.component';
import { GeneralRoutingModule } from './general-routing.module';
import { UpdateUserProfileComponent } from './update-user-profile/update-user-profile.component';
import { ChangePasswordComponent } from './change-password/change-password.component';


@NgModule({
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    GeneralRoutingModule,
    ReactiveFormsModule,
    MatInputModule,
    MatDialogModule,
    MatProgressSpinnerModule,
    DialogModule,
  ],

  declarations: [
    UserProfileComponent,
    ProfileNavbarComponent,
    UpdateUserProfileComponent,
    ChangePasswordComponent,
  ],
  
  exports: [
    UserProfileComponent,
  ]
})
export class GeneralModule { }
