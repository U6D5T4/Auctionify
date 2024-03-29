import { Component, Input } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

import { Rate } from 'src/app/models/rates/rate-models';
import { SellerModel, BuyerModel } from 'src/app/models/users/user-models';
import { RateCalculatorService } from 'src/app/services/rate-calculator/rate-calculator.service';

@Component({
    selector: 'app-average-rating-item',
    templateUrl: './average-rating-item.component.html',
    styleUrls: ['./average-rating-item.component.scss'],
})
export class AverageRatingItemComponent {
    @Input()
    userProfileData: BuyerModel | SellerModel | null = null;
    @Input()
    IsBtnVisible!: boolean;
    @Input()
    width!: number;

    constructor(
        public ratesCalculator: RateCalculatorService,
        private activeRoute: ActivatedRoute,
        private router: Router
    ) {}

    getPercentage(count: number): string {
        const total = this.getTotalCount();
        return total > 0 ? `${(count / total) * 100}%` : '0%';
    }

    getTotalCount(): number {
        let totalCount = 0;
        for (const key in this.userProfileData?.starCounts!) {
            if (this.userProfileData?.starCounts!.hasOwnProperty(key)) {
                totalCount += this.userProfileData?.starCounts![key];
            }
        }

        return totalCount;
    }

    ratesEmpty: { [key: number]: number } = {
        1: 0,
        2: 0,
        3: 0,
        4: 0,
        5: 0,
    };

    isUserHaveRates(): boolean {
        return this.getTotalCount() == 0;
    }

    onShowAllCommentsClick() {
        const userId = this.activeRoute.snapshot.params['id'];

        if (userId) {
            this.router.navigate(['profile', 'user', userId, 'rating']);
        } else {
            this.router.navigate(['rating']);
        }
    }
}
