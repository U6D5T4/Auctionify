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
    styleUrl: './pro-page.component.scss',
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

    confirmDeleteAccount() {
        const dialogRef = this.dialog.open(ChoicePopupComponent, {
            data: {
                text: ['Are you sure you want to delete your account?'],
                isError: true,
                continueBtnText: 'Delete',
                breakBtnText: 'Cancel',
                additionalText: 'This action cannot be undone',
                continueBtnColor: 'warn',
                breakBtnColor: 'primary',
            },
            autoFocus: false,
        });

        dialogRef.closed.subscribe((result) => {
            if (result === 'true') {
                this.deleteAccount();
            }
        });
    }

    deleteAccount() {
        this.client.deleteAccount().subscribe({
            next: (data: any) => {
                this.snackBar.open(
                    'Your account has been successfully deleted',
                    'Close',
                    {
                        horizontalPosition: 'center',
                        verticalPosition: 'bottom',
                        duration: 5000,
                        panelClass: ['success-snackbar'],
                    }
                );
                this.authorizeService.logout();
                this.router.navigate(['/home']);
            },
            error: (result: any) => {
                if (this.isUserBuyer()) {
                    this.errorMessage =
                        'You cannot delete your account because you have bids in active auctions';
                } else if (this.isUserSeller()) {
                    this.errorMessage =
                        "You can't delete your account if you have at least one lot with either active, upcoming, pending approval or reopened status";
                }

                this.snackBar.open(this.errorMessage, 'Close', {
                    horizontalPosition: 'center',
                    verticalPosition: 'bottom',
                    duration: 7000,
                    panelClass: ['error-snackbar'],
                });
            },
        });
    }
}
