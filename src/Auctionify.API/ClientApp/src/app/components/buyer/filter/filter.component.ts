import { DIALOG_DATA, DialogRef } from '@angular/cdk/dialog';
import { Component, EventEmitter, Inject, Output } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { AppLocation, Category, Client, Status } from 'src/app/web-api-client';

export interface FilterResult {
    minimumPrice: number | null;
    maximumPrice: number | null;
    categoryId: number | null;
    location: string | null;
    lotStatuses: number[] | null;
}

interface FilterParameters {
    minimumPrice: FormControl<number | null>;
    maximumPrice: FormControl<number | null>;
    categoryId: FormControl<number | null>;
    location: FormControl<string | null>;
    lotStatuses: FormControl<number[] | null>;
}

@Component({
    selector: 'app-filter',
    templateUrl: './filter.component.html',
    styleUrls: ['./filter.component.scss'],
})
export class FilterComponent {
    checkboxValues = new FormGroup({
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

    filterClick() {
        this.filterForm.value.lotStatuses = [];
        for (const [key, value] of Object.entries(this.checkboxValues.value)) {
            if (value) {
                if (key === 'Closed') {
                    const closedStatuses = this.lotStatuses.filter(
                        (x) =>
                            x.name === 'Sold' ||
                            x.name === 'NotSold' ||
                            x.name === 'Cancelled'
                    );
                    this.filterForm.value.lotStatuses.push(
                        ...closedStatuses.map((val) => val.id)
                    );
                } else {
                    const status = this.lotStatuses.find((x) => x.name == key);
                    this.filterForm.value.lotStatuses?.push(status?.id!);
                }
            }
        }

        const values = this.filterForm.value;
        const result: FilterResult = {
            minimumPrice: values.minimumPrice!,
            maximumPrice: values.maximumPrice!,
            categoryId: values.categoryId!,
            location: values.location!,
            lotStatuses: values.lotStatuses!,
        };

        const data = JSON.stringify(result);
        this.dialogRef.close(data);
    }

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
