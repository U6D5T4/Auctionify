import { RouterModule, Routes } from '@angular/router';
import { NgModule } from '@angular/core';

const BUYER_ROUTES: Routes = [
    {
        path: '',
        redirectTo: 'home',
        pathMatch: 'full',
    },
];

@NgModule({
    imports: [RouterModule.forChild(BUYER_ROUTES)],
    exports: [RouterModule],
})
export class BuyerRoutingModule {}
