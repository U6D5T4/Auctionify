import { Dialog } from '@angular/cdk/dialog';
import { Component } from '@angular/core';

import { ConfirmProComponent } from '../confirm-pro/confirm-pro.component';

@Component({
    selector: 'app-upgrade-to-pro',
    templateUrl: './upgrade-to-pro.component.html',
    styleUrls: ['./upgrade-to-pro.component.scss'],
})
export class UpgradeToProComponent {
    public constructor(private dialog: Dialog) {}

    openConfirmProSubscriptionDialog() {
        const dialogRef = this.dialog.open(ConfirmProComponent, {
            autoFocus: false,
        });
    }
}
