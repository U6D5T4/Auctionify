import { RouterModule, Routes } from '@angular/router';
import { NgModule } from '@angular/core';
import { DashboardComponent } from './dashboard/dashboard.component';
import { WishlistComponent } from './wishlist/wishlist.component';

const BUYER_ROUTES: Routes = [
    {
        path: '',
        redirectTo: 'home',
        pathMatch: 'full',
    },
    {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full',
    },
    {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full',
    },
    {
        path: 'dashboard',
        component: DashboardComponent,
    },
    {
        path: 'wishlist',
        component: WishlistComponent,
    },
];

@NgModule({
    imports: [RouterModule.forChild(BUYER_ROUTES)],
    exports: [RouterModule],
})
export class BuyerRoutingModule {}
