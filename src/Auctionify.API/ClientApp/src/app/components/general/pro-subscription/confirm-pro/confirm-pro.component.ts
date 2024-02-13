import { DialogRef } from '@angular/cdk/dialog';
import { Component } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';

import { Client } from 'src/app/web-api-client';

@Component({
    selector: 'app-confirm-pro',
    templateUrl: './confirm-pro.component.html',
    styleUrls: ['./confirm-pro.component.scss'],
})
export class ConfirmProComponent {
    constructor(
        private dialogRef: DialogRef<ConfirmProComponent>,
        private snackBar: MatSnackBar,
        private client: Client,
        private router: Router
    ) {}

    confirmProSubscription() {
        this.client.subscribeUserToPro().subscribe({
            next: (res: boolean) => {
                if (res) {
                    this.router.navigate(['/home']).then(() => {
                        window.location.reload();
                    });
                } else {
                    this.snackBar.open(
                        'An error occurred while subscribing to Pro',
                        'Close',
                        {
                            horizontalPosition: 'center',
                            verticalPosition: 'bottom',
                            duration: 5000,
                            panelClass: ['error-snackbar'],
                        }
                    );
                }
            },
        });

        this.dialogRef.close();
    }

    closeDialog() {
        this.dialogRef.close();
    }
}
