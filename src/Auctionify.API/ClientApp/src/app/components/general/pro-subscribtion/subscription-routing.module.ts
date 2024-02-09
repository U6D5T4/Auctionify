import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ProPageComponent } from './pro-page/pro-page.component';
import { UpgradeToProComponent } from './upgrade-to-pro/upgrade-to-pro.component';

const subscriptionRoutes: Routes = [
    {
        path: 'pro-subscription',
        component: ProPageComponent,
        pathMatch: 'full',
    },
    {
        path: 'upgrade-to-pro',
        component: UpgradeToProComponent,
        pathMatch: 'full',
    },
];

@NgModule({
    imports: [RouterModule.forChild(subscriptionRoutes)],
    exports: [RouterModule],
})
export class SubscriptionRoutingModule {}
