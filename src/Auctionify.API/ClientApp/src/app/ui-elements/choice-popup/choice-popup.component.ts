import { DIALOG_DATA, DialogRef } from '@angular/cdk/dialog';
import { Component, Inject } from '@angular/core';
import { DialogData } from '../dialog-popup/dialog-popup.component';

export interface ChoicePopupData extends DialogData {
    continueBtnText: string;
    breakBtnText: string;
    additionalText: string;
    continueBtnColor: string;
    breakBtnColor: string;
}

@Component({
    selector: 'app-choice-popup',
    templateUrl: './choice-popup.component.html',
    styleUrls: ['./choice-popup.component.scss'],
})
export class ChoicePopupComponent {
    iconPath: string = '';
    messages: string[] = [];
    continueBtnText: string = '';
    breakBtnText: string = '';
    additionalText: string = '';
    continueBtnColor: string = '';
    breakBtnColor: string = '';

    constructor(
        public dialogRef: DialogRef<string>,
        @Inject(DIALOG_DATA) public data: ChoicePopupData
    ) {
        this.messages = data.text;
        this.iconPath = data.isError
            ? '../../../assets/icons/alert-hexagon.svg'
            : '../../../assets/icons/success.svg';

        this.continueBtnText = data.continueBtnText;
        this.breakBtnText = data.breakBtnText;
        this.additionalText = data.additionalText;
        this.continueBtnColor = data.continueBtnColor;
        this.breakBtnColor = data.breakBtnColor;

        this.dialogRef.outsidePointerEvents.subscribe(() => {
            this.dialogRef.close('false');
        });
    }

    closeDialog(state: boolean) {
        this.dialogRef.close(`${state}`);
    }
}
