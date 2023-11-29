import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute } from '@angular/router';
import { Observable, forkJoin, of, switchMap } from 'rxjs';

import { SignalRService } from 'src/app/services/signalr.service';
import { BidDto, BuyerGetLotResponse, Client } from 'src/app/web-api-client';
import { AddBidComponent } from '../add-bid/add-bid.component';

@Component({
    selector: 'app-get-lot-buyer',
    templateUrl: './get-lot.component.html',
    styleUrls: ['./get-lot.component.scss'],
})
export class GetLotComponent implements OnInit {
    lotId: number = 0;
    lot$!: Observable<BuyerGetLotResponse>;
    bids$!: Observable<BidDto[]>;

    constructor(
        private apiClient: Client,
        private route: ActivatedRoute,
        private signalRService: SignalRService,
        private dialog: MatDialog
    ) {}

    ngOnInit(): void {
        this.getLotFromRoute();

        this.signalRService.onReceiveBidNotification(() => {
            this.getLotFromRoute();
        });

        this.signalRService.onReceiveWithdrawBidNotification(() => {
            this.getLotFromRoute();
        });
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
            });
    }

    openBidModal(): void {
        const dialogRef = this.dialog.open(AddBidComponent, {
            width: '500px',
            height: '300px',
            data: { lotId: this.lotId },
        });

        dialogRef.afterClosed().subscribe((result) => {
            console.log('The dialog was closed');
        });
    }
}
