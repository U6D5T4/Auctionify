import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LotProfileBuyerComponent } from './lot-profile-buyer/lot-profile-buyer.component';
import { MatIconModule } from '@angular/material/icon';



@NgModule({
  declarations: [
    LotProfileBuyerComponent
  ],
  imports: [
    CommonModule,
    MatIconModule
  ],
  exports: [
    LotProfileBuyerComponent
  ],
})
export class BuyerModule { }
