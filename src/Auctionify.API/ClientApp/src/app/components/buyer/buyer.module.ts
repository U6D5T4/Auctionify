import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatSliderModule } from '@angular/material/slider';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDialogModule } from '@angular/material/dialog';
import { ReactiveFormsModule } from '@angular/forms';

import { BuyerRoutingModule } from './buyer-routing.module';
import { DashboardComponent } from './dashboard/dashboard.component';
import { WishlistComponent } from './wishlist/wishlist.component';
import { TransactionsComponent } from '../general/transactions/transactions.component';

@NgModule({
    declarations: [DashboardComponent, WishlistComponent],
    imports: [
        CommonModule,
        MatIconModule,
        MatButtonModule,
        BuyerRoutingModule,
        MatFormFieldModule,
        MatSelectModule,
        MatInputModule,
        MatSliderModule,
        MatCheckboxModule,
        MatDialogModule,
        ReactiveFormsModule,
    ],
    exports: [],
})
export class BuyerModule {}
