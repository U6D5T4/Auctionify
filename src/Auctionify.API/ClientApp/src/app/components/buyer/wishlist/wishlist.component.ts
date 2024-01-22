import { Dialog } from '@angular/cdk/dialog';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

import { Client, LotModel } from 'src/app/web-api-client';
import { RemoveFromWatchlistComponent } from '../../general/remove-from-watchlist/remove-from-watchlist.component';
import { DateCalculationService } from 'src/app/services/date-calculation/date-calculation.service';

@Component({
    selector: 'app-wishlist',
    templateUrl: './wishlist.component.html',
    styleUrls: ['./wishlist.component.scss'],
})
export class WishlistComponent implements OnInit {
    lots: LotModel[] = [];
    constructor(
        private apiClient: Client,
        private snackBar: MatSnackBar,
        private dialog: Dialog,
        private dateCalculationService: DateCalculationService
    ) {}

    ngOnInit(): void {
        this.loadLotsInWatchlist();
    }

    loadLotsInWatchlist() {
        this.apiClient.getLotsInWatchlist(0, 100).subscribe((lots) => {
            this.lots = lots;
        });
    }

    calculateDaysLeftActive(
        startDate: Date | null,
        endDate: Date | null
    ): number | null {
        return this.dateCalculationService.calculateDaysLeftActive(
            startDate,
            endDate
        );
    }

    calculateDaysLeftUpcoming(startDate: Date | null): number | null {
        return this.dateCalculationService.calculateDaysLeftUpcoming(startDate);
    }

    handleLotWatchlist(lot: LotModel) {
        if (!lot.isInWatchlist) {
            this.apiClient.addToWatchlist(lot.id).subscribe({
                next: (result) => {
                    this.snackBar.open(
                        'Successfully added the lot to wishlist',
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
                autoFocus: false,
            });

            dialog.closed.subscribe({
                next: () => {
                    this.loadLotsInWatchlist();
                },
            });
        }
    }
}
