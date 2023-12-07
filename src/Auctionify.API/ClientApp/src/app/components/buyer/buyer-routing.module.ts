import { RouterModule, Routes } from '@angular/router';
import { NgModule } from '@angular/core';
import { FilterComponent } from './filter/filter.component';

const BUYER_ROUTES: Routes = [
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
    imports: [RouterModule.forChild(BUYER_ROUTES)],
    exports: [RouterModule],
})
export class BuyerRoutingModule {}
