import { Component, OnInit } from '@angular/core';
import { Dialog } from '@angular/cdk/dialog';
import { Router } from '@angular/router';

import { Observable, mergeMap, of } from 'rxjs';

import { Client, LotModel } from 'src/app/web-api-client';
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

    private filterData: FilterResult | null = null;

    constructor(
        private apiClient: Client,
        private dialog: Dialog,
        private router: Router,
        private authService: AuthorizeService
    ) {}

    ngOnInit(): void {
        this.isUserSeller = this.authService.isUserSeller();
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
            pageIndex: 0,
            pageSize: this.initialActiveLotsCount,
        };

        this.apiClient.filterLots(filterLot).subscribe((activeLots) => {
            if (activeLots.length < this.initialActiveLotsCount) {
                this.noMoreActiveLotsToLoad = true;
            }
            this.activeLots$ = of(activeLots);
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
            pageIndex: 0,
            pageSize: this.initialUpcomingLotsCount,
        };

        this.apiClient.filterLots(filterLot).subscribe((upcomingLots) => {
            if (upcomingLots.length < this.initialUpcomingLotsCount) {
                this.noMoreUpcomingLotsToLoad = true;
            }
            this.upcomingLots$ = of(upcomingLots);
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
            pageIndex: 0,
            pageSize: this.initialArchivedLotsCount,
        };

        this.apiClient.filterLots(filterLot).subscribe((archivedLots) => {
            if (archivedLots.length < this.initialArchivedLotsCount) {
                this.noMoreArchivedLotsToLoad = true;
            }
            this.archivedLots$ = of(archivedLots);
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
        this.initialArchivedLotsCount += this.additionalArchivedLotsCount;
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

                const filterData: FilterLot = {
                    ...data,
                    sortDir: null,
                    sortField: null,
                    pageIndex: 0,
                    pageSize: 20,
                };

                this.mapFilter(filterData).subscribe({
                    next: (res) => {
                        const archivedLotsIds = [];

                        for (const [statusName, statusId] of res) {
                            if (statusName === 'Active') {
                                const activeFilterData = {
                                    ...filterData,
                                };
                                activeFilterData.lotStatuses = [statusId];

                                const subscriber = this.apiClient
                                    .filterLots(activeFilterData)
                                    .subscribe((activeLots) => {
                                        this.activeLots$ = of(activeLots);

                                        subscriber.unsubscribe();
                                    });
                            } else if (statusName === 'Upcoming') {
                                const upcomingFilterData = {
                                    ...filterData,
                                };
                                upcomingFilterData.lotStatuses = [statusId];

                                const subscriber = this.apiClient
                                    .filterLots(upcomingFilterData)
                                    .subscribe((upcomingLots) => {
                                        this.upcomingLots$ = of(upcomingLots);

                                        subscriber.unsubscribe();
                                    });
                            } else if (
                                statusName === 'Sold' ||
                                statusName === 'NotSold' ||
                                statusName === 'Cancelled'
                            ) {
                                archivedLotsIds.push(statusId);
                            }
                        }

                        if (archivedLotsIds.length > 0) {
                            const archivedFilterData = {
                                ...filterData,
                            };
                            archivedFilterData.lotStatuses = archivedLotsIds;

                            const subscriber = this.apiClient
                                .filterLots(archivedFilterData)
                                .subscribe((archivedLots) => {
                                    this.archivedLots$ = of(archivedLots);

                                    subscriber.unsubscribe();
                                });
                        }
                    },
                });
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
}
