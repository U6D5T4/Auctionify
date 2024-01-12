import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InputComponent } from './input/input.component';
import { ButtonComponent } from './button/button.component';
import { DialogPopupComponent } from './dialog-popup/dialog-popup.component';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { ChoicePopupComponent } from './choice-popup/choice-popup.component';
import { RateItemComponent } from './rate-item/rate-item.component';
import { MatIconModule } from '@angular/material/icon';

@NgModule({
    declarations: [
        ButtonComponent,
        InputComponent,
        DialogPopupComponent,
        ChoicePopupComponent,
        RateItemComponent,
    ],
    imports: [CommonModule, FormsModule, MatButtonModule, MatIconModule],
    exports: [InputComponent, ButtonComponent, RateItemComponent],
})
export class UiElementsModule {}
