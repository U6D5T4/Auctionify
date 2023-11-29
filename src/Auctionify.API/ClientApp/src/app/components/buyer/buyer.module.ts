import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LotProfileBuyerComponent } from './lot-profile-buyer/lot-profile-buyer.component';
import { MatIconModule } from '@angular/material/icon';
import { buyerRoutingModule } from './buyer-routing.module';

@NgModule({
  declarations: [
    LotProfileBuyerComponent
  ],
  imports: [
    CommonModule,
    MatIconModule,
    buyerRoutingModule
  ],
  exports: [
    LotProfileBuyerComponent
  ],
})
export class BuyerModule { }
