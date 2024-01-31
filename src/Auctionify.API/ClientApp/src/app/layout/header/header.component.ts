import { Component, effect } from '@angular/core';
import { Router } from '@angular/router';

import { ChoicePopupComponent } from 'src/app/ui-elements/choice-popup/choice-popup.component';
import { AuthorizeService } from 'src/app/api-authorization/authorize.service';
import { Dialog } from '@angular/cdk/dialog';

@Component({
    selector: 'app-header',
    templateUrl: './header.component.html',
    styleUrls: ['./header.component.scss'],
})
export class HeaderComponent {
    isUserBuyer: boolean = false;

    private isBuyerEffect = effect(() => {
        this.isUserBuyer = this.authService.isUserBuyer();
    });

    constructor(
        private authService: AuthorizeService,
        private router: Router,
        private dialog: Dialog
    ) {}

    logout() {
        const dialogRef = this.dialog.open(ChoicePopupComponent, {
            data: {
                text: ['Are you sure to log out?'],
                isError: true,
                continueBtnText: 'Log Out',
                breakBtnText: 'Cancel',
                additionalText: 'Just double-check before you go',
                continueBtnColor: 'warn',
                breakBtnColor: 'warn',
                breakBtnFocus: 'break',
            },
            autoFocus: false,
        });
        dialogRef.closed.subscribe((result) => {
            if (result === 'true') {
                this.authService.logout();
                this.router.navigate(['/home']).then(() => {
                    window.location.reload();
                  });
            }
        });
    }
}
