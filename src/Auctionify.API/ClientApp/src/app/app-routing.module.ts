import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { HomeComponent } from './components/home/home.component';
import { isSellerGuard } from './guards/seller/is-seller.guard';
import { isBuyerGuard } from './guards/buyer/is-buyer.guard';

const routes: Routes = [
    { path: '', redirectTo: 'home', pathMatch: 'full' },
    { path: 'home', component: HomeComponent },
    {
        path: 'general',
        loadChildren: () =>
            import('./components/general/general.module').then(
                (m) => m.GeneralModule
            ),
        data: { breadcrumb: { skip: true } }
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
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule],
})
export class AppRoutingModule { }
