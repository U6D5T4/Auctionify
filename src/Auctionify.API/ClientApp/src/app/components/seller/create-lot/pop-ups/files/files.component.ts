import { DialogRef, DIALOG_DATA, Dialog } from '@angular/cdk/dialog';
import { Component, ElementRef, Inject, ViewChild } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { LotFormModel } from '../../create-lot.component';
import { FileModel } from 'src/app/models/fileModel';
import { DialogPopupComponent } from 'src/app/ui-elements/dialog-popup/dialog-popup.component';

@Component({
    selector: 'app-files',
    templateUrl: './files.component.html',
    styleUrls: ['./files.component.scss'],
})
export class FilesPopUpComponent {
    filesFormGroup: FormGroup<LotFormModel>;

    @ViewChild('fileInput')
    fileInput!: ElementRef;

    constructor(
        public dialogRef: DialogRef<string>,
        @Inject(DIALOG_DATA) public data: FormGroup<LotFormModel>,
        private dialog: Dialog
    ) {
        this.filesFormGroup = data;
    }

    removeInputFile(name: string) {
        this.filesFormGroup.controls.files.setValue(
            this.filesFormGroup.controls.files.value?.filter(
                (val) => val.name !== name
            )!
        );
    }

    fileUpdateEvent(event: Event) {
        if (event.target === null) return;
        const files = (event.target as HTMLInputElement).files;

        if (!this.filesAmountCondition()) {
            return;
        }

        if (files === null) return;
        for (let i = 0; i < files.length; i++) {
            if (!this.filesAmountCondition()) {
                break;
            }
            const element = files.item(i);

            if (element === null) return;

            const file: FileModel = {
                name: element.name!,
                file: element,
            };

            this.filesFormGroup.controls.files.value?.push(file);
        }
    }

    filesAmountCondition(): boolean {
        if (this.filesFormGroup.controls.files.value?.length! >= 3) {
            let errorMessages = [];
            errorMessages.push('You can add only 3 files to your lot!');
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
}
