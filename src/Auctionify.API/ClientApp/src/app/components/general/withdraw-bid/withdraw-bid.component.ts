import { DIALOG_DATA, DialogRef } from '@angular/cdk/dialog';
import { Component, OnInit, Inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Client } from '../../../web-api-client';

@Component({
    selector: 'app-withdraw-bid',
    templateUrl: './withdraw-bid.component.html',
    styleUrls: ['./withdraw-bid.component.scss'],
})
export class WithdrawBidComponent implements OnInit {
    bidId: number | null = null;
    errorMessage: string = '';

    constructor(
        private dialogRef: DialogRef<WithdrawBidComponent>,
        @Inject(DIALOG_DATA) private data: any,
        private snackBar: MatSnackBar,
        private apiClient: Client
    ) {
        this.bidId = this.data?.bidId ?? null;
    }

    ngOnInit(): void {}

    confirmWithdraw() {
        if (this.bidId !== null) {
            this.apiClient.removeBid(this.bidId).subscribe({
                next: (response) => {
                    this.showSnackBar('Bid successfully withdrawn', 'success');
                    this.dialogRef.close();
                },
                error: (error) => {
                    if (
                        JSON.parse(error) &&
                        JSON.parse(error)?.errors?.length
                    ) {
                        this.errorMessage =
                            JSON.parse(error)?.errors[0]?.ErrorMessage;
                        if (
                            this.errorMessage ===
                            'Bid with this Id does not exist'
                        ) {
                            this.errorMessage =
                                "Withdraw Error! You haven't placed a bid on this lot yet";
                        }
                        this.showSnackBar(this.errorMessage, 'error');
                        this.dialogRef.close();
                    }
                },
            });
        }
    }

    showSnackBar(message: string, messageType: 'success' | 'error') {
        let panelClass = ['custom-snackbar'];
        if (messageType === 'success') {
            panelClass.push('success-snackbar');
        } else if (messageType === 'error') {
            panelClass.push('error-snackbar');
        }

        this.snackBar.open(message, 'Close', {
            duration: 5000,
            horizontalPosition: 'center',
            verticalPosition: 'bottom',
            panelClass: panelClass,
        });
    }

    closeDialog() {
        this.dialogRef.close();
    }
}
