import { DIALOG_DATA, DialogRef } from '@angular/cdk/dialog';
import { Component, Inject, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

import { Client } from 'src/app/web-api-client';

@Component({
    selector: 'app-remove-from-watchlist',
    templateUrl: './remove-from-watchlist.component.html',
    styleUrls: ['./remove-from-watchlist.component.scss'],
})
export class RemoveFromWatchlistComponent implements OnInit {
    lotId: number | null = null;
    errorMessage: string = '';

    constructor(
        private dialogRef: DialogRef<RemoveFromWatchlistComponent>,
        @Inject(DIALOG_DATA) private data: any,
        private snackBar: MatSnackBar,
        private apiClient: Client
    ) {
        this.lotId = this.data?.lotId ?? null;
    }

    ngOnInit(): void {}

    confirmRemove() {
        if (this.lotId !== null) {
            this.apiClient.removeFromWatchList(this.lotId).subscribe({
                next: (response) => {
                    console.log(response);
                    this.showSnackBar(
                        'Lot successfully removed from watchlist',
                        'success'
                    );
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
                            'Lot with this Id does not exist'
                        ) {
                            this.errorMessage =
                                "Remove Error! You haven't added this lot to your watchlist yet";
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
