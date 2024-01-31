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
import { BuyerModel } from 'src/app/models/users/user-models';
import { UserDataValidatorService } from 'src/app/services/user-data-validator/user-data-validator.service';
import { RouterLink } from '@angular/router';

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
    IsBtnVisible: boolean = true;

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
        private dateCalculationService: DateCalculationService,
        private userDataValidator: UserDataValidatorService
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
        this.apiClient.getBuyer().subscribe({
            next: (data: BuyerModel) => {
                this.userProfileData = data;
                this.userDataValidator.validateUserProfileData(data);
            },
            error: (error) => {
                this.openDialog(
                    error.errors || [
                        'Something went wrong, please try again later',
                    ],
                    true
                );
            },
        });
    }

    private fetchRatesData() {
        const pagination: RatePaginationModel = {
            pageIndex: 0,
            pageSize: 2,
        };

        this.apiClient.getRates(pagination).subscribe({
            next: (userRate) => {
                this.senderRates = userRate.items;
            },
            error: (error) => {
                this.openDialog(
                    error.errors || [
                        'Something went wrong, please try again later',
                    ],
                    true
                );
            },
        });
    }

    openDialog(text: string[], error: boolean) {
        const dialogRef = this.dialog.open<string>(DialogPopupComponent, {
            data: {
                text,
                isError: error,
            },
        });

        dialogRef.closed.subscribe(() => {});
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
                autoFocus: false,
            });

            dialog.closed.subscribe({
                next: () => {
                    const updatedAuction = this.auctions.find(
                        (auction) => auction.id === lot.id
                    );
                    if (updatedAuction) {
                        updatedAuction.isInWatchlist = false;
                        this.loadLotsInWatchlist();
                        this.loadBuyerAuctions();
                        this.currentIndex = 0;
                        this.noMoreAuctionsToLoad = false;
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
