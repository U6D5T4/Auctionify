import { Component, OnInit, effect } from '@angular/core';
import { Router } from '@angular/router';

import { AuthorizeService } from 'src/app/api-authorization/authorize.service';

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

    constructor(private authService: AuthorizeService) {}

    logout() {
        this.authService.logout();
    }
}
