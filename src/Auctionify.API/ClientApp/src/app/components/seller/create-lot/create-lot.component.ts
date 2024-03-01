import { Dialog, DialogRef } from '@angular/cdk/dialog';
import {
    Component,
    ElementRef,
    Injectable,
    OnInit,
    QueryList,
    ViewChild,
    ViewChildren,
} from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';

import { FileModel } from 'src/app/models/fileModel';
import { DialogPopupComponent } from 'src/app/ui-elements/dialog-popup/dialog-popup.component';
import { Category, Client, Currency } from 'src/app/web-api-client';
import { LocationPopUpComponent } from './pop-ups/location/location.component';
import { CurrencyPopUpComponent } from './pop-ups/currency/currency.component';
import { FilesPopUpComponent } from './pop-ups/files/files.component';
import { CreateLotModel, UpdateLotModel } from 'src/app/models/lots/lot-models';
import {
    ChoicePopupComponent,
    ChoicePopupData,
} from 'src/app/ui-elements/choice-popup/choice-popup.component';
import { MatSnackBar } from '@angular/material/snack-bar';

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
    latitude: FormControl<string | null>;
    longitude: FormControl<string | null>;
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
export class CreateLotComponent implements OnInit {
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
        latitude: new FormControl<string>('', Validators.required),
        longitude: new FormControl<string>('', Validators.required),
        currencyId: new FormControl<number | null>(null),
        files: new FormControl<FileModel[]>([]),
        images: new FormControl<FileModel[]>([]),
    });

    isLoading = false;
    isLoadingDraft = false;
    isDeleteLoading = false;

    private updateStateEndpoint: string = 'update-lot';

    lotId: number = 0;
    private regExValue: RegExp = /\/([^\/]+)(?=\/?$)/g;

    filesText: string | null = null;
    locationText: string | null = null;
    startingPriceText: string | null = null;

    constructor(
        private client: Client,
        private dialog: Dialog,
        private router: Router,
        private route: ActivatedRoute,
        private snackBar: MatSnackBar
    ) {
        this.populateCategorySelector();
        this.populateCurrencySelector();
    }

    ngOnInit(): void {
        this.route.url.subscribe((params) => {
            const path = params[0].path;

            if (path == this.updateStateEndpoint) {
                this.updateStateInitialization();
            }
        });
    }

    updateStateInitialization() {
        this.route.paramMap.subscribe((params: ParamMap) => {
            const lotIdString = params.get('id');
            if (lotIdString === null) {
                this.router.navigate(['/home']);
                return;
            }

            const lotId: number = parseInt(lotIdString);
            this.lotId = lotId;

            this.client.getOneLotForSeller(lotId).subscribe({
                next: (result) => {
                    const lotFormData = this.lotForm.controls;

                    lotFormData.title.setValue(result.title);
                    lotFormData.description.setValue(result.description);
                    lotFormData.categoryId.setValue(
                        result.category ? result.category.id : null
                    );
                    lotFormData.currencyId.setValue(
                        result.currency ? result.currency.id : null
                    );
                    lotFormData.startDate.setValue(result.startDate);
                    lotFormData.endDate.setValue(result.endDate);
                    lotFormData.country.setValue(result.location.country);
                    lotFormData.address.setValue(result.location.address);
                    lotFormData.city.setValue(result.location.city);
                    lotFormData.latitude.setValue(result.location.latitude);
                    lotFormData.longitude.setValue(result.location.longitude);
                    lotFormData.startingPrice.setValue(result.startingPrice);

                    if (result.additionalDocumentsUrl !== null) {
                        for (const [
                            i,
                            fileUrl,
                        ] of result.additionalDocumentsUrl.entries()) {
                            const fileName = this.getFileNameFromUrl(fileUrl);

                            const fileModel: FileModel = {
                                name: fileName,
                                file: null,
                                fileUrl,
                            };
                            this.lotForm.controls.files.value?.push(fileModel);
                        }

                        this.filesText = this.lotForm.value.files?.at(0)?.name!;
                    }

                    if (result.photosUrl !== null) {
                        for (const [i, fileUrl] of result.photosUrl.entries()) {
                            const fileName = this.getFileNameFromUrl(fileUrl);
                            const fileModel: FileModel = {
                                name: fileName,
                                file: null,
                                fileUrl,
                            };
                            this.imagesToUpload.push(fileModel);
                            const subscription =
                                this.imageElements.changes.subscribe(() => {
                                    this.imageRendering(fileModel);
                                    this.imageElements;
                                    subscription.unsubscribe();
                                });
                        }
                    }

                    if (this.lotForm.value.city) {
                        this.locationText = this.lotForm.value.city!;
                    }

                    if (this.lotForm.value.startingPrice) {
                        this.startingPriceTextSetter();
                    }
                },

                error: (err) => {
                    this.router.navigate(['/home']);
                },
            });
        });
    }

    private getFileNameFromUrl(fileUrl: string): string {
        const fileName = fileUrl.match(this.regExValue);
        if (fileName === null) return '';

        return decodeURI(fileName[0].split('/')[1]);
    }

    submitLot(isDraft: boolean) {
        this.configureLotFormValidators(isDraft);
        this.isLocationValid = true;
        this.isStartingPriceValid = true;
        const controls = this.lotForm.controls;

        this.lotForm.markAllAsTouched();

        if (!this.lotForm.valid) {
            if (
                controls.country.invalid ||
                controls.city.invalid ||
                controls.address.invalid ||
                controls.latitude.invalid ||
                controls.longitude.invalid
            ) {
                this.isLocationValid = false;
                this.showSnackBar(
                    'Incorrect location. Please ensure the address is correct and try again.',
                    'error'
                );
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
            latitude: this.lotForm.value.latitude!,
            longitude: this.lotForm.value.longitude!,
            state: null,
            country: this.lotForm.value.country!,
            address: this.lotForm.value.address!,
            currencyId: this.lotForm.value.currencyId!,
            photos: [],
            additionalDocuments: [],
            isDraft,
        };

        if (this.imagesToUpload.length > 0) {
            for (const image of this.imagesToUpload) {
                lotToCreate.photos?.push(image.file!);
            }
        }

        if (this.lotForm.value.files?.length! > 0) {
            for (const file of this.lotForm.controls.files.value!) {
                lotToCreate.additionalDocuments?.push(file.file!);
            }
        }

        if (this.lotId > 0) {
            const lotToUpdate: UpdateLotModel = {
                id: this.lotId,
                ...lotToCreate,
            };
            this.client.updateLot(lotToUpdate).subscribe({
                next: (res) => {
                    const dialog = this.openDialog(
                        ['Successfully updated the lot!'],
                        false,
                        false
                    );

                    dialog.closed.subscribe(() =>
                        this.router.navigate(['/seller'])
                    );
                },
                error: (err) => {
                    this.handleErrorResponse(err, isDraft);
                },
            });
        } else {
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
                    this.handleErrorResponse(err, isDraft);
                },
            });
        }
    }

    private handleErrorResponse(err: any, isDraft: boolean) {
        const errors = err.errors;
        let errorsToShow = [];

        if (typeof errors == 'string') {
            errorsToShow.push(errors.toString());
        } else {
            for (let key of errors) {
                let msg = `${key.PropertyName}: ${key.ErrorMessage}\n`;
                errorsToShow.push(msg);
            }
        }

        const dialog = this.openDialog(errorsToShow, true, true);

        dialog.closed.subscribe(() =>
            isDraft ? (this.isLoadingDraft = false) : (this.isLoading = false)
        );
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
                Validators.max(1000000000000),
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
            let existingImagesName: string[] = [];
            for (let i = 0; i < filesList.length; i++) {
                const existingFile = this.imagesToUpload.find(
                    (x) => x.name == filesList[i].name
                );
                if (existingFile) {
                    filesArray = filesArray.filter(
                        (x) => x.name !== filesList[i].name
                    );
                    existingImagesName.push(existingFile.name!);
                    continue;
                }
            }

            if (existingImagesName.length > 0) {
                this.openDialog(
                    [
                        `You tried to add the following existing images, so they will not be added again:`,
                        ...existingImagesName,
                    ],
                    true,
                    false
                );
            }

            const iterations = 20 - this.imagesToUpload.length;

            filesArray = filesArray.slice(0, iterations);

            this.multipleImageUpdate(filesArray);
        } else {
            const file = filesList[0];
            const element: FileModel = {
                file,
                name: file.name,
                fileUrl: null,
            };
            if (this.imagesToUpload.find((x) => x.name == element.name)) {
                this.openDialog(
                    [
                        `These images already exist and won't be added again:`,
                        element.name!,
                    ],
                    true,
                    false
                );
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
                fileUrl: null,
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
        if (!file.file) {
            const item = document.getElementById(
                `photo-button-${file.name}`
            ) as HTMLImageElement;

            if (item.getAttribute('src') === '') {
                item.src = file.fileUrl!;
                this.addRemoveBtnToImage(file.name!);
            }
        } else {
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

            reader.readAsDataURL(file.file!);
        }
    }

    removeImageFromInput(name: string) {
        const imageToDelete = this.imagesToUpload.find(
            (value) => value.name === name
        );

        if (imageToDelete === null || imageToDelete === undefined) return;

        if (imageToDelete.fileUrl) {
            const msg = ['This action will totally delete image from system.'];
            const dialogData: ChoicePopupData = {
                isError: true,
                isErrorShown: true,
                continueBtnText: 'Delete',
                breakBtnText: 'Cancel',
                text: msg,
                additionalText: 'Continue?',
                continueBtnColor: 'warn',
                breakBtnColor: 'primary',
            };
            const openedChoiceDialog = this.openChoiceDialog(dialogData);
            const dialogSubscriber = openedChoiceDialog.closed.subscribe(
                (result) => {
                    const isResult = result === 'true';
                    if (isResult) {
                        const deleteLotSubscriber = this.client
                            .deleteLotFile(this.lotId, imageToDelete.fileUrl!)
                            .subscribe((res) => {
                                this.proceedRemoveImage(name);
                                deleteLotSubscriber.unsubscribe();
                            });
                    }

                    dialogSubscriber.unsubscribe();
                }
            );
        } else {
            this.proceedRemoveImage(name);
        }
    }

    proceedRemoveImage(name: string) {
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
            autoFocus: false,
        });

        locationDialogPopup.closed.subscribe((res: any) => {
            const controls = this.lotForm.controls;
            if (
                controls.address.valid &&
                controls.city.valid &&
                controls.country.valid
            ) {
                this.isLocationValid = true;
                this.locationText = this.lotForm.value.city!;
            } else {
                this.locationText = null;
            }
        });
    }

    clickStartingPrice() {
        const locationDialogPopup = this.dialog.open(CurrencyPopUpComponent, {
            data: {
                formGroup: this.lotForm,
                currencies: this.currencies,
            },
            autoFocus: false,
        });

        locationDialogPopup.closed.subscribe((res) => {
            const controls = this.lotForm.controls;

            this.startingPriceText = null;
            if (
                ((controls.startingPrice.valid || controls.currencyId.valid) &&
                    controls.startingPrice.value! > 0) ||
                controls.currencyId.value! !== null
            ) {
                this.isStartingPriceValid = true;
            } else {
                this.startingPriceText = null;
            }

            this.startingPriceTextSetter();
        });
    }

    clickFile() {
        const filesDialogPopup = this.dialog.open(FilesPopUpComponent, {
            data: {
                lotId: this.lotId,
                formGroup: this.lotForm,
            },
            autoFocus: false,
        });

        filesDialogPopup.closed.subscribe((res) => {});
    }

    startingPriceTextSetter() {
        const controls = this.lotForm.controls;

        if (controls.startingPrice.value! > 0) {
            this.startingPriceText = `from ${this.lotForm.value.startingPrice}, `;
        }

        const currencyId = this.lotForm.value.currencyId;
        if (currencyId) {
            const currency = this.currencies.find((x) => x.id == currencyId);

            if (this.startingPriceText == null) {
                this.startingPriceText = `${currency?.code}`;
            } else {
                this.startingPriceText = this.startingPriceText.concat(
                    `${currency?.code}`
                );
            }
        }
    }

    deleteLot() {
        this.isDeleteLoading = true;
        const msg = ['This action will delete lot or cancel it.'];
        const dialogData: ChoicePopupData = {
            isError: true,
            isErrorShown: true,
            continueBtnText: 'Delete',
            breakBtnText: 'Cancel',
            text: msg,
            additionalText: 'Continue?',
            continueBtnColor: 'warn',
            breakBtnColor: 'primary',
        };
        const openedChoiceDialog = this.openChoiceDialog(dialogData);
        const dialogSubscriber = openedChoiceDialog.closed.subscribe(
            (result) => {
                const isResult = result === 'true';
                if (isResult) {
                    const deleteLotSubscriber = this.client
                        .deleteLot(this.lotId)
                        .subscribe({
                            next: (res) => {
                                this.snackBar.open(res, 'Ok', {
                                    duration: 10000,
                                });
                                this.router.navigate(['/home']);
                                deleteLotSubscriber.unsubscribe();
                            },
                            error: (error) => {},
                            complete: () => {
                                this.isDeleteLoading = false;
                            },
                        });
                }

                this.isDeleteLoading = false;
                dialogSubscriber.unsubscribe();
            }
        );
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
            autoFocus: false,
        });
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

    openChoiceDialog(data: ChoicePopupData): DialogRef<string, unknown> {
        return this.dialog.open<string>(ChoicePopupComponent, {
            data,
            autoFocus: false,
        });
    }
}
