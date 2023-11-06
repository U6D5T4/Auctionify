import { Component, ElementRef, Injectable, ViewChild } from '@angular/core';
import { FileModel } from 'src/app/models/fileModel';
import { Category, Client } from 'src/app/web-api-client';

@Injectable({
  providedIn: 'root',
})
@Component({
  selector: 'app-create-lot',
  templateUrl: './create-lot.component.html',
  styleUrls: ['./create-lot.component.scss'],
})
export class CreateLotComponent {
  private imageInputsButtonSize: number = 4;
  inputButtons: number[] = [...Array(this.imageInputsButtonSize).keys()];
  currentFileButtonId: number | undefined;

  @ViewChild('imageInput')
  imageInput!: ElementRef;

  @ViewChild('fileInput')
  fileInput!: ElementRef;

  imagesToUpload: File[] = [];
  filesToUpload: FileModel[] = [];

  categories: Category[] = [];

  constructor(private client: Client) {
    this.populateCategorySelector();
  }

  imageUpdateEvent(event: Event) {
    if (event.target === null) return;
    const files = (event.target as HTMLInputElement).files;

    if (files === null) return;
    for (let index = 0; index < files.length; index++) {
      const element: File = files[index];
      const reader = new FileReader();

      // reader.read;
    }
  }

  fileUpdateEvent(event: Event) {
    if (event.target === null) return;

    const files = (event.target as HTMLInputElement).files;

    if (files === null) return;
    const file: FileModel = {
      id: this.filesToUpload.length + 1,
      file: files[0],
    };

    this.filesToUpload.push(file);
  }

  handleInputButtonClick(buttonId: any) {
    if (this.imageInput) {
      this.currentFileButtonId = buttonId;
      this.imageInput.nativeElement.click();
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
      next: (result: Category[]) => {
        this.categories = result;
        console.log(this.categories);
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
}
