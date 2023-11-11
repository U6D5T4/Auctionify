import { Dialog, DialogRef } from '@angular/cdk/dialog';
import {
    Component,
    ElementRef,
    Injectable,
    QueryList,
    ViewChild,
    ViewChildren,
} from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';

import { FileModel } from 'src/app/models/fileModel';
import { DialogPopupComponent } from 'src/app/ui-elements/dialog-popup/dialog-popup.component';
import { Category, Client, Currency } from 'src/app/web-api-client';
import {
    LocationDialogData,
    LocationPopUpComponent,
} from './pop-ups/location/location.component';
import { CurrencyPopUpComponent } from './pop-ups/currency/currency.component';
import { FilesPopUpComponent } from './pop-ups/files/files.component';

export interface CreateLotModel {
    title: string;
    description: string;
    startingPrice: number | null;
    startDate: Date | null;
    endDate: Date | null;
    categoryId: number | null;
    city: string;
    state: string | null;
    country: string;
    address: string;
    currencyId: number | null;
    photos: File[] | null;
    additionalDocuments: File[] | null;
    isDraft: boolean;
}

export interface LotFormModel {
    title: FormControl<string | null>;
    description: FormControl<string | null>;
    startingPrice: FormControl<number | null>;
    startDate: FormControl<Date | null>;
    endDate: FormControl<Date | null>;
    categoryId: FormControl<number | null>;
    city: FormControl<string | null>;
    country: FormControl<string | null>;
    address: FormControl<string | null>;
    currencyId: FormControl<number | null>;
    files: FormControl<FileModel[] | null>;
    images: FormControl<FileModel[] | null>;
}

@Injectable({
    providedIn: 'root',
})
@Component({
    selector: 'app-create-lot',
    templateUrl: './create-lot.component.html',
    styleUrls: ['./create-lot.component.scss'],
})
export class CreateLotComponent {
    currentFileButtonId: number | undefined;

    isLocationValid: boolean = true;
    isStartingPriceValid: boolean = true;

    @ViewChild('imageInput')
    imageInput!: ElementRef;

    @ViewChildren('imageElements')
    imageElements!: QueryList<ElementRef>;

    imagesToUpload: FileModel[] = [];
    filesToUpload: FileModel[] = [];

    categories: Category[] = [];
    currencies: Currency[] = [];

    lotForm = new FormGroup<LotFormModel>({
        title: new FormControl<string>('', [
            Validators.required,
            Validators.minLength(6),
            Validators.maxLength(64),
        ]),
        description: new FormControl<string>('', [
            Validators.required,
            Validators.minLength(30),
            Validators.maxLength(500),
        ]),
        startingPrice: new FormControl<number | null>(null),
        startDate: new FormControl<Date | null>(null),
        endDate: new FormControl<Date | null>(null),
        categoryId: new FormControl<number | null>(null),
        city: new FormControl<string>('', Validators.required),
        country: new FormControl<string>('', Validators.required),
        address: new FormControl<string>('', Validators.required),
        currencyId: new FormControl<number | null>(null),
        files: new FormControl<FileModel[]>([]),
        images: new FormControl<FileModel[]>([]),
    });

    isLoading = false;
    isLoadingDraft = false;

    constructor(
        private client: Client,
        private dialog: Dialog,
        private router: Router
    ) {
        this.populateCategorySelector();
        this.populateCurrencySelector();
    }

