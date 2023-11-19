import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { Client, FilteredLotModel } from 'src/app/web-api-client';

@Component({
  selector: 'app-auction',
  templateUrl: './auction.component.html',
  styleUrls: ['./auction.component.scss']
})
export class AuctionComponent implements OnInit{
  links: any[] = Array(8).fill({ imgUrl: 'assets/icons/StarIcon.svg', linkText: 'Example' });

  activeLots$!: Observable<FilteredLotModel[]>;
  upcomingLots$!: Observable<FilteredLotModel[]>;
  archivedLots$!: Observable<FilteredLotModel[]>;

  constructor(private apiClient: Client) {}

  ngOnInit(): void {
    this.getAllLots();
  }

  getAllLots(): void {
    const activeFilterParams = {
      minimumPrice: null,
      maximumPrice: null,
      categoryId: null,
      location: null,
      lotStatuses: [1],
      sortDir: null,
      sortField: null
    };

    const upcomingFilterParams = {
      minimumPrice: null,
      maximumPrice: null,
      categoryId: null,
      location: null,
      lotStatuses: [2],
      sortDir: null,
      sortField: null
    };

    const archivedFilterParams = {
      minimumPrice: null,
      maximumPrice: null,
      categoryId: null,
      location: null,
      lotStatuses: [3],
      sortDir: null,
      sortField: null
    };

    this.activeLots$ = this.apiClient.filterLots(activeFilterParams);
    this.upcomingLots$ = this.apiClient.filterLots(upcomingFilterParams);
    this.archivedLots$ = this.apiClient.filterLots(archivedFilterParams);
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
