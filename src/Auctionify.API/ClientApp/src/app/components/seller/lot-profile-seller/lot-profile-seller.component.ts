import { Component, Injectable } from '@angular/core';
import { formatDate } from '@angular/common';
import { Router } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';

import { Observable } from 'rxjs';
import { SellerGetLotResponse, Client } from 'src/app/web-api-client';
import { ChoicePopupComponent } from 'src/app/ui-elements/choice-popup/choice-popup.component';
import { DialogPopupComponent } from 'src/app/ui-elements/dialog-popup/dialog-popup.component';

@Injectable({
  providedIn: 'root',
})

@Component({
  selector: 'app-lot-profile-seller',
  templateUrl: './lot-profile-seller.component.html',
  styleUrls: ['./lot-profile-seller.component.scss']
})
export class LotProfileSellerComponent {
  lotData$!: Observable<SellerGetLotResponse>;
  showAllBids = false;
  selectedMainPhotoIndex: number = 0;

  constructor(
    private apiService: Client, 
    private router: Router,
    private route: ActivatedRoute,
    private dialog: MatDialog
    ) {}

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      const lotId = + params['id'];
      this.lotData$ = this.apiService.getOneLotForSeller(lotId);
    });
  }

  getHighestBidPrice(lotData: SellerGetLotResponse | null): number | null {
    if (lotData && lotData.bids && lotData.bids.length > 0) {
      return Math.max(...lotData.bids.map(bid => bid.newPrice));
    } else {
      return lotData ? lotData.startingPrice : null;
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

  editLot(lotId: number) {
    this.router.navigate(['seller/update-lot', lotId]);
  }

  removeLot(lotId: number): void {
    if (lotId) {
      const dialogRef = this.dialog.open(ChoicePopupComponent, {
        data: {
          isError: true,
          text: ['Are you sure you want to remove this lot?'],
          continueBtnText: 'Remove',
          breakBtnText: 'Cancel',
          additionalText: 'This action cannot be undone.',
          continueBtnColor: 'warn',
          breakBtnColor: 'primary',
        },
      });
  
      dialogRef.afterClosed().subscribe((result) => {
        if (result === 'true') {
          this.apiService.deleteLot(lotId).subscribe(
            () => {
              this.router.navigate(['/dashboard']);
            },
            (error) => {
              this.dialog.open(DialogPopupComponent, {
                data: {
                  isError: true,
                  isErrorShown: true,
                  text: [
                    'Oops, something went wrong.', 
                    'Try to refresh this page or come back later.'
                ],
                },
              });
            }
          );
        } else {

        }
      });
    } else {
      this.dialog.open(DialogPopupComponent, {
        data: {
          isError: true,
          isErrorShown: true,
          text: ['LotId is not available.'],
        },
      });
    }
  }

  downloadDocument(documentUrl: string): void {
    this.apiService.downloadDocument(documentUrl).subscribe((data: any) => {
      const blob = new Blob([data], { type: 'application/octet-stream' });
  
      const link = document.createElement('a');
      link.href = window.URL.createObjectURL(blob);
      const fileName = documentUrl.substring(documentUrl.lastIndexOf('/') + 1);
      link.download = fileName;
  
      link.click();
    });
  }
}
