import { Component, ElementRef, Injectable, ViewChild } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';

@Injectable({
  providedIn: 'root',
})
@Component({
  selector: 'app-create-lot',
  templateUrl: './create-lot.component.html',
  styleUrls: ['./create-lot.component.scss'],
})
export class CreateLotComponent {
  private fileInputsButtonSize: number = 4;
  inputButtons: number[] = [...Array(this.fileInputsButtonSize).keys()];
  currentFileButtonId: number | undefined;

  @ViewChild('fileInput')
  fileInput!: ElementRef;

  filesToUpload!: File[];

  constructor(private sanitizer: DomSanitizer) {}

  fileUpdateEvent(event: Event) {
    if (event.target === null) return;
    const files = (event.target as HTMLInputElement).files;

    if (files === null) return;
    for (let index = 0; index < files.length; index++) {
      const element: File = files[index];
      const reader = new FileReader();

      // reader.read;
    }
  }

  handleInputButtonClick(buttonId: any) {
    console.log(buttonId);
    if (this.fileInput) {
      this.currentFileButtonId = buttonId;
      this.fileInput.nativeElement.click();
    }
  }
}
