import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { isLoggedInGuard } from 'src/app/guards/is-logged-in.guard';

const generalRoutes: Routes = [
  {
    path: '',
    loadChildren: () =>
      import('./profile/profile.module').then(
        (m) => m.ProfileModule
      ),
    data: { breadcrumb: { skip: true } },
    canActivate: [isLoggedInGuard],
  },
];

@NgModule({
  imports: [RouterModule.forChild(generalRoutes)],
  exports: [RouterModule],
})
export class GeneralRoutingModule { }
