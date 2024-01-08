import { Component, OnInit } from '@angular/core';
import {
    Client,
    FilterResponse,
    FilteredLotModel,
    PageRequest,
} from 'src/app/web-api-client';

@Component({
    selector: 'app-dashboard-draft-lots',
    templateUrl: './dashboard-draft-lots.component.html',
    styleUrls: ['./dashboard-draft-lots.component.scss'],
})
export class DashboardDraftLotsComponent implements OnInit {
    draftLots: FilteredLotModel[] = [];
    filterResponse!: FilterResponse;

    constructor(private apiClient: Client) {}

    ngOnInit(): void {
        const pageRequest: PageRequest = {
            PageIndex: 0,
            PageSize: 100,
        };
        this.apiClient.getAllDraftLotsForSeller(pageRequest).subscribe({
            next: (result) => {
                this.filterResponse = result;
                this.draftLots = result.items;
            },
        });
    }
}
