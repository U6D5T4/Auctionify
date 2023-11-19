import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatSliderModule } from '@angular/material/slider';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { ReactiveFormsModule } from '@angular/forms';

import { FilterComponent } from './filter/filter.component';
import { BuyerRoutingModule } from './buyer-routing.module';
import { AuctionComponent } from './auction/auction.component';



@NgModule({
  declarations: [
    FilterComponent,
    AuctionComponent
  ],
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
        ReactiveFormsModule,
    ],
    exports: [
      FilterComponent,
    AuctionComponent],
})
export class BuyerModule {}
