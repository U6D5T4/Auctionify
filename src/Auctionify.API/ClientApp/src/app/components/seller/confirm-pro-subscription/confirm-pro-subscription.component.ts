import { DialogRef } from '@angular/cdk/dialog';
import { Component } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
    selector: 'app-confirm-pro-subscription',
    templateUrl: './confirm-pro-subscription.component.html',
    styleUrls: ['./confirm-pro-subscription.component.scss'],
})
export class ConfirmProSubscriptionComponent {
    constructor(
        private dialogRef: DialogRef<ConfirmProSubscriptionComponent>,
        private snackBar: MatSnackBar
    ) {}

    ngOnInit(): void {}

    confirmProSubscription() {
        this.dialogRef.close();
    }

    closeDialog() {
        this.dialogRef.close();
    }
}
