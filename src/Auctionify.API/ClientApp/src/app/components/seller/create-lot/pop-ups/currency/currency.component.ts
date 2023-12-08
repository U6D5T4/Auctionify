import { DialogRef, DIALOG_DATA } from '@angular/cdk/dialog';
import { Component, Inject } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Currency } from 'src/app/web-api-client';
import { LotFormModel } from '../../create-lot.component';

export interface CurrencyDialogData {
    currencies: Currency[];
    formGroup: FormGroup<LotFormModel>;
}

@Component({
    selector: 'app-currency',
    templateUrl: './currency.component.html',
    styleUrls: ['./currency.component.scss'],
})
export class CurrencyPopUpComponent {
    currencies: Currency[] = [];

    startingPriceFormGroup: FormGroup<LotFormModel>;

    constructor(
        public dialogRef: DialogRef<string>,
        @Inject(DIALOG_DATA) public data: CurrencyDialogData
    ) {
        this.startingPriceFormGroup = data.formGroup;
        this.currencies = data.currencies;
    }

    submit() {
        const controls = this.startingPriceFormGroup.controls;
        controls.startingPrice.markAsTouched();
        controls.currencyId.markAsTouched();
        this.configureValidators();
        if (controls.startingPrice.valid && controls.currencyId.valid) {
            this.dialogRef.close('true');
        }
    }

    configureValidators() {
        if (this.startingPriceFormGroup.controls.startingPrice.value !== null)
            this.startingPriceFormGroup.controls.startingPrice.addValidators([
                Validators.min(1),
                Validators.max(1000000000000),
            ]);

        this.startingPriceFormGroup.controls.startingPrice.updateValueAndValidity();
    }

    closePopup() {
        this.dialogRef.close();
    }
}
