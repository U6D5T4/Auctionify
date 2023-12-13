import {
    HttpErrorResponse,
    HttpResponse,
    HttpStatusCode,
} from '@angular/common/http';
import { Component, ViewChild, ElementRef } from '@angular/core';
import { FormControl } from '@angular/forms';
import {
    MatAutocomplete,
    MatAutocompleteSelectedEvent,
    MatAutocompleteTrigger,
} from '@angular/material/autocomplete';
import { Router } from '@angular/router';
import { forkJoin } from 'rxjs';
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
    isSearching = false;
    isResult = true;

    @ViewChild('searchInputTrigger')
    searchInputTrigger!: MatAutocompleteTrigger;

    constructor(private client: Client, private router: Router) {}

    search(
        event: Event,
        trigger: MatAutocompleteTrigger,
        panel: MatAutocomplete
    ) {
        event.preventDefault();
        if (!this.searchInput.value) return;
        this.isSearching = true;
        this.resetSearchResults();

        event.stopPropagation();
        if (!panel.isOpen) {
            trigger.openPanel();
        }

        const observables = forkJoin({
            nameResults: this.client.searchLotsByName(this.searchInput.value!),
            locationResults: this.client.searchLotsByLocation(
                this.searchInput.value!
            ),
        });

        observables.subscribe({
            next: (result) => {
                if (
                    result.nameResults.length == 0 &&
                    result.locationResults.length == 0
                ) {
                    this.isResult = false;
                    this.resetSearchResults();
                } else {
                    this.searchNameResult = result.nameResults;
                    this.searchLocationResult = result.locationResults;
                    this.isResult = true;
                }

                this.isSearching = false;
            },
            error: (err: any) => {
                if (err instanceof HttpErrorResponse) {
                    if (err.status === HttpStatusCode.Unauthorized) {
                        event.stopPropagation();
                        trigger.closePanel();
                        this.searchInput.setValue('');
                        this.isSearching = false;
                    }
                }
            },
        });
    }

    textChange() {
        this.resetSearchResults();
        this.isSearching = false;
        this.isResult = true;
    }

    resetSearchResults() {
        this.searchNameResult = [];
        this.searchLocationResult = [];
    }
}
