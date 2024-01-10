import { Dialog } from '@angular/cdk/dialog';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit, effect } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

import {
    AuctionModel,
    Client,
    GeneralAuctionResponse,
    LotModel,
} from 'src/app/web-api-client';
import { RemoveFromWatchlistComponent } from '../../general/remove-from-watchlist/remove-from-watchlist.component';
import { AuthorizeService } from 'src/app/api-authorization/authorize.service';

@Component({
    selector: 'app-dashboard',
    templateUrl: './dashboard.component.html',
    styleUrls: ['./dashboard.component.scss'],
})
export class DashboardComponent implements OnInit {
    currentBuyerId: number = 0;

    lots: LotModel[] = [];
    auctions: AuctionModel[] = [];
    generalAuctionResponse!: GeneralAuctionResponse;

    inititalAuctionsCount: number = 5;
    additionalAuctionsCount: number = 5;
    noMoreAuctionsToLoad: boolean = false;
    currentIndex: number = 0;

    priceColor: string = 'black';

    constructor(
        private apiClient: Client,
        private snackBar: MatSnackBar,
        private dialog: Dialog,
        private authService: AuthorizeService
    ) {
        effect(() => {
            this.currentBuyerId = this.authService.getUserId()!;
        });
    }

    ngOnInit(): void {
        this.loadBuyerAuctions();
        this.loadLotsInWatchlist();
    }

    getRecentUserBidForLot(lot: AuctionModel): number | null {
        if (!lot.bids) {
            return null;
        }

        const userBid = lot.bids.find(
            (bid) => bid.buyerId === this.currentBuyerId
        );

        if (!userBid) {
            return null;
        }

        return userBid.newPrice;
    }

    loadBuyerAuctions() {
        this.apiClient
            .getBuyerAuctions(0, this.inititalAuctionsCount)
            .subscribe((response: GeneralAuctionResponse) => {
                this.generalAuctionResponse = response;
                this.auctions = response.items;
                console.log(this.auctions);

                if (!this.generalAuctionResponse.hasNext) {
                    this.noMoreAuctionsToLoad = true;
                }
            });
    }

    loadMoreBuyerAuctions() {
        if (!this.generalAuctionResponse.hasNext) {
            this.noMoreAuctionsToLoad = true;
            return;
        }

        this.currentIndex += 1;

        this.apiClient
            .getBuyerAuctions(this.currentIndex, this.additionalAuctionsCount)
            .subscribe((response: GeneralAuctionResponse) => {
                this.auctions = [...this.auctions, ...response.items];
                this.generalAuctionResponse = response;
                console.log(response);

                if (!this.generalAuctionResponse.hasNext) {
                    this.noMoreAuctionsToLoad = true;
                }
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
                    this.loadLotsInWatchlist();
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
                    const updatedAuction = this.auctions.find(
                        (auction) => auction.id === lot.id
                    );
                    if (updatedAuction) {
                        updatedAuction.isInWatchlist = false;
                        this.loadLotsInWatchlist();
                    }
                    this.loadLotsInWatchlist();
                },
            });
        }
    }

    determinePriceColor(auction: any) {
        if (auction.lotStatus.name === 'Active') {
            if (this.currentBuyerId === auction.bids[0].buyerId) {
                this.priceColor = '#2b5293';
            } else {
                this.priceColor = '#ab2d25';
            }
        } else {
            if (auction.buyerId === this.currentBuyerId) {
                this.priceColor = '#2b9355';
            } else {
                this.priceColor = '#ab2d25';
            }
        }
    }
}
