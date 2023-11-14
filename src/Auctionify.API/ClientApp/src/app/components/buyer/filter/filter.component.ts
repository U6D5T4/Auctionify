import { DIALOG_DATA, DialogRef } from '@angular/cdk/dialog';
import { Component, Inject } from '@angular/core';

@Component({
    selector: 'app-filter',
    templateUrl: './filter.component.html',
    styleUrls: ['./filter.component.scss'],
})
export class FilterComponent {
    constructor(
        public dialogRef: DialogRef<string>,
        @Inject(DIALOG_DATA) public data: any
    ) {}
}
