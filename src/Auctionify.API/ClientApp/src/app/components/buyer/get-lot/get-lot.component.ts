import { Component, OnInit } from '@angular/core';
import { Dialog } from '@angular/cdk/dialog';
import { ActivatedRoute } from '@angular/router';
import { Observable, of, switchMap } from 'rxjs';

import { SignalRService } from 'src/app/services/signalr-service/signalr.service';
import { BidDto, BuyerGetLotResponse, Client } from 'src/app/web-api-client';
import { AddBidComponent } from '../add-bid/add-bid.component';

@Component({
    selector: 'app-get-lot-buyer',
    templateUrl: './get-lot.component.html',
    styleUrls: ['./get-lot.component.scss'],
})
export class GetLotComponent implements OnInit {
    lotId: number = 0;
    bidCount!: number;
    currency!: string;
    startingPrice!: number;
    lot$!: Observable<BuyerGetLotResponse>;
    bids$!: Observable<BidDto[]>;
    currentHighestBid!: number;

    constructor(
        private apiClient: Client,
        private route: ActivatedRoute,
        private signalRService: SignalRService,
        private dialog: Dialog
    ) {}

    async ngOnInit() {
        this.getLotFromRoute();

        await this.signalRService.joinLotGroupAfterConnection(this.lotId);

        this.signalRService.onReceiveBidNotification(() => {
            this.getLotFromRoute();
        }, this.lotId);

        this.signalRService.onReceiveWithdrawBidNotification(() => {
            this.getLotFromRoute();
        }, this.lotId);
    }

    getLotFromRoute(): void {
        this.route.paramMap
            .pipe(
                switchMap((params) => {
                    this.lotId = Number(params.get('id')) || 0;
                    return this.apiClient.getOneLotForBuyer(this.lotId);
                })
            )
            .subscribe((lot) => {
                this.lot$ = of(lot);
                this.bidCount = lot.bidCount || 0;
                this.currency = lot.currency.code || '';
                this.startingPrice = lot.startingPrice || 0;
                this.currentHighestBid = lot.bids[0]?.newPrice || 0;
            });
    }

    openBidModal(): void {
        const dialog = this.dialog.open(AddBidComponent, {
            data: {
                lotId: this.lotId,
                bidCount: this.bidCount,
                currency: this.currency,
                startingPrice: this.startingPrice,
                currentHighestBid: this.currentHighestBid,
            },
        });
    }

    ngOnDestroy() {
        this.signalRService.leaveLotGroup(this.lotId);
    }
}
