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
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { FileModel } from 'src/app/models/fileModel';
import { UpdateLotModel } from 'src/app/models/lots/lot-models';
import {
    ChoicePopupComponent,
    ChoicePopupData,
} from 'src/app/ui-elements/choice-popup/choice-popup.component';
import { DialogPopupComponent } from 'src/app/ui-elements/dialog-popup/dialog-popup.component';
import {
    CategoryResponse,
    CurrencyResponse,
    Client,
} from 'src/app/web-api-client';

@Injectable({
    providedIn: 'root',
})
@Component({
    selector: 'app-edit-lot',
    templateUrl: './edit-lot.component.html',
    styleUrls: ['./edit-lot.component.scss'],
})
export class EditLotComponent implements OnInit {
    private regExValue: RegExp = /\/([^\/]+)(?=\/?$)/g;
    private lotId: number = 0;

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
    isDeleteLoading = false;

    constructor(
        private client: Client,
        private dialog: Dialog,
        private choiceDialog: Dialog,
        private router: Router,
        private route: ActivatedRoute,
        private snackBar: MatSnackBar
    ) {
        this.populateCategorySelector();
        this.populateCurrencySelector();
    }

    ngOnInit(): void {
        this.route.paramMap.subscribe((params: ParamMap) => {
            const lotIdString = params.get('id');
            if (lotIdString === null) return;

            const lotId: number = parseInt(lotIdString);
            this.lotId = lotId;

            this.client.getOneLotForSeller(lotId).subscribe((result) => {
                const lotFormData = this.lotForm.controls;

                console.log(result);

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
                lotFormData.startingPrice.setValue(result.startingPrice);

                if (result.additionalDocumentsUrl !== null) {
                    for (const [
                        i,
                        fileUrl,
                    ] of result.additionalDocumentsUrl.entries()) {
                        const fileName = this.getFileNameFromUrl(fileUrl);

                        console.log(fileName);
                        const fileModel: FileModel = {
                            id: i,
                            fileUrl,
                            fileName: fileName,
                        };
                        this.filesToUpload.push(fileModel);
                    }
                }

                if (result.photosUrl !== null) {
                    for (const [i, fileUrl] of result.photosUrl.entries()) {
                        const fileName = this.getFileNameFromUrl(fileUrl);
                        const fileModel: FileModel = {
                            id: i,
                            fileUrl,
                            fileName: fileName,
                        };
                        this.imagesToUpload.push(fileModel);
                        this.inputButtons.push(this.inputButtons.length);
                        const subscription =
                            this.imageElements.changes.subscribe(() => {
                                this.imageRendering(fileModel);
                                this.imageElements;
                                subscription.unsubscribe();
                            });
                    }
                }
            });
        });
    }

    private getFileNameFromUrl(fileUrl: string): string {
        const fileName = fileUrl.match(this.regExValue);
        if (fileName === null) return '';

        return decodeURI(fileName[0].split('/')[1]);
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

        const lotToCreate: UpdateLotModel = {
            id: this.lotId,
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
                if (image.fileUrl) continue;
                lotToCreate.photos?.push(image.file!);
            }
        }

        if (this.filesToUpload) {
            for (const file of this.filesToUpload) {
                if (file.fileUrl) continue;
                lotToCreate.additionalDocuments?.push(file.file!);
            }
        }

        console.log(lotToCreate);

