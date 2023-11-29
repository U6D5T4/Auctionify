import { RouterModule, Routes } from '@angular/router';
import { NgModule } from '@angular/core';
import { GetLotComponent } from './get-lot/get-lot.component';
import { AddBidComponent } from './add-bid/add-bid.component';

const BUYER_ROUTES: Routes = [
    {
        path: '',
        redirectTo: 'home',
        pathMatch: 'full',
    },
    {
        path: 'buyer/get-lot/:id',
        component: AddBidComponent,
    },
    {
        path: 'get-lot/:id',
        component: GetLotComponent,
        pathMatch: 'full',
    },
];

@NgModule({
    imports: [RouterModule.forChild(BUYER_ROUTES)],
    exports: [RouterModule],
})
export class BuyerRoutingModule {}
