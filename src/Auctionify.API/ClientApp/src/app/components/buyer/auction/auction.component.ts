import { Component, OnInit } from '@angular/core';
import { Observable, BehaviorSubject } from 'rxjs';
import { Client, FilteredLotModel } from 'src/app/web-api-client';

@Component({
  selector: 'app-auction',
  templateUrl: './auction.component.html',
  styleUrls: ['./auction.component.scss']
})
export class AuctionComponent implements OnInit{
  links: any[] = Array(8).fill({ imgUrl: 'assets/icons/StarIcon.svg', linkText: 'Example' });

  activeLots$: Observable<FilteredLotModel[]>;
  upcomingLots$: Observable<FilteredLotModel[]>;
  archivedLots$: Observable<FilteredLotModel[]>;

  private activeLotsSubject = new BehaviorSubject<FilteredLotModel[]>([]);
  private upcomingLotsSubject = new BehaviorSubject<FilteredLotModel[]>([]);
  private archivedLotsSubject = new BehaviorSubject<FilteredLotModel[]>([]);
  private initiallyLoadedLotsCount = 20;

  constructor(private apiClient: Client) {
    this.activeLots$ = this.activeLotsSubject.asObservable();
    this.upcomingLots$ = this.upcomingLotsSubject.asObservable();
    this.archivedLots$ = this.archivedLotsSubject.asObservable();
  }

  ngOnInit(): void {
    this.loadInitialLots();
  }

  loadInitialLots(): void {
    this.loadActiveLots();
    this.loadUpcomingLots(10);
    this.loadArchivedLots(10);
  }

  loadActiveLots(): void {
    const filterParams = {
      minimumPrice: null,
      maximumPrice: null,
      categoryId: null,
      location: null,
      lotStatuses: [1],
      sortDir: null,
      sortField: null
    };

    this.apiClient.filterLots(filterParams).subscribe((lots) => {
      const slicedLots = lots.slice(0, this.initiallyLoadedLotsCount);
      this.activeLotsSubject.next(slicedLots);
    });
  }

  loadUpcomingLots(count: number): void {
    const filterParams = {
      minimumPrice: null,
      maximumPrice: null,
      categoryId: null,
      location: null,
      lotStatuses: [2],
      sortDir: null,
      sortField: null
    };

    this.apiClient.filterLots(filterParams).subscribe((lots) => {
      const slicedLots = lots.slice(0, count);
      this.activeLotsSubject.next(slicedLots);
    });
  }

  loadArchivedLots(count: number): void {
    const filterParams = {
      minimumPrice: null,
      maximumPrice: null,
      categoryId: null,
      location: null,
      lotStatuses: [3],
      sortDir: null,
      sortField: null
    };

    this.apiClient.filterLots(filterParams).subscribe((lots) => {
      const slicedLots = lots.slice(0, count);
      this.activeLotsSubject.next(slicedLots);
    });
  }

  loadMoreActiveLots(): void {
    this.initiallyLoadedLotsCount += 20;
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
}
