import { RouterModule, Routes } from '@angular/router';
import { NgModule } from '@angular/core';
import { GetLotComponent } from './get-lot/get-lot.component';
import { FilterComponent } from './filter/filter.component';
import { AuctionComponent } from './auction/auction.component';

const BUYER_ROUTES: Routes = [
    {
        path: '',
        redirectTo: 'home',
        pathMatch: 'full',
    },
    {
        path: 'home',
        component: AuctionComponent,
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
