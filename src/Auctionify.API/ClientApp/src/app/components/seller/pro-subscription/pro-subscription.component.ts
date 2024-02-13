import { Dialog } from '@angular/cdk/dialog';
import { Component } from '@angular/core';
import { ConfirmProSubscriptionComponent } from '../confirm-pro-subscription/confirm-pro-subscription.component';

@Component({
    selector: 'app-pro-subscription',
    templateUrl: './pro-subscription.component.html',
    styleUrls: ['./pro-subscription.component.scss'],
})
export class ProSubscriptionComponent {
    constructor(public dialog: Dialog) {}

    openConfirmProSubscriptionDialog() {
        const dialogRef = this.dialog.open(ConfirmProSubscriptionComponent, {
            autoFocus: false,
        });
    }
}
