import { DIALOG_DATA, DialogRef } from '@angular/cdk/dialog';
import { Component, EventEmitter, Inject, Output } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { AppLocation, Category, Client, Status } from 'src/app/web-api-client';

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

    chekboxValues = new FormGroup({
        Closed: new FormControl<boolean>(false),
        Active: new FormControl<boolean>(false),
        Upcoming: new FormControl<boolean>(false),
    });

    categories: Category[] = [];
    lotStatuses: Status[] = [];
    locations: AppLocation[] = [];

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
        this.populateLocations();
        this.populateLotStatuses();
    }

    filterClick() {}

    populateCategorySelector() {
        this.client.getAllCategories().subscribe({
            next: (result: Category[]) => {
                this.categories = result;
            },
        });
    }

    populateLotStatuses() {
        this.client.getAllLotStatuses().subscribe({
            next: (result: Status[]) => {
                this.lotStatuses = result;
            },
        });
    }

    populateLocations() {
        this.client.getAllLocations().subscribe({
            next: (result: AppLocation[]) => {
                this.locations = result;
            },
        });
    }
}
