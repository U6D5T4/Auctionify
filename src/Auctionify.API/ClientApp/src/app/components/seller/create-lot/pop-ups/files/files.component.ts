import { DialogRef, DIALOG_DATA, Dialog } from '@angular/cdk/dialog';
import { Component, ElementRef, Inject, ViewChild } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { LotFormModel } from '../../create-lot.component';
import { FileModel } from 'src/app/models/fileModel';
import { DialogPopupComponent } from 'src/app/ui-elements/dialog-popup/dialog-popup.component';
import { Client } from 'src/app/web-api-client';
import {
    ChoicePopupComponent,
    ChoicePopupData,
} from 'src/app/ui-elements/choice-popup/choice-popup.component';

export interface FilesDialogData {
    formGroup: FormGroup<LotFormModel>;
    lotId: number;
}

@Component({
    selector: 'app-files',
    templateUrl: './files.component.html',
    styleUrls: ['./files.component.scss'],
})
export class FilesPopUpComponent {
    filesFormGroup: FormGroup<LotFormModel>;

    @ViewChild('fileInput')
    fileInput!: ElementRef;

    private lotId: number;

    constructor(
        public dialogRef: DialogRef<string>,
        @Inject(DIALOG_DATA) public data: FilesDialogData,
        private dialog: Dialog,
        private client: Client
    ) {
        this.filesFormGroup = data.formGroup;
        this.lotId = data.lotId;
    }

    removeInputFile(name: string) {
        const file = this.filesFormGroup.value.files?.find(
            (x) => x.name === name
        );

        if (file?.fileUrl) {
            const msg = ['This action will totally delete file from system.'];
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
                            .deleteLotFile(this.lotId, file.fileUrl!)
                            .subscribe((res) => {
                                this.filesFormGroup.controls.files.setValue(
                                    this.filesFormGroup.value.files?.filter(
                                        (x) => x.name !== file.name
                                    )!
                                );
                                deleteLotSubscriber.unsubscribe();
                            });
                    }

                    dialogSubscriber.unsubscribe();
                }
            );
        } else {
            this.filesFormGroup.controls.files.setValue(
                this.filesFormGroup.controls.files.value?.filter(
                    (val) => val.name !== name
                )!
            );
        }
    }

    fileUpdateEvent(event: Event) {
        if (event.target === null) return;
        const files = (event.target as HTMLInputElement).files;

        if (!this.filesAmountCondition()) {
            return;
        }

        if (files === null) return;
        let existingFilesNames: string[] = [];

        for (let i = 0; i < files.length; i++) {
            if (!this.filesAmountCondition()) {
                break;
            }

            const element = files.item(i);
            if (element === null) return;

            const existingFile = this.filesFormGroup.value.files?.find(
                (x) => x.name === element.name
            );

            if (existingFile) {
                existingFilesNames.push(existingFile.name!);
            } else {
                const file: FileModel = {
                    name: element.name!,
                    file: element,
                    fileUrl: null,
                };

                this.filesFormGroup.controls.files.value?.push(file);
            }
        }

        if (existingFilesNames.length > 0) {
            this.openDialog(
                [
                    "These files already exist and won't be added again:",
                    ...existingFilesNames,
                ],
                true,
                false
            );
        }
    }

    filesAmountCondition(): boolean {
        if (this.filesFormGroup.controls.files.value?.length! >= 3) {
            let errorMessages = [];
            errorMessages.push('You can only add 3 files!');
            const dialog = this.openDialog(errorMessages, true, false);
            return false;
        }

        return true;
    }

    handleFileInputButtonClick() {
        if (this.fileInput) {
            this.fileInput.nativeElement.click();
        }
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

    openChoiceDialog(data: ChoicePopupData): DialogRef<string, unknown> {
        return this.dialog.open<string>(ChoicePopupComponent, {
            data,
        });
    }

    closePopup() {
        this.dialogRef.close();
    }
}
