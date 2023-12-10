import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

import { UserProfileComponent } from './user-profile/user-profile.component';
import { ProfileNavbarComponent } from './profile-navbar/profile-navbar.component';
import { GeneralRoutingModule } from './general-routing.module';

@NgModule({
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    GeneralRoutingModule,
  ],

  declarations: [
    UserProfileComponent,
    ProfileNavbarComponent
  ],
  
  exports: [
    UserProfileComponent,
  ]
})
export class GeneralModule { }
