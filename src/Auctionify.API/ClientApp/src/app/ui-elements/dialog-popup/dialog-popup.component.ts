import { Component, Inject } from '@angular/core';
import { DIALOG_DATA, DialogRef } from '@angular/cdk/dialog';

export interface DialogData {
  text: string[];
  isError: boolean;
}

@Component({
  selector: 'app-dialog-popup',
  templateUrl: './dialog-popup.component.html',
  styleUrls: ['./dialog-popup.component.scss']
})
export class DialogPopupComponent {
  iconPath: string = "";
  message: string = "";
  constructor(public dialogRef: DialogRef<string>, @Inject(DIALOG_DATA) public data: DialogData) {
    this.message = data.text[0];
    this.iconPath = data.isError ?
    "../../../assets/icons/alert-hexagon.svg" :
    "../../../assets/icons/warning_triangle.svg";
  }

  closeDialog() {
    this.dialogRef.close();
  }
}
