import { DialogRef, DIALOG_DATA } from '@angular/cdk/dialog';
import {
    Component,
    Inject,
    ViewChild,
    ElementRef,
    AfterViewInit,
    OnInit,
} from '@angular/core';
import { FormGroup, Validators } from '@angular/forms';
import { LotFormModel } from '../../create-lot.component';
import { MatSnackBar } from '@angular/material/snack-bar';

declare var google: any;

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
export class LocationPopUpComponent implements AfterViewInit {
    @ViewChild('addressInput') addressInput!: ElementRef;
    locationGroup: FormGroup<LotFormModel>;

    latitude!: number;
    longitude!: number;
    disabled: boolean = true;

    constructor(
        public dialogRef: DialogRef<string>,
        @Inject(DIALOG_DATA) public data: FormGroup<LotFormModel>,
        private snackBar: MatSnackBar
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
            controls.city.valid &&
            this.latitude !== undefined &&
            this.longitude !== undefined
        ) {
            this.dialogRef.close('true');
        } else {
            this.showSnackBar(
                'Incorrect location. Please ensure the address is correct and try again.',
                'error'
            );
        }
    }

    showSnackBar(message: string, messageType: 'success' | 'error') {
        let panelClass = ['custom-snackbar'];
        if (messageType === 'success') {
            panelClass.push('success-snackbar');
        } else if (messageType === 'error') {
            panelClass.push('error-snackbar');
        }

        this.snackBar.open(message, 'Close', {
            duration: 3000,
            horizontalPosition: 'center',
            verticalPosition: 'bottom',
            panelClass: panelClass,
        });
    }

    ngAfterViewInit() {
        this.initAutocomplete();
        
        this.locationGroup.controls['country'].disable();
        this.locationGroup.controls['city'].disable();
    }

    initAutocomplete(): void {
        const autocomplete = new google.maps.places.Autocomplete(
            this.addressInput.nativeElement
        );
        autocomplete.addListener('place_changed', () => {
            const place = autocomplete.getPlace();
            let country = null;
            let city = null;

            if (!place.geometry) {
                console.log(
                    "Autocomplete's returned place contains no geometry"
                );
                return;
            }

            this.latitude = place.geometry.location.lat();
            this.longitude = place.geometry.location.lng();

            place.address_components.forEach(
                (component: { types: any; long_name: any }) => {
                    const types = component.types;
                    if (types.includes('country')) {
                        country = component.long_name;
                    }
                    if (
                        types.includes('locality') ||
                        types.includes('administrative_area_level_1')
                    ) {
                        city = component.long_name;
                    }
                }
            );

            this.locationGroup.controls['country'].setValue(country);
            this.locationGroup.controls['city'].setValue(city);
            this.locationGroup.controls['address'].setValue(
                place.formatted_address
            );

            this.locationGroup.controls['latitude'].setValue(
                this.latitude.toString()
            );
            this.locationGroup.controls['longitude'].setValue(
                this.longitude.toString()
            );
        });
    }

    closePopup() {
        this.dialogRef.close();
    }
}
