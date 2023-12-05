import { Component, OnInit } from '@angular/core';
import { Dialog } from '@angular/cdk/dialog';
import { Router } from '@angular/router';

import { Observable, of } from 'rxjs';

import { Client, LotModel } from 'src/app/web-api-client';
import { FilterLot } from 'src/app/models/lots/filter';
import { FilterComponent, FilterResult } from '../filter/filter.component';


@Component({
    selector: 'app-auction',
    templateUrl: './auction.component.html',
    styleUrls: ['./auction.component.scss'],
})
export class AuctionComponent implements OnInit {
    links: any[] = Array(8).fill({
        imgUrl: 'assets/icons/StarIcon.svg',
        linkText: 'Example',
    });

    initialActiveLotsCount: number = 10;
    additionalActiveLotsCount: number = 10;
    initialUpcomingLotsCount: number = 5;
    additionalUpcomingLotsCount: number = 10;
    initialArchivedLotsCount: number = 5;
    additionalArchivedLotsCount: number = 10;

    activeLots$!: Observable<LotModel[]>;
    upcomingLots$!: Observable<LotModel[]>;
    archivedLots$!: Observable<LotModel[]>;

    constructor(
        private apiClient: Client, 
        private dialog: Dialog, 
        private router: Router
        ) {}

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
            pageIndex: 0,
            pageSize: this.initialActiveLotsCount,
        };

        this.apiClient.filterLots(filterLot).subscribe((activeLots) => {
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
            this.archivedLots$ = of(archivedLots)
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
        const dialogSubscriber = this.dialog.open(FilterComponent);

        dialogSubscriber.closed.subscribe((res: any) => {
            if (res) {
                const data = JSON.parse(res) as FilterResult;

                const filterData: FilterLot = {
                    ...data,
                    sortDir: null,
                    sortField: null,  
                    pageIndex: 0,
                    pageSize: 20               
                };

                this.apiClient.filterLots(filterData).subscribe((res) => {
                    console.log(res);
                    this.activeLots$ = of(res);
                });
            }
        });
    }
}
