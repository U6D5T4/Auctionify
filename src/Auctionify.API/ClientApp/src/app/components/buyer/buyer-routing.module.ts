import { RouterModule, Routes } from '@angular/router';
import { NgModule } from '@angular/core';
import { FilterComponent } from './filter/filter.component';
import { AuctionComponent } from './auction/auction.component';

const buyerRoutes: Routes = [
    {
        path: '',
        redirectTo: 'home',
        pathMatch: 'full',
    },
    {
        path: 'home',
        component: AuctionComponent,
    },
];

@NgModule({
    imports: [RouterModule.forChild(buyerRoutes)],
    exports: [RouterModule],
})
export class BuyerRoutingModule {}
