import { Dialog } from '@angular/cdk/dialog';
import { Component, OnInit } from '@angular/core';

import { Rate, RatePaginationModel } from 'src/app/models/rates/rate-models';
import { DialogPopupComponent } from 'src/app/ui-elements/dialog-popup/dialog-popup.component';
import { Client } from 'src/app/web-api-client';
import { SellerModel } from 'src/app/models/users/user-models';

@Component({
    selector: 'app-dashboard',
    templateUrl: './dashboard.component.html',
    styleUrls: ['./dashboard.component.scss'],
})
export class DashboardComponent implements OnInit {
    userProfileData: SellerModel | null = null;
    senderRates: Rate[] = [];

    constructor(private client: Client, public dialog: Dialog) {}

    ngOnInit(): void {
        this.fetchUserProfileData();
        this.fetchRatesData();
    }

    private fetchUserProfileData() {
        this.client.getSeller().subscribe(
            (data: SellerModel) => {
                this.userProfileData = data;
                this.validate();
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

    private fetchRatesData() {
        const pagination: RatePaginationModel = {
            pageIndex: 0,
            pageSize: 2,
        };

        this.client.getRates(pagination).subscribe(
            (userRate) => {
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

    private validate() {
        if (!this.userProfileData?.averageRate) {
            this.userProfileData!.averageRate = 0;
        } else if (!this.userProfileData?.ratesCount) {
            this.userProfileData!.ratesCount = 0;
        }
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
}
