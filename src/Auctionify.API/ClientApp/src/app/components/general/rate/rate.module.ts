import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';

import { RatingComponent } from './rating/rating.component';
import { FeedbackComponent } from './feedback/feedback.component';
import { RateRoutingModule } from './rate-routing.module';
import { ProfileModule } from '../profile/profile.module';
import { MatButtonModule } from '@angular/material/button';

@NgModule({
    imports: [
        CommonModule,
        MatIconModule,
        RateRoutingModule,
        ProfileModule,
        MatButtonModule,
    ],

    declarations: [RatingComponent, FeedbackComponent],
})
export class RateModule {}
