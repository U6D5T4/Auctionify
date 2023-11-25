import { Component, OnInit } from '@angular/core';
import { Observable, of } from 'rxjs';
import { Client, LotModel } from 'src/app/web-api-client';
import { FilterLot } from 'src/app/models/lots/filter';

@Component({
  selector: 'app-auction',
  templateUrl: './auction.component.html',
  styleUrls: ['./auction.component.scss']
})
export class AuctionComponent implements OnInit{
  links: any[] = Array(8).fill({ imgUrl: 'assets/icons/StarIcon.svg', linkText: 'Example' });

  initialActiveLotsCount: number = 10;
  additionalActiveLotsCount: number = 10;
  initialUpcomingLotsCount: number = 5;
  additionalUpcomingLotsCount: number = 55;
  initialArchivedLotsCount: number = 5;
  additionalArchivedLotsCount: number = 55;
  
  activeLots$!: Observable<LotModel[]>;
  upcomingLots$!: Observable<LotModel[]>;
  archivedLots$!: Observable<LotModel[]>;

  constructor(private apiClient: Client) {}

  ngOnInit(): void {
    this.loadActiveLots();
    this.loadUpcomingLots();
    this.loadArchivedLots();
  }

  loadActiveLots(): void {
    const filterLot: FilterLot = {
      minimumPrice: null,
      maximumPrice: null,
      categoryId: null,
      location: null,
      lotStatuses: [5],
      sortDir: null,
      sortField: null,
    };

    this.apiClient.filterLots(filterLot).subscribe((activeLots) => {
      this.activeLots$ = of(activeLots.slice(0, this.initialActiveLotsCount));
    });
  }

  loadUpcomingLots(): void {
    const filterLot: FilterLot = {
      minimumPrice: null,
      maximumPrice: null,
      categoryId: null,
      location: null,
      lotStatuses: [4],
      sortDir: null,
      sortField: null,
    };

    this.apiClient.filterLots(filterLot).subscribe((upcomingLots) => {
      this.upcomingLots$ = of(upcomingLots.slice(0, this.initialUpcomingLotsCount));
    });
  }

  loadArchivedLots(): void {
    const filterLot: FilterLot = {
      minimumPrice: null,
      maximumPrice: null,
      categoryId: null,
      location: null,
      lotStatuses: [10],
      sortDir: null,
      sortField: null,
    };

    this.apiClient.filterLots(filterLot).subscribe((archivedLots) => {
      this.archivedLots$ = of(archivedLots.slice(0, this.initialArchivedLotsCount));
    });
  }

  loadMoreUpcomingLots(): void {
    this.initialUpcomingLotsCount += this.additionalUpcomingLotsCount;
    this.loadUpcomingLots();
  }

  loadMoreArchivedLots(): void {
    this.initialArchivedLotsCount += this.additionalArchivedLotsCount;
    this.loadArchivedLots();
  }

  loadMoreActiveLots(): void {
    this.initialActiveLotsCount += this.additionalActiveLotsCount;
    this.loadActiveLots();
  }

  calculateDaysLeft(startDate: Date | null, endDate: Date | null): number | null {
    if (!startDate || !endDate) {
      return null;
    }

    const start = new Date(startDate);
    const end = new Date(endDate);
    const timeDifference = end.getTime() - start.getTime();
    return Math.ceil(timeDifference / (1000 * 3600 * 24));
  }

  calculateDaysLeftUpcomming(startDate: Date | null): number | null {
    if (!startDate) {
      return null;
    }

    const now = new Date();
    const start = new Date(startDate);
    if (start <= now) {
      return 0;
    }

    const timeDifference = start.getTime() - now.getTime();
    return Math.ceil(timeDifference / (1000 * 3600 * 24));
  }
}
