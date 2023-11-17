import { Component, OnInit, computed, effect } from '@angular/core';
import { AuthorizeService } from './api-authorization/authorize.service';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss'],
})
export class AppComponent {
    isUserBuyer: boolean = false;
    isUserSeller: boolean = false;

    private isBuyerEffect = effect(() => {
        this.isUserBuyer = this.authService.isUserBuyer();
    });

    private isSellectEffect = effect(() => {
        this.isUserSeller = this.authService.isUserSeller();
    });

    constructor(private authService: AuthorizeService) {}

    title = 'ClientApp';
}
