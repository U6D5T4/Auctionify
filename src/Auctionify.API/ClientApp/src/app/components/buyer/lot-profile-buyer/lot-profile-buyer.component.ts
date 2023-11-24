import { Component, OnInit, Input } from '@angular/core';
import { formatDate } from '@angular/common';
import { Observable } from 'rxjs';
import { BidDto, BuyerGetLotResponse, Client } from 'src/app/web-api-client';

@Component({
  selector: 'app-lot-profile-buyer',
  templateUrl: './lot-profile-buyer.component.html',
  styleUrls: ['./lot-profile-buyer.component.scss']
})
export class LotProfileBuyerComponent implements OnInit {
  lotData$!: Observable<BuyerGetLotResponse>;
  showAllBids = false;

  constructor(private apiService: Client) {}

  ngOnInit(): void {
    const lotId = 2;
    this.lotData$ = this.apiService.getOneLotForBuyer(lotId);
  }

  getHighestBidPrice(lotData: BuyerGetLotResponse | null): number | null {
    if (lotData && lotData.bids && lotData.bids.length > 0) {
      return Math.max(...lotData.bids.map(bid => bid.newPrice));
    } else {
      return lotData ? lotData.startingPrice : null;
    }
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
}
