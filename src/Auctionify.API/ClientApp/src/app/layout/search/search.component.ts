import { Component, ViewChild, ElementRef } from '@angular/core';
import { FormControl } from '@angular/forms';
import {
    MatAutocomplete,
    MatAutocompleteTrigger,
} from '@angular/material/autocomplete';
import { Client, SearchLotResponse } from 'src/app/web-api-client';

@Component({
    selector: 'app-search',
    templateUrl: './search.component.html',
    styleUrls: ['./search.component.scss'],
})
export class SearchComponent {
    searchInput = new FormControl<string>('');
    searchNameResult: SearchLotResponse[] = [];
    searchLocationResult: SearchLotResponse[] = [];

    @ViewChild('searchInputTrigger')
    searchInputTrigger!: MatAutocompleteTrigger;

    constructor(private client: Client) {}

    search(event: Event, trigger: MatAutocompleteTrigger) {
        this.client.searchLotsByName(this.searchInput.value!).subscribe({
            next: (result) => {
                this.searchNameResult = result;
            },
        });

        this.client.searchLotsByLocation(this.searchInput.value!).subscribe({
            next: (result) => {
                this.searchLocationResult = result;
            },
        });

        event.stopPropagation();
        trigger.openPanel();
    }
}