    submitLot(isDraft: boolean) {
        console.log(this.lotForm);
        console.log(this.imagesToUpload);
        this.configureLotFormValidators(isDraft);
        this.isLocationValid = true;
        this.isStartingPriceValid = true;
        const controls = this.lotForm.controls;

        console.log(controls);

        this.lotForm.markAllAsTouched();

        if (!this.lotForm.valid) {
            if (
                controls.country.invalid ||
                controls.city.invalid ||
                controls.address.invalid
            ) {
                this.isLocationValid = false;
            }

            if (controls.startingPrice.invalid || controls.currencyId.invalid) {
                this.isStartingPriceValid = false;
            }

            return;
        }

        isDraft ? (this.isLoadingDraft = true) : (this.isLoading = true);

        this.isLocationValid = true;
        this.isStartingPriceValid = true;

        const lotToCreate: CreateLotModel = {
            title: this.lotForm.value.title!,
            description: this.lotForm.value.description!,
            startingPrice: this.lotForm.value.startingPrice!,
            startDate: this.lotForm.value.startDate!,
            endDate: this.lotForm.value.endDate!,
            categoryId: this.lotForm.value.categoryId!,
            city: this.lotForm.value.city!,
            state: null,
            country: this.lotForm.value.country!,
            address: this.lotForm.value.address!,
            currencyId: this.lotForm.value.currencyId!,
            photos: [],
            additionalDocuments: [],
            isDraft,
        };

        if (this.imagesToUpload) {
            for (const image of this.imagesToUpload) {
                lotToCreate.photos?.push(image.file);
            }
        }

        if (this.filesToUpload) {
            for (const file of this.lotForm.controls.files.value!) {
                lotToCreate.additionalDocuments?.push(file.file);
            }
        }

        this.client.createLot(lotToCreate).subscribe({
            next: (res) => {
                const dialog = this.openDialog(
                    ['Successfully created the lot!'],
                    false,
                    false
                );

                dialog.closed.subscribe(() =>
                    this.router.navigate(['/seller'])
                );
            },
            error: (err) => {
                const errors = err.errors;
                let errorsToShow = [];

                if (typeof errors == 'string') {
                    errorsToShow.push(errors.toString());
                } else {
                    for (let key of errors) {
                        console.log(key);
                        let msg = `${key.PropertyName}: ${key.ErrorMessage}\n`;
                        errorsToShow.push(msg);
                    }
                }

                console.log(err);

                const dialog = this.openDialog(errorsToShow, true, true);

                dialog.closed.subscribe(() =>
                    isDraft
                        ? (this.isLoadingDraft = false)
                        : (this.isLoading = false)
                );
            },
        });
    }

    private configureLotFormValidators(isDraft: boolean) {
        if (!isDraft) {
            this.lotForm.controls.categoryId.addValidators(Validators.required);
            this.lotForm.controls.startDate.addValidators(Validators.required);
            this.lotForm.controls.endDate.addValidators(Validators.required);
            this.lotForm.controls.currencyId.addValidators(Validators.required);

            this.lotForm.controls.startingPrice.addValidators(
                Validators.required
            );
        } else {
            this.lotForm.controls.categoryId.removeValidators(
                Validators.required
            );
            this.lotForm.controls.startDate.removeValidators(
                Validators.required
            );
            this.lotForm.controls.endDate.removeValidators(Validators.required);
            this.lotForm.controls.currencyId.removeValidators(
                Validators.required
            );
            this.lotForm.controls.startingPrice.removeValidators(
                Validators.required
            );
        }

        if (this.lotForm.controls.startingPrice.value !== null)
            this.lotForm.controls.startingPrice.addValidators([
                Validators.min(1),
                Validators.max(9999),
            ]);

        this.lotForm.controls.categoryId.updateValueAndValidity();
        this.lotForm.controls.startDate.updateValueAndValidity();
        this.lotForm.controls.endDate.updateValueAndValidity();
        this.lotForm.controls.currencyId.updateValueAndValidity();
        this.lotForm.controls.startingPrice.updateValueAndValidity();
    }

    imageUpdateEvent(event: Event) {
        if (event.target === null) return;
        const filesList = (event.target as HTMLInputElement).files;

        if (filesList === null) return;

        let filesArray = [...filesList];

        if (!this.imagesAmountCondition(filesArray)) return;

        if (filesList.length > 1) {
            let addedInputButtons = 0;
            for (let i = 0; i < filesList.length; i++) {
                if (
                    this.imagesToUpload.find((x) => x.name == filesList[i].name)
                ) {
                    filesArray = filesArray.filter(
                        (x) => x.name !== filesList[i].name
                    );
                    continue;
                }

                addedInputButtons++;
            }

            const iterations = 20 - this.imagesToUpload.length;

            filesArray = filesArray.slice(0, iterations);

            this.multipleImageUpdate(filesArray);
        } else {
            const file = filesList[0];
            const element: FileModel = {
                file,
                name: file.name,
            };
            if (this.imagesToUpload.find((x) => x.name == element.name)) {
                return;
            }
            this.imagesToUpload.push(element);
            const imageAddedSubscriber = this.imageElements.changes.subscribe(
                () => {
                    this.imageRendering(element);
                    this.addRemoveBtnToImage(element.name!);
                    imageAddedSubscriber.unsubscribe();
                }
            );
        }
    }

