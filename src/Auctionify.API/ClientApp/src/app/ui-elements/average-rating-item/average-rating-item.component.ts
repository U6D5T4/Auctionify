import { Component, Input } from '@angular/core';
import { SellerModel, BuyerModel } from 'src/app/models/users/user-models';

@Component({
    selector: 'app-average-rating-item',
    templateUrl: './average-rating-item.component.html',
    styleUrls: ['./average-rating-item.component.scss'],
})
export class AverageRatingItemComponent {
    @Input()
    userProfileData: BuyerModel | SellerModel | null = null;

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
        const total = this.userProfileData?.ratesCount!;
        return total > 0 ? `${(count / total) * 100}%` : '0%';
    }
}
