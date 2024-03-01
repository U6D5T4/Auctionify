import { Component, OnInit, computed, effect } from '@angular/core';
import { AuthorizeService } from './api-authorization/authorize.service';
import { GoogleMapService } from './services/map-service/map.service';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit {
    ngOnInit() {
        this.mapService.loadApi().subscribe({});
    }

    isUserBuyer: boolean = false;
    isUserSeller: boolean = false;

    private isBuyerEffect = effect(() => {
        this.isUserBuyer = this.authService.isUserBuyer();
    });

    private isSellectEffect = effect(() => {
        this.isUserSeller = this.authService.isUserSeller();
    });

    constructor(
        private authService: AuthorizeService,
        private mapService: GoogleMapService
    ) {}

    title = 'Auctionify';
}
