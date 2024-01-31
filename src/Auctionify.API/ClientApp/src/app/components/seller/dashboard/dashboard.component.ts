import { Dialog } from '@angular/cdk/dialog';
import { Component, OnInit } from '@angular/core';

import { Rate, RatePaginationModel } from 'src/app/models/rates/rate-models';
import { DialogPopupComponent } from 'src/app/ui-elements/dialog-popup/dialog-popup.component';
import { Client } from 'src/app/web-api-client';
import { SellerModel } from 'src/app/models/users/user-models';
import { UserDataValidatorService } from 'src/app/services/user-data-validator/user-data-validator.service';

@Component({
    selector: 'app-dashboard',
    templateUrl: './dashboard.component.html',
    styleUrls: ['./dashboard.component.scss'],
})
export class DashboardComponent implements OnInit {
    userProfileData: SellerModel | null = null;
    senderRates: Rate[] = [];
    IsBtnVisible: boolean = true;

    constructor(
        private client: Client,
        public dialog: Dialog,
        private userDataValidator: UserDataValidatorService
    ) {}

    ngOnInit(): void {
        this.fetchUserProfileData();
        this.fetchRatesData();
    }

    private fetchUserProfileData() {
        this.client.getSeller().subscribe(
            (data: SellerModel) => {
                this.userProfileData = data;
                this.userDataValidator.validateUserProfileData(data);
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

    openDialog(text: string[], error: boolean) {
        const dialogRef = this.dialog.open<string>(DialogPopupComponent, {
            data: {
                text,
                isError: error,
            },
        });

        dialogRef.closed.subscribe((res) => {});
    }
}
