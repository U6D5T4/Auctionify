import { Dialog, DialogRef } from '@angular/cdk/dialog';
import { formatDate } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { DomSanitizer } from '@angular/platform-browser';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { AuthorizeService } from 'src/app/api-authorization/authorize.service';
import { FileModel } from 'src/app/models/fileModel';
import { SignalRService } from 'src/app/services/signalr-service/signalr.service';
import {
    ChoicePopupComponent,
    ChoicePopupData,
} from 'src/app/ui-elements/choice-popup/choice-popup.component';
import {
    BidDto,
    BuyerGetLotResponse,
    Client,
    SellerGetLotResponse,
} from 'src/app/web-api-client';
import { ImagePopupComponent } from '../image-popup/image-popup.component';
import { AddBidComponent } from '../add-bid/add-bid.component';
import { WithdrawBidComponent } from '../withdraw-bid/withdraw-bid.component';

@Component({
    selector: 'app-lot-profile',
    templateUrl: './lot-profile.component.html',
    styleUrls: ['./lot-profile.component.scss'],
})
export class LotProfileComponent implements OnInit {
    lotData: BuyerGetLotResponse | SellerGetLotResponse | null = null;
    bidsToShow: BidDto[] = [];
    files: FileModel[] | null = null;
    lotId: number = 0;
    showAllBids = false;
    selectedMainPhotoIndex: number = 0;
    parentCategoryName: string = '';
    isDeleteLoading = false;
    recentBidOfCurrentBuyer: number = 0;

    private isSignalrConnected = false;

    constructor(
        private client: Client,
        private route: ActivatedRoute,
        private router: Router,
        private authService: AuthorizeService,
        private signalRService: SignalRService,
        private dialog: Dialog,
        private snackBar: MatSnackBar,
        private sanitizer: DomSanitizer
    ) {}

    ngOnInit() {
        this.getLotFromRoute();
    }

    getLotFromRoute() {
        this.route.paramMap.subscribe(async (params: ParamMap) => {
            this.lotId = Number(params.get('id'));
            this.bidsToShow = [];

            if (!this.isSignalrConnected) {
                await this.signalRService.joinLotGroupAfterConnection(
                    this.lotId
                );

                this.signalRService.onReceiveBidNotification(() => {
                    this.getLotFromRoute();
                }, this.lotId);

                this.signalRService.onReceiveWithdrawBidNotification(() => {
                    this.getLotFromRoute();
                }, this.lotId);
            }

            if (this.authService.isUserBuyer()) {
                this.client.getOneLotForBuyer(this.lotId).subscribe({
                    next: this.handleLotResponse.bind(this),
                    error: this.handleLotError.bind(this),
                });
            } else {
                this.client.getOneLotForSeller(this.lotId).subscribe({
                    next: this.handleLotResponse.bind(this),
                    error: this.handleLotError.bind(this),
                });
            }
        });
    }

    handleLotResponse(result: BuyerGetLotResponse | SellerGetLotResponse) {
        this.lotData = result;
        if (this.lotData.additionalDocumentsUrl) {
            this.files = this.lotData.additionalDocumentsUrl.map(
                (url: string): FileModel => {
                    const file: FileModel = {
                        name: this.getFileNameFromUrl(url),
                        fileUrl: url,
                        file: null,
                    };

                    return file;
                },
                this
            );
        }

        if (this.lotData.category.parentCategoryId) {
            this.client.getAllCategories().subscribe({
                next: (result) => {
                    const parentCategory = result.find(
                        (x) => x.id === this.lotData?.category.parentCategoryId
                    );

                    if (!parentCategory) return;
                    this.parentCategoryName = parentCategory?.name;
                },
            });
        }

        if (this.lotData.bids.length > 0) {
            if (this.showAllBids) {
                this.bidsToShow = this.lotData.bids;
            } else {
                this.bidsToShow = this.lotData.bids.slice(0, 3);
            }
        }
    }

    handleLotError() {
        this.router.navigate(['/home']);
    }

    getHighestBidPrice(
        lotData: BuyerGetLotResponse | SellerGetLotResponse | null
    ): number {
        if (lotData && lotData.bids && lotData.bids.length > 0) {
            return Math.max(...lotData.bids.map((bid) => bid.newPrice));
        } else {
            return lotData?.startingPrice!;
        }
    }

    getBidsCountString() {
        return `Show all ${this.lotData?.bidCount} bids`;
    }

    showBidsClick() {
        if (!this.showAllBids) {
            this.bidsToShow = this.lotData?.bids!;
            this.showAllBids = true;
        } else {
            this.bidsToShow = this.lotData?.bids!.slice(0, 3)!;
            this.showAllBids = false;
        }
    }

