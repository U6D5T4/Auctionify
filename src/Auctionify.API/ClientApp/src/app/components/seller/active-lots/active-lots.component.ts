import { Component, OnInit } from '@angular/core';
import {
    Client,
    FilterResponse,
    FilteredLotModel,
    PageRequest,
} from 'src/app/web-api-client';

@Component({
    selector: 'app-active-lots',
    templateUrl: './active-lots.component.html',
    styleUrls: ['./active-lots.component.scss'],
})
export class ActiveLotsComponent implements OnInit {
    activeLots: FilteredLotModel[] = [];
    filterResponse!: FilterResponse;

    constructor(private apiClient: Client) {}

    ngOnInit(): void {
        const pageRequest: PageRequest = {
            PageIndex: 0,
            PageSize: 500,
        };
        this.apiClient.getAllActiveLotsForSeller(pageRequest).subscribe({
            next: (result) => {
                this.filterResponse = result;
                this.activeLots = result.items;
            },
        });
    }
}
