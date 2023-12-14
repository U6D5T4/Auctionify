import { Component, OnInit } from '@angular/core';
import { Dialog } from '@angular/cdk/dialog';

import { DialogPopupComponent } from 'src/app/ui-elements/dialog-popup/dialog-popup.component';
import { AuthorizeService } from 'src/app/api-authorization/authorize.service';
import { BuyerModel, SellerModel } from 'src/app/models/users/user-models';
import { Client } from 'src/app/web-api-client';

@Component({
    selector: 'app-user-profile',
    templateUrl: './user-profile.component.html',
    styleUrls: ['./user-profile.component.scss'],
})
export class UserProfileComponent {
    userProfileData: BuyerModel | SellerModel | null = null;

    constructor(
        private authorizeService: AuthorizeService,
        private client: Client,
        public dialog: Dialog
    ) {}

    ngOnInit(): void {
        this.fetchUserProfileData();
    }

    private fetchUserProfileData() {
        if (this.isUserBuyer()) {
            this.client.getBuyer().subscribe(
                (data: BuyerModel) => {
                    this.userProfileData = data;
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

    openDialog(text: string[], error: boolean) {
        const dialogRef = this.dialog.open<string>(DialogPopupComponent, {
            data: {
                text,
                isError: error,
            },
        });

        dialogRef.closed.subscribe((res) => {});
    }

    isUserSeller(): boolean {
        return this.authorizeService.isUserSeller();
    }

    isUserBuyer(): boolean {
        return this.authorizeService.isUserBuyer();
    }
}
