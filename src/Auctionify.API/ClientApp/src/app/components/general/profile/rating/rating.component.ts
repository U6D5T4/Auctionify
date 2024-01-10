import { Dialog } from '@angular/cdk/dialog';
import { Component, OnInit } from '@angular/core';
import { formatDate } from '@angular/common';

import { DialogPopupComponent } from 'src/app/ui-elements/dialog-popup/dialog-popup.component';
import { AuthorizeService } from 'src/app/api-authorization/authorize.service';
import { BuyerModel, SellerModel } from 'src/app/models/users/user-models';
import { Client } from 'src/app/web-api-client';
import { Rate } from 'src/app/models/rates/rate-models';

@Component({
    selector: 'app-raiting',
    templateUrl: './rating.component.html',
    styleUrls: ['./rating.component.scss'],
})
export class RatingComponent implements OnInit {
    userProfileData: BuyerModel | SellerModel | null = null;
    senderRates: Rate[] = [];

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
        if (this.isUserBuyer()) {
            this.client.getBuyer().subscribe(
                (data: BuyerModel) => {
                    this.userProfileData = data;
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
            this.client.getSeller().subscribe(
                (data: SellerModel) => {
                    this.userProfileData = data;
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

    getAverageRate(): number {
        var average = 0;
        for (let a of this.senderRates) {
            average += a.ratingValue!;
        }
        return average / this.senderRates.length;
    }

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

    getAverageStars(): string[] {
        const averageRating = this.getAverageRate();
        const roundedAverage = Math.round(averageRating);

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
