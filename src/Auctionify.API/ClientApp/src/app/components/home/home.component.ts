import { Dialog } from '@angular/cdk/dialog';
import { Component, effect } from '@angular/core';
import { Router } from '@angular/router';
import { AuthorizeService } from 'src/app/api-authorization/authorize.service';
import {
    FilterComponent,
    FilterResult,
} from '../buyer/filter/filter.component';
import { Client } from 'src/app/web-api-client';
import { FilterLot } from 'src/app/models/lots/filter';

@Component({
    selector: 'app-home',
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.scss'],
})
export class HomeComponent {
    constructor(
        private router: Router,
        private authService: AuthorizeService,
        private dialog: Dialog,
        private client: Client
    ) {
        effect(() => {
            if (this.authService.isUserSeller())
                this.router.navigate(['/seller/']);
        });

        effect(() => {
            if (this.authService.isUserBuyer()) {
                const dialogSubscriber = this.dialog.open(FilterComponent);

                dialogSubscriber.closed.subscribe((res: any) => {
                    if (res) {
                        const data = JSON.parse(res) as FilterResult;

                        const filterData: FilterLot = {
                            ...data,
                            sortDir: null,
                            sortField: null,
                        };

                        this.client
                            .filterLots(filterData)
                            .subscribe((res) => {});
                    }
                });
            }
        });
    }
}
