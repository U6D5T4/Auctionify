import { formatDate } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { AuthorizeService } from 'src/app/api-authorization/authorize.service';
import { FileModel } from 'src/app/models/fileModel';
import { SignalRService } from 'src/app/services/signalr-service/signalr.service';
import {
    BidDto,
    BuyerGetLotResponse,
    Client,
    SellerGetLotResponse,
} from 'src/app/web-api-client';

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

    private isSignalrConnected = false;

    constructor(
        private client: Client,
        private route: ActivatedRoute,
        private router: Router,
        private authService: AuthorizeService,
        private signalRService: SignalRService
    ) {}

    ngOnInit() {
        this.getLotFromRoute();
    }

    getLotFromRoute() {
        this.route.paramMap.subscribe(async (params: ParamMap) => {
            this.lotId = Number(params.get('id'));

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
    ): number | null {
        if (lotData && lotData.bids && lotData.bids.length > 0) {
            return Math.max(...lotData.bids.map((bid) => bid.newPrice));
        } else {
            return lotData ? lotData.startingPrice : null;
        }
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

    formatBidDate(date: Date): string {
        return formatDate(date, 'd MMMM HH:mm', 'en-US');
    }

    formatStartDate(date: Date | null): string {
        return date ? formatDate(date, 'd/MM/yy', 'en-US') : '';
    }

    formatEndDate(date: Date | null): string {
        return date ? formatDate(date, 'd/MM/yy', 'en-US') : '';
    }

    addLotToWatchlist() {
        this.client.addToWatchlist(this.lotId);
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

    isResponseForBuyer(response: any): response is BuyerGetLotResponse {
        return 'isInWatchList' in response;
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
