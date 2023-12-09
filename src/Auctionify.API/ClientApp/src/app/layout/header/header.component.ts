import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { AuthorizeService } from 'src/app/api-authorization/authorize.service';

@Component({
    selector: 'app-header',
    templateUrl: './header.component.html',
    styleUrls: ['./header.component.scss'],
})
export class HeaderComponent implements OnInit {
    isAuthenticated: boolean = false;
    isBuyer: boolean = false;

    constructor(
        private authService: AuthorizeService,
        private router: Router
    ) {}

    ngOnInit() {
        this.checkAuthentication();
        this.checkUserRole();

        this.authService.authenticationChanged.subscribe((status) => {
            this.isAuthenticated = status;
            this.checkUserRole();
        });
    }

    checkAuthentication() {
        this.isAuthenticated = this.authService.isUserLoggedIn();
    }

    checkUserRole() {
        this.isBuyer = this.authService.isUserBuyer();
    }

    logout() {
        this.authService.logout();
    }
}
