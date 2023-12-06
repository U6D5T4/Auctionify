import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LotProfileComponent } from './lot-profile/lot-profile.component';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@NgModule({
    declarations: [LotProfileComponent],
    imports: [CommonModule, MatButtonModule, MatIconModule],
    exports: [LotProfileComponent],
})
export class GeneralModule {}
