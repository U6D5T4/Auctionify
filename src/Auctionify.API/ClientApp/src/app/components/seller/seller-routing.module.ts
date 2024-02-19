import { RouterModule, Routes } from '@angular/router';
import { DashboardComponent } from './dashboard/dashboard.component';
import { CreateLotComponent } from './create-lot/create-lot.component';
import { NgModule } from '@angular/core';
import { AnalyticsComponent } from './analytics/analytics.component';

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
        path: 'analytics',
        component: AnalyticsComponent,
        pathMatch: 'full',
        canActivate: []
    }
];

@NgModule({
    imports: [RouterModule.forChild(sellerRoutes)],
    exports: [RouterModule],
})
export class sellerRoutingModule { }
