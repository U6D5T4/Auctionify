import { Dialog } from '@angular/cdk/dialog';
import { Component, OnInit } from '@angular/core';
import { formatDate } from '@angular/common';

import { DialogPopupComponent } from 'src/app/ui-elements/dialog-popup/dialog-popup.component';
import { AuthorizeService } from 'src/app/api-authorization/authorize.service';
import { BuyerModel, SellerModel } from 'src/app/models/users/user-models';
import { Client } from 'src/app/web-api-client';
import { UserDataValidatorService } from 'src/app/services/user-data-validator/user-data-validator.service';
import { Rate, RatePaginationModel } from 'src/app/models/rates/rate-models';
import { RateCalculatorService } from 'src/app/services/rate-calculator/rate-calculator.service';

@Component({
    selector: 'app-rating',
    templateUrl: './rating.component.html',
    styleUrls: ['./rating.component.scss'],
})
export class RatingComponent implements OnInit {
    userProfileData: BuyerModel | SellerModel | null = null;
    senderRates: Rate[] = [];
    IsSentRates: boolean = true;

    initialRatesCount: number = 10;
    addRatesCount: number = 10;

    noMoreRates: boolean = false;

    constructor(
        private authorizeService: AuthorizeService,
        private client: Client,
        public dialog: Dialog,
        public userDataValidator: UserDataValidatorService,
        public ratesCalculator: RateCalculatorService
    ) {}

    ngOnInit(): void {
        this.fetchUserProfileData();
        this.fetchRatesData();
    }

    openDialog(text: string[], error: boolean) {
        const dialogRef = this.dialog.open<string>(DialogPopupComponent, {
            data: {
                text,
                isError: error,
            },
        });

        dialogRef.closed.subscribe(() => {});
    }

    private fetchUserProfileData() {
        if (this.isUserBuyer()) {
            this.client.getBuyer().subscribe({
                next: (data: BuyerModel) => {
                    this.userProfileData = data;
                    this.userDataValidator.validateUserProfileData(
                        this.userProfileData
                    );
                },
                error: (error) => {
                    this.openDialog(
                        error.errors || [
                            'Something went wrong, please try again later',
                        ],
                        true
                    );
                },
            });
        } else if (this.isUserSeller()) {
            this.client.getSeller().subscribe({
                next: (data: SellerModel) => {
                    this.userProfileData = data;
                    this.userDataValidator.validateUserProfileData(
                        this.userProfileData
                    );
                },
                error: (error) => {
                    this.openDialog(
                        error.errors || [
                            'Something went wrong, please try again later',
                        ],
                        true
                    );
                },
            });
        }
    }

    private fetchRatesData() {
        const pagination: RatePaginationModel = {
            pageIndex: 0,
            pageSize: this.initialRatesCount,
        };

        this.client.getRates(pagination).subscribe({
            next: (userRate) => {
                this.noMoreRates = userRate.hasNext;
                this.senderRates = userRate.items;
            },
            error: (error) => {
                this.openDialog(
                    error.errors || [
                        'Something went wrong, please try again later',
                    ],
                    true
                );
            },
        });
    }

    loadMoreRates(): void {
        this.initialRatesCount += this.addRatesCount;
        this.fetchRatesData();
    }

    isUserSeller(): boolean {
        return this.authorizeService.isUserSeller();
    }

    isUserBuyer(): boolean {
        return this.authorizeService.isUserBuyer();
    }

    isUserHaveRates(): boolean {
        return this.senderRates.length == 0;
    }

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
}
