import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InputComponent } from './input/input.component';
import { ButtonComponent } from './button/button.component';
import { DialogPopupComponent } from './dialog-popup/dialog-popup.component';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { ChoicePopupComponent } from './choice-popup/choice-popup.component';
import { LotItemComponent } from './lot-item/lot-item.component';
import { RouterModule } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';

@NgModule({
    declarations: [
        ButtonComponent,
        InputComponent,
        DialogPopupComponent,
        ChoicePopupComponent,
        LotItemComponent,
    ],
    imports: [
        CommonModule,
        FormsModule,
        MatButtonModule,
        RouterModule,
        MatIconModule,
    ],
    exports: [InputComponent, ButtonComponent, LotItemComponent],
})
export class UiElementsModule {}
