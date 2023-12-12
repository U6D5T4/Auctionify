import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';

import { ChoicePopupComponent } from 'src/app/ui-elements/choice-popup/choice-popup.component';
import { AuthorizeService } from 'src/app/api-authorization/authorize.service';

@Component({
  selector: 'app-profile-navbar',
  templateUrl: './profile-navbar.component.html',
  styleUrls: ['./profile-navbar.component.scss']
})
export class ProfileNavbarComponent {
  constructor(
    private authService: AuthorizeService, 
    private router: Router,
    private dialog: MatDialog
    ) {}

  logOut() {
    const dialogRef = this.dialog.open(ChoicePopupComponent, {
      data: {
        text: ['Sure you want to log out?'],
        isError: true,
        continueBtnText: 'Log Out',
        breakBtnText: 'Cancel',
        additionalText: 'Just a double-check before you go',
        continueBtnColor: 'primary',
        breakBtnColor: 'warn',
      },
    });
    dialogRef.afterClosed().subscribe((result) => {
      if (result === 'true') {
        this.authService.logout();
        this.router.navigate(['/home']);
      }
    });
  }
}
