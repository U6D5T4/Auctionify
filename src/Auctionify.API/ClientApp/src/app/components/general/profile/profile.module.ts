import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { ReactiveFormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { DialogModule } from '@angular/cdk/dialog';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { UserProfileComponent } from './user-profile/user-profile.component';
import { ProfileNavbarComponent } from './profile-navbar/profile-navbar.component';
import { UpdateUserProfileComponent } from './update-user-profile/update-user-profile.component';
import { ChangePasswordComponent } from './change-password/change-password.component';
import { ProfileRoutingModule } from './profile-routing.module';
import { UiElementsModule } from 'src/app/ui-elements/ui-elements.module';

@NgModule({
    imports: [
        CommonModule,
        MatButtonModule,
        MatIconModule,
        MatFormFieldModule,
        ReactiveFormsModule,
        MatInputModule,
        MatProgressSpinnerModule,
        DialogModule,
        ProfileRoutingModule,
        UiElementsModule,
    ],

    declarations: [
        UserProfileComponent,
        ProfileNavbarComponent,
        UpdateUserProfileComponent,
        ChangePasswordComponent,
    ],

    exports: [
        UserProfileComponent,
        UpdateUserProfileComponent,
        ChangePasswordComponent,
        ProfileNavbarComponent,
    ],
})
export class ProfileModule {}