        this.client.updateLot(lotToCreate).subscribe({
            next: (res) => {
                console.log(res);
                const dialog = this.openDialog(
                    ['Successfully updated the lot!'],
                    false
                );

                dialog.closed.subscribe(() =>
                    this.router.navigate(['/seller'])
                );
            },
            error: (err) => {
                console.log(err);
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
            const filesToProceed: File[] = [];
            for (let i = 0; i < files.length; i++) {
                const element: File = files.item(i)!;
                if (
                    this.imagesToUpload.find((x) => x.fileName === element.name)
                )
                    continue;

                if (
                    this.imagesToUpload
                        .filter((x) => x.file)
                        .find((y) => y.file?.name == element.name)
                )
                    continue;

                filesToProceed.push(element);
                this.inputButtons.push(i);
            }
            const subscriber = this.imageElements.changes.subscribe(() => {
                this.multipleImageUpdate(filesToProceed);
                subscriber.unsubscribe();
                console.log(this.imagesToUpload);
            });
        } else {
            const file: FileModel = {
                id: this.imageInputSelectId,
                file: files.item(0)!,
            };
            if (this.imagesToUpload.find((x) => x.fileName === file.file?.name))
                return;

            if (
                this.imagesToUpload
                    .filter((x) => x.file)
                    .find((y) => y.file?.name == file.file?.name)
            )
                return;

            this.imageRendering(file);
            this.imagesToUpload.push(file);
            this.addRemoveBtnToImage(this.imageInputSelectId);
            console.log(this.imagesToUpload);
        }
    }

    multipleImageUpdate(files: File[]) {
        for (let index = 0; index < files.length; index++) {
            const element: File = files[index]!;

            const file: FileModel = {
                id: this.imageInputSelectId!,
                file: element,
            };
            this.imageRendering(file);
            this.imagesToUpload.push(file);
            this.addRemoveBtnToImage(this.imageInputSelectId!);
            this.imageInputSelectId!++;
        }
    }

    imageRendering(file: FileModel) {
        if (file.fileUrl) {
            const item = document.getElementById(
                `photo-button-${file.id}`
            ) as HTMLImageElement;

            if (item.getAttribute('src') === '') {
                item.src = file.fileUrl;
                this.addRemoveBtnToImage(file.id);
            }
        } else {
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
    }

    removeImageFromInput(index: number) {
        const imageToDelete = this.imagesToUpload.find(
            (value) => value.id === index
        );

        if (imageToDelete === null || imageToDelete === undefined) return;

        if (imageToDelete.fileUrl) {
            const msg = ['This action will totally delete image.'];
            const dialogData: ChoicePopupData = {
                isError: true,
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
                                console.log(res);
                                this.proceedRemoveImage(index);
                                deleteLotSubscriber.unsubscribe();
                            });
                    }

                    dialogSubscriber.unsubscribe();
                }
            );
        } else {
            this.proceedRemoveImage(index);
        }
    }

    proceedRemoveImage(index: number) {
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
            this.imageInputSelectId = index;
            this.imageInput.nativeElement.click();
        }
    }

    handleFileInputButtonClick() {
        if (this.fileInput) {
            this.fileInput.nativeElement.click();
        }
    }

    removeInputFile(id: number) {
        const fileToDelete = this.filesToUpload.find(
            (value) => value.id === id
        );

        if (fileToDelete === null || fileToDelete === undefined) return;

        if (fileToDelete.fileUrl) {
            const msg = ['This action will totally delete file.'];
            const dialogData: ChoicePopupData = {
                isError: true,
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
                            .deleteLotFile(this.lotId, fileToDelete.fileUrl!)
                            .subscribe((res) => {
                                this.filesToUpload = this.filesToUpload.filter(
                                    (val) => val.id !== id
                                );

                                deleteLotSubscriber.unsubscribe();
                            });
                    }

                    dialogSubscriber.unsubscribe();
                }
            );
        } else {
            this.filesToUpload = this.filesToUpload.filter(
                (val) => val.id !== id
            );
        }
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

    openChoiceDialog(data: ChoicePopupData): DialogRef<string, unknown> {
        return this.choiceDialog.open<string>(ChoicePopupComponent, {
            data,
        });
    }

    deleteLot() {
        this.isDeleteLoading = true;
        const msg = ['This action will delete lot or cancel it.'];
        const dialogData: ChoicePopupData = {
            isError: true,
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
                                this.snackBar.open(res);
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
}
