import { Dialog } from '@angular/cdk/dialog';
import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';

import { AuthorizeService } from 'src/app/api-authorization/authorize.service';
import { BuyerModel, SellerModel } from 'src/app/models/users/user-models';
import { UserDataValidatorService } from 'src/app/services/user-data-validator/user-data-validator.service';
import { ChoicePopupComponent } from 'src/app/ui-elements/choice-popup/choice-popup.component';
import { DialogPopupComponent } from 'src/app/ui-elements/dialog-popup/dialog-popup.component';
import { Client } from 'src/app/web-api-client';

@Component({
    selector: 'app-pro-page',
    templateUrl: './pro-page.component.html',
    styleUrls: ['./pro-page.component.scss'],
})
export class ProPageComponent implements OnInit {
    userProfileData: BuyerModel | SellerModel | null = null;
    errorMessage!: string;

    constructor(
        private authorizeService: AuthorizeService,
        private client: Client,
        public dialog: Dialog,
        public userDataValidator: UserDataValidatorService,
        private router: Router,
        private snackBar: MatSnackBar
    ) {}

    ngOnInit(): void {
        this.fetchUserProfileData();
    }

    private fetchUserProfileData() {
        if (this.isUserBuyer()) {
            this.client.getBuyer().subscribe(
                (data: BuyerModel) => {
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
        } else if (this.isUserSeller()) {
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
    }

    isUserSeller(): boolean {
        return this.authorizeService.isUserSeller();
    }

    isUserBuyer(): boolean {
        return this.authorizeService.isUserBuyer();
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

    confirmunsubscribeFromPro() {
        const dialogRef = this.dialog.open(ChoicePopupComponent, {
            data: {
                text: ['Are you sure you want to unsubscribe?'],
                isError: true,
                continueBtnText: 'Unsubscribe',
                breakBtnText: 'Cancel',
                additionalText:
                    'This will revoke access to premium features. Please consider the implications before confirming the action',
                continueBtnColor: 'warn',
                breakBtnColor: 'warn',
            },
            autoFocus: false,
        });

        dialogRef.closed.subscribe((result) => {
            if (result === 'true') {
                this.unsubscribeFromPro();
            }
        });
    }

    unsubscribeFromPro() {
        this.client.unsubscribeUserFromPro().subscribe({
            next: (res) => {
                if (res) {
                    this.router.navigate(['/home']).then(() => {
                        window.location.reload();
                    });
                }
            },
            error: (err) => {
                this.snackBar.open(
                    'An error occurred while unsubscribing from Pro',
                    'Close',
                    {
                        horizontalPosition: 'center',
                        verticalPosition: 'bottom',
                        duration: 5000,
                        panelClass: ['error-snackbar'],
                    }
                );
            },
        });
    }
}
