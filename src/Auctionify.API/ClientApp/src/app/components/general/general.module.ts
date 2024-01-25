import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LotProfileComponent } from './lot-profile/lot-profile.component';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { RouterModule } from '@angular/router';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ImagePopupComponent } from './image-popup/image-popup.component';
import { AddBidComponent } from './add-bid/add-bid.component';
import { ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { AuctionComponent } from './home/auction/auction.component';
import { FilterComponent } from './home/filter/filter.component';
import { MatSelectModule } from '@angular/material/select';
import { MatSliderModule } from '@angular/material/slider';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { WithdrawBidComponent } from './withdraw-bid/withdraw-bid.component';
import { RemoveFromWatchlistComponent } from './remove-from-watchlist/remove-from-watchlist.component';
import { TransactionsComponent } from './transactions/transactions.component';

@NgModule({
    declarations: [
        LotProfileComponent,
        ImagePopupComponent,
        AddBidComponent,
        AuctionComponent,
        FilterComponent,
        WithdrawBidComponent,
        RemoveFromWatchlistComponent,
        TransactionsComponent,
    ],
    imports: [
        CommonModule,
        RouterModule,
        MatButtonModule,
        MatIconModule,
        MatSelectModule,
        MatSliderModule,
        MatProgressSpinnerModule,
        ReactiveFormsModule,
        MatFormFieldModule,
        MatInputModule,
        MatCheckboxModule,
        MatSnackBarModule,
    ],

    exports: [LotProfileComponent, TransactionsComponent],
})
export class GeneralModule {}
