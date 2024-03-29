import { RouterModule, Routes } from '@angular/router';
import { DashboardComponent } from './dashboard/dashboard.component';
import { CreateLotComponent } from './create-lot/create-lot.component';
import { NgModule } from '@angular/core';
import { ActiveLotsComponent } from './active-lots/active-lots.component';
import { AnalyticsComponent } from './analytics/analytics.component';
import { isProGuard } from 'src/app/guards/seller/is-pro.guard';

const sellerRoutes: Routes = [
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
        path: 'create-lot',
        component: CreateLotComponent,
        pathMatch: 'full',
    },
    {
        path: 'update-lot/:id',
        component: CreateLotComponent,
        pathMatch: 'full',
    },
    {
        path: 'active-lots',
        component: ActiveLotsComponent,
        pathMatch: 'full',
    },
    {
        path: 'analytics',
        component: AnalyticsComponent,
        pathMatch: 'full',
        canActivate: [isProGuard],
    },
];

@NgModule({
    imports: [RouterModule.forChild(sellerRoutes)],
    exports: [RouterModule],
})
export class sellerRoutingModule {}
