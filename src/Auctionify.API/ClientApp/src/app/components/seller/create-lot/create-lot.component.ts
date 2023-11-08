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
import { CreateLotModel } from 'src/app/models/lots/lot-models';
import { DialogPopupComponent } from 'src/app/ui-elements/dialog-popup/dialog-popup.component';
import {
    CategoryResponse,
    Client,
    CurrencyResponse,
} from 'src/app/web-api-client';

@Injectable({
    providedIn: 'root',
})
@Component({
    selector: 'app-create-lot',
    templateUrl: './create-lot.component.html',
    styleUrls: ['./create-lot.component.scss'],
})
export class CreateLotComponent {
    private imageInputSelectId: number = 0;
    private imageInputsButtonSize: number = 4;
    inputButtons: number[] = [...Array(this.imageInputsButtonSize).keys()];
    currentFileButtonId: number | undefined;

    isLocationValid: boolean = true;

    @ViewChild('imageInput')
    imageInput!: ElementRef;

    @ViewChild('fileInput')
    fileInput!: ElementRef;

    @ViewChildren('imageElements')
    imageElements!: QueryList<ElementRef>;

    imagesToUpload: FileModel[] = [];
    filesToUpload: FileModel[] = [];

    categories: CategoryResponse[] = [];
    currencies: CurrencyResponse[] = [];

    lotForm = new FormGroup({
        title: new FormControl<string>('', Validators.required),
        description: new FormControl<string>('', Validators.required),
        startingPrice: new FormControl<number | null>(0),
        startDate: new FormControl<Date>(new Date()),
        endDate: new FormControl<Date>(new Date()),
        categoryId: new FormControl<number | null>(null),
        city: new FormControl<string>('', Validators.required),
        country: new FormControl<string>('', Validators.required),
        address: new FormControl<string>('', Validators.required),
        currencyId: new FormControl<number | null>(null),
    });

    isLoading = false;

    constructor(
        private client: Client,
        private dialog: Dialog,
        private router: Router
    ) {
        this.populateCategorySelector();
        this.populateCurrencySelector();
    }

    submitLot(isDraft: boolean) {
        if (!this.lotForm.valid) {
            const controls = this.lotForm.controls;
            if (
                !controls.country.valid ||
                !controls.city.valid ||
                !controls.address.valid
            ) {
                this.isLocationValid = false;
            }

            return;
        }
        this.isLoading = true;
        this.isLocationValid = true;

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
                lotToCreate.photos?.push(image.file!);
            }
        }

        if (this.filesToUpload) {
            for (const file of this.filesToUpload) {
                lotToCreate.additionalDocuments?.push(file.file!);
            }
        }

        this.client.createLot(lotToCreate).subscribe({
            next: (res) => {
                const dialog = this.openDialog(
                    ['Successfully created the lot!'],
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
                    for (let key in errors) {
                        if (errors.hasOwnProperty(key)) {
                            let errorStringForKey = `${key}: `;
                            if (errors[key] instanceof Array) {
                                for (const item of errors[key]) {
                                    errorStringForKey += `${item}`;
                                }
                            } else errorStringForKey += errors[key];

                            errorsToShow.push(errorStringForKey);
                        }
                    }
                }

                const dialog = this.openDialog(errorsToShow, true);

                dialog.closed.subscribe(() => (this.isLoading = false));
            },
        });
    }

    imageUpdateEvent(event: Event) {
        if (event.target === null) return;
        const files = (event.target as HTMLInputElement).files;

        if (files === null) return;

        if (files.length > 1) {
            for (let i = 0; i < files.length; i++) {
                this.inputButtons.push(i);
            }
            this.imageElements.changes.subscribe(() =>
                this.multipleImageUpdate(files)
            );
        } else {
            const file: FileModel = {
                id: this.imageInputSelectId,
                file: files.item(0)!,
            };
            this.imageRendering(file);
            this.imagesToUpload.push(file);
            this.addRemoveBtnToImage(this.imageInputSelectId);
        }
    }

    multipleImageUpdate(files: FileList) {
        for (let index = 0; index < files.length; index++) {
            const element: File = files.item(index)!;

            const file: FileModel = {
                id: this.imageInputSelectId,
                file: element,
            };
            this.imageRendering(file);
            this.imagesToUpload.push(file);
            this.addRemoveBtnToImage(this.imageInputSelectId);
            this.imageInputSelectId++;
        }
    }

    imageRendering(file: FileModel) {
        const reader = new FileReader();

        reader.onload = (e) => {
            const item = document.getElementById(
                `photo-button-${file.id}`
            ) as HTMLImageElement;

            if (item.getAttribute('src') !== '') {
                this.removeImageFromInput(file.id);
                this.imagesToUpload.push(file);
            }

            item.src = reader.result as string;
            this.addRemoveBtnToImage(file.id);
        };

        reader.readAsDataURL(file.file!);
    }

    removeImageFromInput(index: number) {
        const item = document.getElementById(
            `photo-button-${index}`
        ) as HTMLImageElement;

        item.src = '';

        this.imagesToUpload = this.imagesToUpload.filter(
            (value) => value.id !== index
        );
        this.removeRemoveBtnFromImage(index);
    }

    removeRemoveBtnFromImage(index: number) {
        const removeBtn = document.getElementById(
            `photo-button-remove-${index}`
        );

        (removeBtn?.parentNode as HTMLElement).classList.remove('image-added');
    }

    addRemoveBtnToImage(index: number) {
        const removeBtn = document.getElementById(
            `photo-button-remove-${index}`
        );

        (removeBtn?.parentNode as HTMLElement).classList.add('image-added');
    }

    fileUpdateEvent(event: Event) {
        if (event.target === null) return;
        const files = (event.target as HTMLInputElement).files;

        if (files === null) return;
        for (let i = 0; i < files.length; i++) {
            const element = files.item(i);

            const file: FileModel = {
                id: this.filesToUpload.length + 1,
                file: element!,
            };

            this.filesToUpload.push(file);
        }
    }

    handleImageInputButtonClick(buttonId: any, index: number) {
        if (this.imageInput) {
            this.currentFileButtonId = buttonId;
            this.imageInput.nativeElement.click();
            this.imageInputSelectId = index;
        }
    }

    handleFileInputButtonClick() {
        if (this.fileInput) {
            this.fileInput.nativeElement.click();
        }
    }

    removeInputFile(id: number) {
        this.filesToUpload = this.filesToUpload.filter((val) => val.id !== id);
    }

    populateCategorySelector() {
        this.client.getAllCategories().subscribe({
            next: (result: CategoryResponse[]) => {
                this.categories = result;
            },
        });
    }

    populateCurrencySelector() {
        this.client.getAllCurrencies().subscribe({
            next: (result: CurrencyResponse[]) => {
                this.currencies = result;
            },
        });
    }

    clickLocation() {
        const locationElement = document.getElementById(
            'button-select-parent-location'
        );

        locationElement?.classList.toggle('show');
    }

    clickStartingPrice() {
        const locationElement = document.getElementById(
            'button-select-parent-starting-price'
        );

        locationElement?.classList.toggle('show');
    }

    clickFile() {
        const locationElement = document.getElementById(
            'button-select-parent-file'
        );

        locationElement?.classList.toggle('show');
    }

    openDialog(text: string[], error: boolean): DialogRef<string, unknown> {
        return this.dialog.open<string>(DialogPopupComponent, {
            data: {
                text,
                isError: error,
            },
        });
    }
}
