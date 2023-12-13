import { DIALOG_DATA, DialogRef } from '@angular/cdk/dialog';
import { Component, Inject, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { Observable, forkJoin, map } from 'rxjs';

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
export class FilterComponent implements OnInit {
    checkboxValues = new FormGroup({
        Closed: new FormControl<boolean>(false),
        Active: new FormControl<boolean>(false),
        Upcoming: new FormControl<boolean>(false),
    });

    categories: Category[] = [];
    lotStatuses: Status[] = [];
    locations: AppLocation[] = [];
    highestLotPrice: number = 0;

    filterForm = new FormGroup<FilterParameters>({
        minimumPrice: new FormControl<number | null>(null),
        maximumPrice: new FormControl<number | null>(null),
        categoryId: new FormControl<number | null>(null),
        location: new FormControl<string | null>(null),
        lotStatuses: new FormControl<number[] | null>([]),
    });

    constructor(
        public dialogRef: DialogRef<string>,
        @Inject(DIALOG_DATA) public data: FilterResult | null,
        private client: Client
    ) {}

    ngOnInit(): void {
        const observablesTasks = forkJoin({
            category: this.populateCategorySelector(),
            location: this.populateLocations(),
            statuses: this.populateLotStatuses(),
            highestlotPrice: this.populateHighestLotPrice(),
        });

        observablesTasks.subscribe({
            next: () => {
                this.assignExistingFilterResult();
            },
        });
    }

    assignExistingFilterResult() {
        if (this.data) {
            this.filterForm.controls.minimumPrice.setValue(
                this.data.minimumPrice
            );
            this.filterForm.controls.maximumPrice.setValue(
                this.data.maximumPrice
            );
            this.filterForm.controls.categoryId.setValue(this.data.categoryId);
            this.filterForm.controls.location.setValue(this.data.location);

            if (this.data.lotStatuses) {
                const data = Object.entries(this.checkboxValues.value);

                for (const [key, value] of data) {
                    let statusesToSearch: Status[];
                    if (key === 'Closed') {
                        statusesToSearch = this.lotStatuses.filter(
                            (x) =>
                                x.name === 'Sold' ||
                                x.name === 'NotSold' ||
                                x.name === 'Cancelled'
                        );
                    } else {
                        statusesToSearch = this.lotStatuses.filter(
                            (x) => x.name == key
                        );
                    }

                    const statusResults = this.data.lotStatuses.find((x) => {
                        const result = statusesToSearch.some((s) => s.id === x);
                        return result;
                    });

                    if (statusResults) {
                        this.checkboxValues.get(key)?.setValue(true);
                    }
                }
            }
        }
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

    populateCategorySelector(): Observable<boolean> {
        return this.client.getAllCategories().pipe(
            map((result: Category[]) => {
                this.categories = result;
                return true;
            })
        );
    }

    populateLotStatuses(): Observable<boolean> {
        return this.client.getAllLotStatuses().pipe(
            map((result: Status[]) => {
                this.lotStatuses = result;
                return true;
            })
        );
    }

    populateLocations(): Observable<boolean> {
        return this.client.getAllLocations().pipe(
            map((result: AppLocation[]) => {
                this.locations = result;
                return true;
            })
        );
    }

    populateHighestLotPrice(): Observable<boolean> {
        return this.client.getHighestLotPrice().pipe(
            map((result: number) => {
                this.highestLotPrice = result;
                return true;
            })
        );
    }

    clickClose() {
        this.dialogRef.close();
    }

    clickReset() {
        this.filterForm.reset();
        this.checkboxValues.reset();
    }
}
