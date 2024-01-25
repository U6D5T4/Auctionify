import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RatingComponent } from './rating/rating.component';
import { FeedbackComponent } from './feedback/feedback.component';

const profileRoutes: Routes = [
    {
        path: '',
        component: RatingComponent,
        pathMatch: 'full',
    },
    {
        path: 'feedback',
        component: FeedbackComponent,
        pathMatch: 'full',
    },
];

@NgModule({
    imports: [RouterModule.forChild(profileRoutes)],
    exports: [RouterModule],
})
export class RateRoutingModule {}
