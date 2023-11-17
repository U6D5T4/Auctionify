import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FilterComponent } from './filter/filter.component';
import { MatIconModule } from '@angular/material/icon';
import { BuyerRoutingModule } from './buyer-routing.module';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatSliderModule } from '@angular/material/slider';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { ReactiveFormsModule } from '@angular/forms';



@NgModule({
  declarations: [
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
    exports: [FilterComponent],
})
export class BuyerModule {}
