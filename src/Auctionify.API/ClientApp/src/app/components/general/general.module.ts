import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LotProfileComponent } from './lot-profile/lot-profile.component';

@NgModule({
    declarations: [LotProfileComponent],
    imports: [CommonModule],
    exports: [LotProfileComponent],
})
export class GeneralModule {}
