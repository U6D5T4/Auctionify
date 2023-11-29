import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatSliderModule } from '@angular/material/slider';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { ReactiveFormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';

import { BuyerRoutingModule } from './buyer-routing.module';
import { GetLotComponent } from './get-lot/get-lot.component';
import { AddBidComponent } from './add-bid/add-bid.component';
import { MatDialogModule } from '@angular/material/dialog';

@NgModule({
    declarations: [GetLotComponent, AddBidComponent],
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
    exports: [AddBidComponent],
})
export class BuyerModule {}
