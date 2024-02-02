import { Injectable } from '@angular/core';
import { BuyerModel, SellerModel } from 'src/app/models/users/user-models';

@Injectable({
    providedIn: 'root',
})
export class UserDataValidatorService {
    private userIdString: string = 'userId';

    constructor() {}

    validateUserProfileData(
        userProfileData: BuyerModel | SellerModel | null
    ): void {
        if (!userProfileData?.averageRate) {
            userProfileData!.averageRate = 0;
        } else if (!userProfileData?.ratesCount) {
            userProfileData!.ratesCount = 0;
        }
    }
}
