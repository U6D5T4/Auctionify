import { HttpErrorResponse } from '@angular/common/http';
import { Component, Input } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Client, LotModel } from 'src/app/web-api-client';

@Component({
    selector: 'app-lot-item',
    templateUrl: './lot-item.component.html',
    styleUrls: ['./lot-item.component.scss'],
})
export class LotItemComponent {
    @Input()
    lot!: LotModel;

    @Input()
    isUserBuyer!: boolean;

    constructor(private apiClient: Client, private snackBar: MatSnackBar) {}

    handleLotWatchlist(lotData: LotModel) {
        if (!lotData.isInWatchlist) {
            this.apiClient.addToWatchlist(lotData.id).subscribe({
                next: (result) => {
                    this.snackBar.open(result, 'Ok', {
                        horizontalPosition: 'right',
                        verticalPosition: 'top',
                        duration: 5000,
                    });
                    lotData.isInWatchlist = true;
                },
                error: (result: HttpErrorResponse) => {
                    this.snackBar.open(
                        result.error.errors[0].ErrorMessage,
                        'Ok',
                        {
                            horizontalPosition: 'right',
                            verticalPosition: 'top',
                            duration: 5000,
                        }
                    );
                },
            });
        } else {
            this.apiClient.removeFromWatchList(lotData.id).subscribe({
                next: (result) => {
                    this.snackBar.open(result, 'Ok', {
                        horizontalPosition: 'right',
                        verticalPosition: 'top',
                        duration: 5000,
                    });
                    lotData.isInWatchlist = false;
                },
                error: (result: HttpErrorResponse) => {
                    this.snackBar.open(
                        result.error.errors[0].ErrorMessage,
                        'Ok',
                        {
                            horizontalPosition: 'right',
                            verticalPosition: 'top',
                            duration: 5000,
                        }
                    );
                },
            });
        }
    }

    calculateDaysLeft(
        startDate: Date | null,
        endDate: Date | null
    ): number | null {
        if (!startDate || !endDate) {
            return null;
        }

        if (this.lot.lotStatus.name == 'Active') {
            const start = new Date(startDate);
            const end = new Date(endDate);
            const timeDifference = end.getTime() - start.getTime();
            return Math.ceil(timeDifference / (1000 * 3600 * 24));
        } else {
            const now = new Date();
            const start = new Date(startDate);
            if (start <= now) {
                return 0;
            }

            const timeDifference = start.getTime() - now.getTime();
            return Math.ceil(timeDifference / (1000 * 3600 * 24));
        }
    }
}
