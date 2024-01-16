import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { isSellerGuard } from './guards/seller/is-seller.guard';
import { isBuyerGuard } from './guards/buyer/is-buyer.guard';
import { LotProfileComponent } from './components/general/lot-profile/lot-profile.component';
import { isLoggedInGuard } from './guards/is-logged-in.guard';
import { AuctionComponent } from './components/general/home/auction/auction.component';

const routes: Routes = [
    { path: '', redirectTo: 'home', pathMatch: 'full' },
    { path: 'home', component: AuctionComponent },
    {
        path: 'profile',
        loadChildren: () =>
            import('./components/general/profile/profile.module').then(
                (m) => m.ProfileModule
            ),
        data: { breadcrumb: { skip: true } },
        canActivate: [isLoggedInGuard],
    },
    {
        path: 'rating',
        loadChildren: () =>
            import('./components/general/rate/rate.module').then(
                (m) => m.RateModule
            ),
        data: { breadcrumb: { skip: true } },
        canActivate: [isLoggedInGuard],
    },
    {
        path: 'seller',
        loadChildren: () =>
            import('./components/seller/seller.module').then(
                (m) => m.SellerModule
            ),
        data: { breadcrumb: { skip: true } },
        canActivate: [isSellerGuard],
    },
    {
        path: 'buyer',
        loadChildren: () =>
            import('./components/buyer/buyer.module').then(
                (m) => m.BuyerModule
            ),
        data: { breadcrumb: { skip: true } },
        canActivate: [isBuyerGuard],
    },
    {
        path: 'get-lot/:id',
        component: LotProfileComponent,
        canActivate: [isLoggedInGuard],
    },
];

@NgModule({
    imports: [
        RouterModule.forRoot(routes, { scrollPositionRestoration: 'enabled' }),
    ],
    exports: [RouterModule],
})
export class AppRoutingModule {}
