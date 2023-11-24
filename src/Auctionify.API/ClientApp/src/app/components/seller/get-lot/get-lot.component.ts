import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import * as signalR from '@microsoft/signalr';
import { Observable, of, switchMap } from 'rxjs';
import { Client, SellerGetLotResponse } from 'src/app/web-api-client';
import { environment } from 'src/environments/environment';

@Component({
    selector: 'app-get-lot-seller',
    templateUrl: './get-lot.component.html',
    styleUrls: ['./get-lot.component.scss'],
})
export class GetLotComponent implements OnInit {
    private apiUrl = environment.apiUrl;

    constructor(private apiClient: Client, private route: ActivatedRoute) {}

    lotId: number = 0;
    lot$!: Observable<SellerGetLotResponse>;

    ngOnInit() {
        this.getLotFromRoute();

        const connection = new signalR.HubConnectionBuilder()
            .configureLogging(signalR.LogLevel.Information)
            .withUrl(this.apiUrl + 'api/auctionHub')
            .build();

        connection
            .start()
            .then(function () {
                console.log('SignalR connected');
            })
            .catch(function (err) {
                return console.error(err.toString());
            });

        connection.on('ReceiveBidNotification', () => {
            console.log('Bid received');
            this.getLotFromRoute();
        });
    }

    getLotFromRoute(): void {
        this.route.paramMap
            .pipe(
                switchMap((params) => {
                    this.lotId = Number(params.get('id')) || 0;
                    return this.apiClient.getOneLotForSeller(this.lotId);
                })
            )
            .subscribe((lot) => {
                this.lot$ = of(lot);
            });
    }
}
