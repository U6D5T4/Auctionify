import { Component, OnInit, Injectable } from '@angular/core';
import { formatDate } from '@angular/common';
import { ActivatedRoute } from '@angular/router';

import { Observable, of, switchMap } from 'rxjs';
import { BuyerGetLotResponse, Client } from 'src/app/web-api-client';

@Injectable({
  providedIn: 'root',
})

@Component({
  selector: 'app-lot-profile-buyer',
  templateUrl: './lot-profile-buyer.component.html',
  styleUrls: ['./lot-profile-buyer.component.scss']
})
export class LotProfileBuyerComponent implements OnInit {
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
      return Math.max(...lotData.bids.map(bid => bid.newPrice));
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

  addLotToWatchlist(lotId: number){
    this.client.addToWatchlist(lotId)
  }

  downloadDocument(documentUrl: string): void {
    this.client.downloadDocument(documentUrl).subscribe((data: any) => {
      const blob = new Blob([data], { type: 'application/octet-stream' });
  
      const link = document.createElement('a');
      link.href = window.URL.createObjectURL(blob);
      const fileName = documentUrl.substring(documentUrl.lastIndexOf('/') + 1);
      link.download = fileName;
  
      link.click();
    });
  }
}