    imagesAmountCondition(filesArray: File[]): boolean {
        if (
            this.imagesToUpload.length >= 20 ||
            filesArray.length + this.imagesToUpload.length > 20
        ) {
            let errorMessages = [];
            errorMessages.push(
                `You can add only 20 images to your lot! So only ${
                    20 - this.imagesToUpload.length
                } chosen images will be added except existing images`
            );
            const dialog = this.openDialog(errorMessages, true, false);
            return true;
        }

        return true;
    }

    multipleImageUpdate(files: File[]) {
        for (let index = 0; index < files.length; index++) {
            const element: FileModel = {
                file: files[index]!,
                name: files[index].name,
            };

            this.imagesToUpload.push(element);

            const imagesChangeSubscriber = this.imageElements.changes.subscribe(
                () => {
                    this.imageRendering(element);
                    this.addRemoveBtnToImage(element.name!);

                    imagesChangeSubscriber.unsubscribe();
                }
            );
        }
    }

    imageRendering(file: FileModel) {
        const reader = new FileReader();

        reader.onload = (e) => {
            const item = document.getElementById(
                `photo-button-${file.name}`
            ) as HTMLImageElement;

            if (item.getAttribute('src') !== '') {
                this.removeImageFromInput(file.name!);
                this.imagesToUpload.push(file);
            }

            item.src = reader.result as string;
            this.addRemoveBtnToImage(file.name!);
        };

        reader.readAsDataURL(file.file);
    }

    removeImageFromInput(name: string) {
        const item = document.getElementById(
            `photo-button-${name}`
        ) as HTMLImageElement;

        item.src = '';

        this.imagesToUpload = this.imagesToUpload.filter(
            (value) => value.name !== name
        );

        this.removeRemoveBtnFromImage(name);
    }

    removeRemoveBtnFromImage(name: string) {
        const removeBtn = document.getElementById(
            `photo-button-remove-${name}`
        );

        (removeBtn?.parentNode as HTMLElement).classList.remove('image-added');
    }

    addRemoveBtnToImage(name: string) {
        const removeBtn = document.getElementById(
            `photo-button-remove-${name}`
        );

        (removeBtn?.parentNode as HTMLElement).classList.add('image-added');
    }

    handleImageInputButtonClick() {
        if (this.imageInput) {
            this.imageInput.nativeElement.click();
        }
    }

    populateCategorySelector() {
        this.client.getAllCategories().subscribe({
            next: (result: Category[]) => {
                this.categories = result;
            },
        });
    }

    populateCurrencySelector() {
        this.client.getAllCurrencies().subscribe({
            next: (result: Currency[]) => {
                this.currencies = result;
            },
        });
    }

    clickLocation() {
        const locationDialogPopup = this.dialog.open(LocationPopUpComponent, {
            data: this.lotForm,
        });

        locationDialogPopup.closed.subscribe((res: any) => {
            console.log(res);
        });
    }

    clickStartingPrice() {
        const locationDialogPopup = this.dialog.open(CurrencyPopUpComponent, {
            data: {
                formGroup: this.lotForm,
                currencies: this.currencies,
            },
        });

        locationDialogPopup.closed.subscribe((res) => {
            console.log(res);
        });
    }

    clickFile() {
        const filesDialogPopup = this.dialog.open(FilesPopUpComponent, {
            data: this.lotForm,
        });

        filesDialogPopup.closed.subscribe((res) => {
            console.log(res);
        });
    }

    openDialog(
        text: string[],
        error: boolean,
        isErrorShown: boolean
    ): DialogRef<string, unknown> {
        return this.dialog.open<string>(DialogPopupComponent, {
            data: {
                text,
                isError: error,
                isErrorShown,
            },
        });
    }
}
