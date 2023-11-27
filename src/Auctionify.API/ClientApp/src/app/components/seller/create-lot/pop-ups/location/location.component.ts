import { DialogRef, DIALOG_DATA } from '@angular/cdk/dialog';
import { Component, Inject } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { LotFormModel } from '../../create-lot.component';

export interface LocationDialogData {
    country: string | null;
    city: string | null;
    address: string | null;
}

@Component({
    selector: 'app-location',
    templateUrl: './location.component.html',
    styleUrls: ['./location.component.scss'],
})
export class LocationPopUpComponent {
    locationGroup: FormGroup<LotFormModel>;

    constructor(
        public dialogRef: DialogRef<string>,
        @Inject(DIALOG_DATA) public data: FormGroup<LotFormModel>
    ) {
        this.locationGroup = data;
    }

    submit() {
        const controls = this.locationGroup.controls;
        controls.address.markAsTouched();
        controls.country.markAsTouched();
        controls.city.markAsTouched();
        if (
            controls.address.valid &&
            controls.country.valid &&
            controls.city.valid
        ) {
            this.dialogRef.close('true');
        }
    }
}
