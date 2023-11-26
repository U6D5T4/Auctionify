import { RouterModule, Routes } from '@angular/router';
import { NgModule } from '@angular/core';
import { LotProfileBuyerComponent } from './lot-profile-buyer/lot-profile-buyer.component';

const buyerRoutes: Routes = [
    {
        path: ':id/buyer',
        component: LotProfileBuyerComponent,
        pathMatch: 'full',
    }
];

@NgModule({
    imports: [RouterModule.forChild(buyerRoutes)],
    exports: [RouterModule],
})
export class buyerRoutingModule {}
