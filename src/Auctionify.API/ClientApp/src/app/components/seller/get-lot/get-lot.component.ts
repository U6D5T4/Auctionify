import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Observable, of, switchMap } from 'rxjs';

import { Client, SellerGetLotResponse } from 'src/app/web-api-client';
import { SignalRService } from 'src/app/services/signalr-service/signalr.service';

@Component({
    selector: 'app-get-lot-seller',
    templateUrl: './get-lot.component.html',
    styleUrls: ['./get-lot.component.scss'],
})
export class GetLotComponent implements OnInit {
    lotId: number = 0;
    lot$!: Observable<SellerGetLotResponse>;

    constructor(
        private apiClient: Client,
        private route: ActivatedRoute,
        private signalRService: SignalRService
    ) {}

    ngOnInit() {
        this.route.paramMap
            .pipe(
                switchMap((params) => {
                    this.lotId = Number(params.get('id')) || 0;
                    return this.apiClient.getOneLotForSeller(this.lotId);
                })
            )
            .subscribe(async (lot) => {
                this.lot$ = of(lot);

                this.signalRService.onReceiveBidNotification(() => {
                    this.getLotFromRoute();
                }, this.lotId);

                this.signalRService.onReceiveWithdrawBidNotification(() => {
                    this.getLotFromRoute();
                }, this.lotId);

                await this.signalRService.joinLotGroup(this.lotId);
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
