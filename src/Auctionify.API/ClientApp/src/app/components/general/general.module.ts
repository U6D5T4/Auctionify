import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserProfileComponent } from './user-profile/user-profile.component';
import { ProfileNavbarComponent } from './profile-navbar/profile-navbar.component';



@NgModule({
  declarations: [
    UserProfileComponent,
    ProfileNavbarComponent
  ],
  imports: [
    CommonModule
  ]
})
export class GeneralModule { }
