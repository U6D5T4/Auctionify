import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const generalRoutes: Routes = [
  {
    path: '',
    loadChildren: () =>
      import('./profile/profile.module').then(
        (m) => m.ProfileModule
      ),
    data: { breadcrumb: { skip: true } }
  },
];

@NgModule({
  imports: [RouterModule.forChild(generalRoutes)],
  exports: [RouterModule],
})
export class GeneralRoutingModule { }
