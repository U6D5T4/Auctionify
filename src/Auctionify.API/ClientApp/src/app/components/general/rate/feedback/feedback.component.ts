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

    initialRatesCount: number = 10;
    addRatesCount: number = 10;
    starsCount: number = 5;

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
            pageSize: 10,
        };

        this.client.getFeedbacks(pagination).subscribe({
            next: (userRate) => {
                this.noMoreRates = userRate.hasNext;
                this.receiverRates = userRate.items;
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

    formatDate(date: Date | null): string {
        return date ? formatDate(date, 'dd LLLL, h:mm', 'en-US') : '';
    }
}
