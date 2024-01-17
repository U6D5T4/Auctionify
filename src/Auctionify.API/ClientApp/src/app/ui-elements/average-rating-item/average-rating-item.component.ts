import { Component, Input } from '@angular/core';

import { Rate } from 'src/app/models/rates/rate-models';
import { SellerModel, BuyerModel } from 'src/app/models/users/user-models';

@Component({
    selector: 'app-average-rating-item',
    templateUrl: './average-rating-item.component.html',
    styleUrls: ['./average-rating-item.component.scss'],
})
export class AverageRatingItemComponent {
    @Input()
    userProfileData: BuyerModel | SellerModel | null = null;
    @Input()
    senderRates: Rate[] = [];

    getAverageStars(rate: number | null): string[] {
        const averageRating = rate;

        const roundedAverage = Math.round(averageRating!);

        const stars: string[] = [];
        for (let i = 1; i <= 5; i++) {
            if (i <= roundedAverage) {
                stars.push('star');
            } else if (i - roundedAverage === 0.5) {
                stars.push('star_half');
            } else {
                stars.push('star_border');
            }
        }

        return stars;
    }

    getPercentage(count: number): string {
        const total = this.getTotalCount();
        return total > 0 ? `${(count / total) * 100}%` : '0%';
    }

    getTotalCount(): number {
        return this.senderRates.length;
    }

    ratesEmpty: { [key: number]: number } = {
        1: 0,
        2: 0,
        3: 0,
        4: 0,
        5: 0,
    };

    isUserHaveRates(): boolean {
        return this.senderRates.length == 0;
    }
}
