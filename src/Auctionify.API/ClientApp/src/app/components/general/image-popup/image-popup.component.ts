import { DIALOG_DATA, DialogRef } from '@angular/cdk/dialog';
import { Component, Inject } from '@angular/core';

@Component({
    selector: 'app-image-popup',
    templateUrl: './image-popup.component.html',
    styleUrls: ['./image-popup.component.scss'],
})
export class ImagePopupComponent {
    imageUrl_: string = '';
    constructor(
        public dialogRef: DialogRef<string>,
        @Inject(DIALOG_DATA) private imageUrl: string
    ) {
        this.imageUrl_ = imageUrl;
    }
}
