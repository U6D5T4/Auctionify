import { Dialog } from '@angular/cdk/dialog';
import { Component } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AuthorizeService } from 'src/app/api-authorization/authorize.service';
import { UserDataValidatorService } from 'src/app/services/user-data-validator/user-data-validator.service';
import { ChoicePopupComponent } from 'src/app/ui-elements/choice-popup/choice-popup.component';
import { Client } from 'src/app/web-api-client';

@Component({
    selector: 'app-pro-subscription',
    templateUrl: './pro-subscription.component.html',
    styleUrls: ['./pro-subscription.component.scss'],
})
export class ProSubscriptionComponent {
    constructor(
        private authorizeService: AuthorizeService,
        private client: Client,
        public dialog: Dialog,
        private snackBar: MatSnackBar
    ) {}

    // create custom modal component with pro like look
    // confirmSubscription() {
    //     const dialogRef = this.dialog.open(ChoicePopupComponent, {
    //         data: {
    //             text: ['Are you sure you want to subscribe to our Pro plan?'],
    //             isError: true,
    //             continueBtnText: 'Subscribe',
    //             breakBtnText: 'Cancel',
    //             continueBtnColor: 'primary',
    //             breakBtnColor: 'warn',
    //         },
    //         autoFocus: false,
    //     });
    // }
}
