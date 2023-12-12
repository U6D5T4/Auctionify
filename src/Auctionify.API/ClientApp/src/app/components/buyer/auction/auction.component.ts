import { Component, OnInit } from '@angular/core';
import { Dialog } from '@angular/cdk/dialog';
import { Router } from '@angular/router';

import { Observable, mergeMap, of } from 'rxjs';

import { Client, LotModel, Status } from 'src/app/web-api-client';
import { FilterLot } from 'src/app/models/lots/filter';
import { FilterComponent, FilterResult } from '../filter/filter.component';
import { AuthorizeService } from 'src/app/api-authorization/authorize.service';

@Component({
    selector: 'app-auction',
    templateUrl: './auction.component.html',
    styleUrls: ['./auction.component.scss'],
})
export class AuctionComponent implements OnInit {
    initialActiveLotsCount: number = 10;
    additionalActiveLotsCount: number = 5;
    initialUpcomingLotsCount: number = 5;
    additionalUpcomingLotsCount: number = 5;
    initialArchivedLotsCount: number = 5;
    additionalArchivedLotsCount: number = 5;

    isUserSeller: boolean = false;

    noMoreActiveLotsToLoad: boolean = false;
    noMoreUpcomingLotsToLoad: boolean = false;
    noMoreArchivedLotsToLoad: boolean = false;

    activeLots$!: Observable<LotModel[]>;
    upcomingLots$!: Observable<LotModel[]>;
    archivedLots$!: Observable<LotModel[]>;

    sortDir: string = 'asc';

    private filterData: FilterResult;
    private lotStatuses: Status[] = [];

    constructor(
        private apiClient: Client,
        private dialog: Dialog,
        private router: Router,
        private authService: AuthorizeService
    ) {
        this.filterData = {
            minimumPrice: null,
            maximumPrice: null,
            categoryId: null,
            lotStatuses: null,
            location: null,
        };
    }

    ngOnInit(): void {
        this.isUserSeller = this.authService.isUserSeller();
        this.apiClient.getAllLotStatuses().subscribe({
            next: (res) => {
                this.lotStatuses = res;
                this.loadActiveLots();
                this.loadUpcomingLots();
                this.loadArchivedLots();
            },
        });
    }

    loadActiveLots(): void {
        const activeStatus = this.lotStatuses.find((x) => x.name === 'Active');

        if (!activeStatus) return;

        const filterLot: FilterLot = {
            ...this.filterData,
            sortDir: this.sortDir,
            sortField: 'EndDate',
            pageIndex: 0,
            pageSize: this.initialActiveLotsCount,
        };

        filterLot.lotStatuses = [activeStatus.id];

        this.apiClient.filterLots(filterLot).subscribe((filterResult) => {
            this.noMoreActiveLotsToLoad = filterResult.hasNext;
            this.activeLots$ = of(filterResult.items);
        });
    }

    loadUpcomingLots(): void {
        const upcomingStatus = this.lotStatuses.find(
            (x) => x.name === 'Upcoming'
        );

        if (!upcomingStatus) return;

        const filterLot: FilterLot = {
            ...this.filterData,
            sortDir: this.sortDir,
            sortField: 'EndDate',
            pageIndex: 0,
            pageSize: this.initialUpcomingLotsCount,
        };

        filterLot.lotStatuses = [upcomingStatus.id];

        this.apiClient.filterLots(filterLot).subscribe((filterResult) => {
            this.noMoreUpcomingLotsToLoad = filterResult.hasNext;
            this.upcomingLots$ = of(filterResult.items);
        });
    }

    loadArchivedLots(): void {
        const cancelledStatuses = this.lotStatuses
            .filter(
                (x) =>
                    x.name === 'Cancelled' ||
                    x.name === 'Sold' ||
                    x.name === 'NotSold'
            )
            .map((x) => x.id);

        if (!cancelledStatuses) return;

        const filterLot: FilterLot = {
            ...this.filterData,
            sortDir: this.sortDir,
            sortField: 'EndDate',
            pageIndex: 0,
            pageSize: this.initialArchivedLotsCount,
        };

        filterLot.lotStatuses = cancelledStatuses;

        this.apiClient.filterLots(filterLot).subscribe((filterResult) => {
            this.noMoreArchivedLotsToLoad = filterResult.hasNext;
            this.archivedLots$ = of(filterResult.items);
        });
    }

    loadMoreActiveLots(): void {
        this.initialActiveLotsCount += this.additionalActiveLotsCount;
        this.loadActiveLots();
    }

    loadMoreUpcomingLots(): void {
        this.initialUpcomingLotsCount += this.additionalUpcomingLotsCount;
        this.loadUpcomingLots();
    }

    loadMoreArchivedLots(): void {
        console.log('HERE');
        this.initialArchivedLotsCount += this.additionalArchivedLotsCount;
        console.log(this.initialArchivedLotsCount);
        console.log(this.additionalArchivedLotsCount);

        this.loadArchivedLots();
    }

    calculateDaysLeft(
        startDate: Date | null,
        endDate: Date | null
    ): number | null {
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

    callFilter() {
        const dialogSubscriber = this.dialog.open(FilterComponent, {
            data: this.filterData,
        });

        dialogSubscriber.closed.subscribe((res: any) => {
            if (res) {
                const data = JSON.parse(res) as FilterResult;
                this.filterData = data;

                this.loadActiveLots();
                this.loadUpcomingLots();
                this.loadArchivedLots();
            }
        });
    }

    mapFilter(filter: FilterLot): Observable<[string, number][]> {
        return this.apiClient.getAllLotStatuses().pipe(
            mergeMap((res): Observable<[string, number][]> => {
                let statuses: [string, number][] = [];

                if (filter.lotStatuses) {
                    for (const status of filter.lotStatuses) {
                        const statusFiltered = res.find((x) => x.id === status);
                        if (statusFiltered === null) continue;
                        statuses.push([statusFiltered?.name!, status]);
                    }
                }

                return of(statuses);
            })
        );
    }

    toggleSort(): void {
        this.sortDir = this.sortDir === 'asc' ? 'desc' : 'asc';

        this.loadActiveLots();
        this.loadUpcomingLots();
        this.loadArchivedLots();
    }
}
