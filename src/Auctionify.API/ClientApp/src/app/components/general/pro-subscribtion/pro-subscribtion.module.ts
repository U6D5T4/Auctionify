import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProPageComponent } from './pro-page/pro-page.component';
import { MatIconModule } from '@angular/material/icon';
import { SubscriptionRoutingModule } from './subscription-routing.module';
import { MatButtonModule } from '@angular/material/button';
import { UiElementsModule } from 'src/app/ui-elements/ui-elements.module';
import { ProfileModule } from '../profile/profile.module';
import { UpgradeToProComponent } from './upgrade-to-pro/upgrade-to-pro.component';
import { ConfirmProComponent } from './confirm-pro/confirm-pro.component';

@NgModule({
    imports: [
        CommonModule,
        MatIconModule,
        MatButtonModule,
        UiElementsModule,
        SubscriptionRoutingModule,
        ProfileModule,
        MatButtonModule,
    ],
    declarations: [ProPageComponent, UpgradeToProComponent, ConfirmProComponent],
})
export class ProSubscribtionModule {}