    setMainPhoto(index: number): void {
        this.selectedMainPhotoIndex = index;
    }

    openMainPhoto(imageUrl: string) {
        this.dialog.open<string>(ImagePopupComponent, {
            autoFocus: true,
            data: imageUrl,
        });
    }

    openBidModal(): void {
        const dialog = this.dialog.open(AddBidComponent, {
            data: {
                lotId: this.lotId,
                bidCount: this.lotData!.bidCount,
                currency: this.lotData!.currency,
                startingPrice: this.lotData!.startingPrice,
                currentHighestBid: this.getHighestBidPrice(this.lotData),
            },
        });
    }

    openWithdrawBidModal(): void {
        this.getBuyerRecentBidForLot().then((bidId) => {
            const dialog = this.dialog.open(WithdrawBidComponent, {
                data: {
                    bidId: bidId,
                },
            });
        });
    }

    getBuyerRecentBidForLot(): Promise<number> {
        return new Promise<number>((resolve, reject) => {
            this.client.getAllBidsOfUserForLot(this.lotId, 0, 1).subscribe({
                next: (bids) => {
                    if (bids.length > 0) {
                        this.recentBidOfCurrentBuyer = bids[0].newPrice;
                        resolve(bids[0].id);
                    } else {
                        resolve(0);
                    }
                },
                error: (error) => {
                    console.error('Failed to fetch bids:', error);
                    reject(error);
                },
            });
        });
    }

    formatBidDate(date: Date): string {
        return formatDate(date, 'd MMMM HH:mm', 'en-US');
    }

    formatStartDate(date: Date | null): string {
        return date ? formatDate(date, 'dd LLLL, h:mm (z)', 'en-US') : '';
    }

    formatEndDate(date: Date | null): string {
        return date ? formatDate(date, 'dd LLLL, h:mm (z)', 'en-US') : '';
    }

    handleLotWatchlist() {
        const lotData = this.lotData as BuyerGetLotResponse;
        if (!lotData.isInWatchlist) {
            this.client.addToWatchlist(this.lotId).subscribe({
                next: (result) => {
                    this.snackBar.open(result, 'Ok', {
                        horizontalPosition: 'right',
                        verticalPosition: 'top',
                        duration: 5000,
                    });
                    this.getLotFromRoute();
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
            this.client.removeFromWatchList(this.lotId).subscribe({
                next: (result) => {
                    this.snackBar.open(result, 'Ok', {
                        horizontalPosition: 'right',
                        verticalPosition: 'top',
                        duration: 5000,
                    });
                    this.getLotFromRoute();
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

    downloadDocument(documentUrl: string): void {
        this.client.downloadDocument(documentUrl).subscribe((data: any) => {
            const blob = new Blob([data], { type: 'application/octet-stream' });

            const link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            const fileName = this.getFileNameFromUrl(documentUrl);
            link.download = fileName;

            link.click();
        });
    }

    deleteLot() {
        this.isDeleteLoading = true;
        const msg = ['This action will delete lot or cancel it.'];
        const dialogData: ChoicePopupData = {
            isError: true,
            isErrorShown: true,
            continueBtnText: 'Delete',
            breakBtnText: 'Cancel',
            text: msg,
            additionalText: 'Continue?',
            continueBtnColor: 'warn',
            breakBtnColor: 'primary',
        };
        const openedChoiceDialog = this.openChoiceDialog(dialogData);
        const dialogSubscriber = openedChoiceDialog.closed.subscribe(
            (result) => {
                const isResult = result === 'true';
                if (isResult) {
                    const deleteLotSubscriber = this.client
                        .deleteLot(this.lotId)
                        .subscribe({
                            next: (res) => {
                                this.snackBar.open(res, 'Ok', {
                                    duration: 10000,
                                });
                                this.router.navigate(['/home']);
                                deleteLotSubscriber.unsubscribe();
                            },
                            error: (error) => {},
                            complete: () => {
                                this.isDeleteLoading = false;
                            },
                        });
                }

                this.isDeleteLoading = false;
                dialogSubscriber.unsubscribe();
            }
        );
    }

    openChoiceDialog(data: ChoicePopupData): DialogRef<string, unknown> {
        return this.dialog.open<string>(ChoicePopupComponent, {
            data,
        });
    }

    isResponseForBuyer(response: any): response is BuyerGetLotResponse {
        return 'isInWatchlist' in response;
    }

    getFileNameFromUrl(fileUrl: string): string {
        const regExValue: RegExp = /\/([^\/]+)(?=\/?$)/g;
        const fileName = fileUrl.match(regExValue);
        if (fileName === null) return '';

        return decodeURI(fileName[0].split('/')[1]);
    }

    ngOnDestroy() {
        this.signalRService.leaveLotGroup(this.lotId);
    }
}
