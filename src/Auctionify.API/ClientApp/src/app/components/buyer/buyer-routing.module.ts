import { RouterModule, Routes } from '@angular/router';
import { NgModule } from '@angular/core';
import { FilterComponent } from './filter/filter.component';

const sellerRoutes: Routes = [
    {
        path: '',
        redirectTo: 'home',
        pathMatch: 'full',
    },
    {
        path: 'home',
        component: FilterComponent,
    },
];

@NgModule({
    imports: [RouterModule.forChild(sellerRoutes)],
    exports: [RouterModule],
})
export class BuyerRoutingModule {}
