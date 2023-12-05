import { RouterModule, Routes } from '@angular/router';
import { NgModule } from '@angular/core';

const buyerRoutes: Routes = [
    {
        path: '',
        redirectTo: 'home',
        pathMatch: 'full',
    },
];

@NgModule({
    imports: [RouterModule.forChild(buyerRoutes)],
    exports: [RouterModule],
})
export class buyerRoutingModule {}
