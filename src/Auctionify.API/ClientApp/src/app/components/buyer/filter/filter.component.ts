import { DIALOG_DATA, DialogRef } from '@angular/cdk/dialog';
import { Component, EventEmitter, Inject, Output } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { Category, Client } from 'src/app/web-api-client';

export interface FilterParameters {
    minimumPrice: FormControl<number | null>;
    maximumPrice: FormControl<number | null>;
    categoryId: FormControl<number | null>;
    location: FormControl<string | null>;
    lotStatuses: FormControl<number[] | null>;
    sortField: FormControl<string | null>;
    sortDir: FormControl<string | null>;
}

@Component({
    selector: 'app-filter',
    templateUrl: './filter.component.html',
    styleUrls: ['./filter.component.scss'],
})
export class FilterComponent {
    @Output() filterUpdatedEvent = new EventEmitter<FilterParameters>();

    categories: Category[] = [];

    filterForm = new FormGroup<FilterParameters>({
        minimumPrice: new FormControl<number | null>(null),
        maximumPrice: new FormControl<number | null>(null),
        categoryId: new FormControl<number | null>(null),
        location: new FormControl<string | null>(null),
        lotStatuses: new FormControl<number[] | null>([]),
        sortField: new FormControl<string | null>(''),
        sortDir: new FormControl<string | null>(''),
    });

    constructor(
        public dialogRef: DialogRef<string>,
        @Inject(DIALOG_DATA) public data: any,
        private client: Client
    ) {
        this.populateCategorySelector();
    }

    filterClick() {
        console.log(this.filterForm.value);
    }

    populateCategorySelector() {
        this.client.getAllCategories().subscribe({
            next: (result: Category[]) => {
                this.categories = result;
            },
        });
    }
}
