import { Dialog } from '@angular/cdk/dialog';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

import { AuctionModel, Client, LotModel } from 'src/app/web-api-client';
import { RemoveFromWatchlistComponent } from '../../general/remove-from-watchlist/remove-from-watchlist.component';

@Component({
    selector: 'app-dashboard',
    templateUrl: './dashboard.component.html',
    styleUrls: ['./dashboard.component.scss'],
})
export class DashboardComponent implements OnInit {
    lots: LotModel[] = [];
    auctions: AuctionModel[] = [];
    constructor(
        private apiClient: Client,
        private snackBar: MatSnackBar,
        private dialog: Dialog
    ) {}

    ngOnInit(): void {
        this.loadBuyerAuctions();
        this.loadLotsInWatchlist();
    }

    loadBuyerAuctions() {
        this.apiClient.getBuyerAuctions(0, 100).subscribe((auctions) => {
            this.auctions = auctions;
        });
    }

    loadLotsInWatchlist() {
        this.apiClient.getLotsInWatchlist(0, 5).subscribe((lots) => {
            this.lots = lots;
        });
    }

    calculateDaysLeftActive(
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

    calculateDaysLeftUpcoming(startDate: Date | null): number | null {
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

    handleLotWatchlist(lot: LotModel) {
        if (!lot.isInWatchlist) {
            this.apiClient.addToWatchlist(lot.id).subscribe({
                next: (result) => {
                    this.snackBar.open(
                        'Successfully added the lot to watchlist',
                        'Close',
                        {
                            horizontalPosition: 'center',
                            verticalPosition: 'bottom',
                            duration: 5000,
                            panelClass: ['success-snackbar'],
                        }
                    );
                    lot.isInWatchlist = true;
                },
                error: (result: HttpErrorResponse) => {
                    this.snackBar.open(
                        result.error.errors[0].ErrorMessage,
                        'Close',
                        {
                            horizontalPosition: 'center',
                            verticalPosition: 'bottom',
                            duration: 5000,
                            panelClass: ['error-snackbar'],
                        }
                    );
                },
            });
        } else {
            const dialog = this.dialog.open(RemoveFromWatchlistComponent, {
                data: {
                    lotId: lot.id,
                },
            });

            dialog.closed.subscribe({
                next: () => {
                    this.loadLotsInWatchlist();
                },
            });
        }
    }
}
