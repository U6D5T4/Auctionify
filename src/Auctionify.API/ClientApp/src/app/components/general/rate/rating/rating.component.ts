import { Dialog } from '@angular/cdk/dialog';
import { Component, OnInit } from '@angular/core';
import { formatDate } from '@angular/common';
import { Router } from '@angular/router';

import { DialogPopupComponent } from 'src/app/ui-elements/dialog-popup/dialog-popup.component';
import { AuthorizeService } from 'src/app/api-authorization/authorize.service';
import { BuyerModel, SellerModel } from 'src/app/models/users/user-models';
import { Client } from 'src/app/web-api-client';
import { Rate, RatePaginationModel } from 'src/app/models/rates/rate-models';

@Component({
    selector: 'app-rating',
    templateUrl: './rating.component.html',
    styleUrls: ['./rating.component.scss'],
})
export class RatingComponent implements OnInit {
    userProfileData: BuyerModel | SellerModel | null = null;
    senderRates: Rate[] = [];

    initialRatesCount: number = 10;
    addRatesCount: number = 10;

    noMoreRates: boolean = false;

    constructor(
        private authorizeService: AuthorizeService,
        private client: Client,
        public dialog: Dialog
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

        dialogRef.closed.subscribe((res) => {});
    }

    private fetchUserProfileData() {
        if (this.isUserBuyer()) {
            this.client.getBuyer().subscribe({
                next: (data: BuyerModel) => {
                    this.userProfileData = data;
                    this.validate();
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
                    this.validate();
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

        this.client.getRates(pagination).subscribe(
            (userRate) => {
                this.noMoreRates = userRate.hasNext;
                this.senderRates = userRate.items;
            },
            (error) => {
                this.openDialog(
                    error.errors! || [
                        'Something went wrong, please try again later',
                    ],
                    true
                );
            }
        );
    }

    loadMoreRates(): void {
        this.initialRatesCount += this.addRatesCount;
        this.fetchRatesData();
    }

    private validate() {
        if (!this.userProfileData?.averageRate) {
            this.userProfileData!.averageRate = 0;
        } else if (!this.userProfileData?.ratesCount) {
            this.userProfileData!.ratesCount = 0;
        }
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

    formatDate(date: Date | null): string {
        return date ? formatDate(date, 'dd LLLL, h:mm', 'en-US') : '';
    }

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

    getStars(count: number): string[] {
        const stars: string[] = [];
        for (let i = 1; i <= 5; i++) {
            if (i <= count) {
                stars.push('star');
            } else {
                stars.push('star_border');
            }
        }
        return stars;
    }

    ratesEmpty: { [key: number]: number } = {
        1: 0,
        2: 0,
        3: 0,
        4: 0,
        5: 0,
    };
}
