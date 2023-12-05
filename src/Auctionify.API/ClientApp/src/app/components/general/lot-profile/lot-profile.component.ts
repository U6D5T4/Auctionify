import { formatDate } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Observable, switchMap, of } from 'rxjs';
import { BuyerGetLotResponse, Client } from 'src/app/web-api-client';

@Component({
    selector: 'app-lot-profile',
    templateUrl: './lot-profile.component.html',
    styleUrls: ['./lot-profile.component.scss'],
})
export class LotProfileComponent implements OnInit {
    lotData$!: Observable<BuyerGetLotResponse>;
    lotId: number = 0;
    showAllBids = false;
    selectedMainPhotoIndex: number = 0;

    constructor(private client: Client, private route: ActivatedRoute) {}

    ngOnInit(): void {
        this.getLotFromRoute();
    }

    getLotFromRoute(): void {
        this.route.paramMap
            .pipe(
                switchMap((params) => {
                    this.lotId = Number(params.get('id')) || 0;
                    return this.client.getOneLotForBuyer(this.lotId);
                })
            )
            .subscribe((lot) => {
                this.lotData$ = of(lot);
            });
    }

    getHighestBidPrice(lotData: BuyerGetLotResponse | null): number | null {
        if (lotData && lotData.bids && lotData.bids.length > 0) {
            return Math.max(...lotData.bids.map((bid) => bid.newPrice));
        } else {
            return lotData ? lotData.startingPrice : null;
        }
    }

    setMainPhoto(index: number): void {
        this.selectedMainPhotoIndex = index;
    }

    formatBidDate(date: Date): string {
        return formatDate(date, 'd MMMM HH:mm', 'en-US');
    }

    formatStartDate(date: Date | null): string {
        return date ? formatDate(date, 'd/MM/yy', 'en-US') : '';
    }

    formatEndDate(date: Date | null): string {
        return date ? formatDate(date, 'd/MM/yy', 'en-US') : '';
    }

    addLotToWatchlist() {
        this.client.addToWatchlist(this.lotId);
    }

    downloadDocument(documentUrl: string): void {
        this.client.downloadDocument(documentUrl).subscribe((data: any) => {
            const blob = new Blob([data], { type: 'application/octet-stream' });

            const link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            const fileName = documentUrl.substring(
                documentUrl.lastIndexOf('/') + 1
            );
            link.download = fileName;

            link.click();
        });
    }
}
