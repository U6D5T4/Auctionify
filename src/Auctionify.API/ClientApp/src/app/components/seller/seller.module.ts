import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CreateLotComponent } from './create-lot/create-lot.component';
import { sellerRoutingModule } from './seller-routing.module';
import { MatFormFieldModule } from '@angular/material/form-field';
import { ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';

@NgModule({
  declarations: [CreateLotComponent],
  imports: [
    CommonModule,
    sellerRoutingModule,
    MatFormFieldModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
  ],
  exports: [CreateLotComponent],
})
export class SellerModule {}
