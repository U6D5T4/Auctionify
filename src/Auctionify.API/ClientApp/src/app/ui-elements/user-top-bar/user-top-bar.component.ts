import { Component, Input } from '@angular/core';

import { AuthorizeService } from 'src/app/api-authorization/authorize.service';
import { BuyerModel, SellerModel } from 'src/app/models/users/user-models';

@Component({
    selector: 'app-user-top-bar',
    templateUrl: './user-top-bar.component.html',
    styleUrls: ['./user-top-bar.component.scss'],
})
export class UserTopBarComponent {
    @Input()
    userProfileData: BuyerModel | SellerModel | null = null;

    constructor(private authorizeService: AuthorizeService) {}

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

    isUserSeller(): boolean {
        return this.authorizeService.isUserSeller();
    }
}
