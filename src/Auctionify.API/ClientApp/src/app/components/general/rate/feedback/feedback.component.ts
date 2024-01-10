import { Dialog } from '@angular/cdk/dialog';
import { Component, OnInit } from '@angular/core';
import { formatDate } from '@angular/common';

import { DialogPopupComponent } from 'src/app/ui-elements/dialog-popup/dialog-popup.component';
import { AuthorizeService } from 'src/app/api-authorization/authorize.service';
import { BuyerModel, SellerModel } from 'src/app/models/users/user-models';
import { Client } from 'src/app/web-api-client';
import { Rate, RatePaginationModel } from 'src/app/models/rates/rate-models';

@Component({
    selector: 'app-feedback',
    templateUrl: './feedback.component.html',
    styleUrls: ['./feedback.component.scss'],
})
export class FeedbackComponent {
    userProfileData: BuyerModel | SellerModel | null = null;
    receiverRates: Rate[] = [];

    constructor(
        private authorizeService: AuthorizeService,
        private client: Client,
        public dialog: Dialog
    ) {}

    ngOnInit(): void {
        this.fetchUserProfileData();
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
        const pagination: RatePaginationModel = {
            pageIndex: 0,
            pageSize: 20,
        };

        if (this.isUserBuyer()) {
            this.client.getBuyer(pagination).subscribe(
                (data: BuyerModel) => {
                    this.userProfileData = data;
                    this.validate();
                    this.addRates();
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
        } else if (this.isUserSeller()) {
            this.client.getSeller(pagination).subscribe(
                (data: SellerModel) => {
                    this.userProfileData = data;
                    this.validate();
                    this.addRates();
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

    private validate() {
        if (!this.userProfileData?.averageRate) {
            this.userProfileData!.averageRate = 0;
        } else if (!this.userProfileData?.ratesCount) {
            this.userProfileData!.ratesCount = 0;
        }
    }

    private addRates() {
        this.receiverRates = this.userProfileData?.receiverRates!;
    }

    isUserSeller(): boolean {
        return this.authorizeService.isUserSeller();
    }

    isUserBuyer(): boolean {
        return this.authorizeService.isUserBuyer();
    }

    formatDate(date: Date | null): string {
        return date ? formatDate(date, 'dd LLLL, h:mm', 'en-US') : '';
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
}
