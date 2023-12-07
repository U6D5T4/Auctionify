import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LotProfileComponent } from './lot-profile/lot-profile.component';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { RouterModule } from '@angular/router';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ImagePopupComponent } from './image-popup/image-popup.component';

@NgModule({
    declarations: [LotProfileComponent, ImagePopupComponent],
    imports: [
        CommonModule,
        RouterModule,
        MatButtonModule,
        MatIconModule,
        MatProgressSpinnerModule,
    ],
    exports: [LotProfileComponent],
})
export class GeneralModule {}
