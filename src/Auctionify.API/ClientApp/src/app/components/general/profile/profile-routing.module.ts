import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { UpdateUserProfileComponent } from './update-user-profile/update-user-profile.component';
import { ChangePasswordComponent } from './change-password/change-password.component';
import { UserProfileComponent } from './user-profile/user-profile.component';
import { PublicUserProfileComponent } from './public-user-profile/public-user-profile.component';

const profileRoutes: Routes = [
    {
        path: '/id',
        component: PublicUserProfileComponent,
        pathMatch: 'full',
    },
    {
        path: '',
        component: UserProfileComponent,
        pathMatch: 'full',
    },
    {
        path: 'update-profile',
        component: UpdateUserProfileComponent,
        pathMatch: 'full',
    },
    {
        path: 'change-password',
        component: ChangePasswordComponent,
        pathMatch: 'full',
    },
];

@NgModule({
    imports: [RouterModule.forChild(profileRoutes)],
    exports: [RouterModule],
})
export class ProfileRoutingModule {}
