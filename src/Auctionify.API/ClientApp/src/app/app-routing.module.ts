import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { HomeComponent } from './components/home/home.component';
import { isSellerGuard } from './guards/seller/is-seller.guard';
import { isBuyerGuard } from './guards/buyer/is-buyer.guard';
import { LotProfileComponent } from './components/general/lot-profile/lot-profile.component';
import { isLoggedInGuard } from './guards/is-logged-in.guard';

const routes: Routes = [
    { path: '', redirectTo: 'home', pathMatch: 'full' },
    { path: 'home', component: HomeComponent },
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
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule],
})
export class AppRoutingModule {}
