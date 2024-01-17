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
import { DateCalculationService } from 'src/app/services/date-calculation/date-calculation.service';
import { Rate, RatePaginationModel } from 'src/app/models/rates/rate-models';
import { DialogPopupComponent } from 'src/app/ui-elements/dialog-popup/dialog-popup.component';
import { Client } from 'src/app/web-api-client';
import { BuyerModel } from 'src/app/models/users/user-models';

@Component({
    selector: 'app-dashboard',
    templateUrl: './dashboard.component.html',
    styleUrls: ['./dashboard.component.scss'],
})
export class DashboardComponent implements OnInit {
    private readonly MAIN_BLUE_COLOR: string = '#2b5293';
    private readonly MAIN_RED_COLOR: string = '#ab2d25';
    private readonly MAIN_GREEN_COLOR: string = '#2b9355';
    userProfileData: BuyerModel | null = null;
    senderRates: Rate[] = [];

    priceColor: string = 'black';

    currentBuyerId: number = 0;

    lots: LotModel[] = [];
    auctions: AuctionModel[] = [];
    generalAuctionResponse!: GeneralAuctionResponse;

    inititalAuctionsCount: number = 5;
    additionalAuctionsCount: number = 5;
    noMoreAuctionsToLoad: boolean = false;
    currentIndex: number = 0;

    constructor(
        private apiClient: Client,
        private snackBar: MatSnackBar,
        private dialog: Dialog,
        private authService: AuthorizeService,
        private dateCalculationService: DateCalculationService
    ) {
        effect(() => {
            this.currentBuyerId = this.authService.getUserId()!;
        });
    }

    ngOnInit(): void {
        this.loadBuyerAuctions();
        this.loadLotsInWatchlist();
        this.fetchUserProfileData();
        this.fetchRatesData();
    }

    private fetchUserProfileData() {
        this.client.getBuyer().subscribe(
            (data: BuyerModel) => {
                this.userProfileData = data;
                this.validate();
            },
            (error) => {
                this.openDialog(
                    error.errors! || [
                        'Something went wrong, please try again later',
                    ],
                    true
                );
        }
        );
        }

    private fetchRatesData() {
        const pagination: RatePaginationModel = {
            pageIndex: 0,
            pageSize: 2,
        };

        this.client.getRates(pagination).subscribe(
            (userRate) => {
                this.senderRates = userRate.items;
            },
            (error) => {
                this.openDialog(
                    error.errors! || [
                        'Something went wrong, please try again later',
                    ],
                    true
                );
                }
        );
    }

    private validate() {
        if (!this.userProfileData?.averageRate) {
            this.userProfileData!.averageRate = 0;
        } else if (!this.userProfileData?.ratesCount) {
            this.userProfileData!.ratesCount = 0;
        }
    }

    openDialog(text: string[], error: boolean) {
        const dialogRef = this.dialog.open<string>(DialogPopupComponent, {
            data: {
                text,
                isError: error,
            },
            });

        dialogRef.closed.subscribe((res) => {});
    }

    getAverageStars(rate: number | null): string[] {
        const averageRating = rate;

        const roundedAverage = Math.round(averageRating!);

        const stars: string[] = [];
        for (let i = 1; i <= 5; i++) {
            if (i <= roundedAverage) {
                stars.push('star');
            } else if (i - roundedAverage === 0.5) {
                stars.push('star_half');
            } else {
                stars.push('star_border');
                        }
                        }

        return stars;
    }

    getPercentage(count: number): string {
        const total = this.getTotalCount();
        return total > 0 ? `${(count / total) * 100}%` : '0%';
        }

    getTotalCount(): number {
        return this.senderRates.length;
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
                this.priceColor = this.MAIN_BLUE_COLOR;
            } else {
                this.priceColor = this.MAIN_RED_COLOR;
            }
        } else {
            if (auction.buyerId === this.currentBuyerId) {
                this.priceColor = this.MAIN_GREEN_COLOR;
            } else {
                this.priceColor = this.MAIN_RED_COLOR;
            }
        }
    }
}
