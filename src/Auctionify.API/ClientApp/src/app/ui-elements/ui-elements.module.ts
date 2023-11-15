import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InputComponent } from './input/input.component';
import { ButtonComponent } from './button/button.component';
import { DialogPopupComponent } from './dialog-popup/dialog-popup.component';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';



@NgModule({
  declarations: [
    ButtonComponent,
    InputComponent,
    ButtonComponent,
    DialogPopupComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    MatButtonModule
  ],
  exports: [
    InputComponent,
    ButtonComponent
  ]
})
export class UiElementsModule { }
