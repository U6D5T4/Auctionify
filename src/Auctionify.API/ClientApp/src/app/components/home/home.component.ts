import { Dialog } from '@angular/cdk/dialog';
import { Component, effect } from '@angular/core';
import { Router } from '@angular/router';
import { AuthorizeService } from 'src/app/api-authorization/authorize.service';
import { FilterComponent } from '../buyer/filter/filter.component';

@Component({
    selector: 'app-home',
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.scss'],
})
export class HomeComponent {
    constructor(
        private router: Router,
        private authService: AuthorizeService,
        private dialog: Dialog
    ) {
        effect(() => {
            if (this.authService.isUserSeller())
                this.router.navigate(['/seller/']);
        });

        effect(() => {
            if (this.authService.isUserBuyer())
                this.dialog.open(FilterComponent);
        });
    }
}
