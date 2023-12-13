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
import { ProfileModule } from './profile/profile.module';
import { GeneralRoutingModule } from './general-routing.module';

@NgModule({
    declarations: [LotProfileComponent, ImagePopupComponent, AddBidComponent],
    imports: [
        CommonModule,
        RouterModule,
        MatButtonModule,
        MatIconModule,
        MatProgressSpinnerModule,
        ReactiveFormsModule,
        MatFormFieldModule,
        MatInputModule,
        ProfileModule,
        GeneralRoutingModule,
    ],

    exports: [ProfileModule, LotProfileComponent],
})
export class GeneralModule {}
