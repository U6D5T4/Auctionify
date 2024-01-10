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

    constructor(
        private authorizeService: AuthorizeService,
        private client: Client,
        private router: Router,
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
                    this.getRatingData();
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
                    this.getRatingData();
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

    private validate() {
        if (!this.userProfileData?.averageRate) {
            this.userProfileData!.averageRate = 0;
        } else if (!this.userProfileData?.ratesCount) {
            this.userProfileData!.ratesCount = 0;
        }
    }

    private addRates() {
        this.senderRates = this.userProfileData?.senderRates!;
    }

    isUserSeller(): boolean {
        return this.authorizeService.isUserSeller();
    }

    isUserBuyer(): boolean {
        return this.authorizeService.isUserBuyer();
    }

    getRatingData() {
        for (let a of this.senderRates) {
            if (a.ratingValue != null) {
                this.ratings[
                    a.ratingValue.toString() as '1' | '2' | '3' | '4' | '5'
                ]++;
            }
        }
    }

    ratings = {
        '1': 0,
        '2': 0,
        '3': 0,
        '4': 0,
        '5': 0,
    };

    getPercentage(count: number): string {
        const total = this.getTotalCount();
        return total > 0 ? `${(count / total) * 100}%` : '0%';
    }

    getTotalCount(): number {
        return this.senderRates.length;
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
}
