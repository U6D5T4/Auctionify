import {
    ChangeDetectorRef,
    Component,
    ElementRef,
    OnInit,
    ViewChild,
} from '@angular/core';
import {
    Client,
    FilterResponse,
    FilteredLotModel,
    LotModel,
    PageRequest,
} from 'src/app/web-api-client';
import { SwiperContainer } from 'swiper/element';

@Component({
    selector: 'app-dashboard-active-lots',
    templateUrl: './dashboard-active-lots.component.html',
    styleUrls: ['./dashboard-active-lots.component.scss'],
})
export class DashboardActiveLotsComponent implements OnInit {
    activeLots: FilteredLotModel[] = [];
    filterResponse!: FilterResponse;

    constructor(private apiClient: Client) {}

    ngOnInit(): void {
        const pageRequest: PageRequest = {
            PageIndex: 0,
            PageSize: 100,
        };
        this.apiClient.getAllActiveLotsForSeller(pageRequest).subscribe({
            next: (result) => {
                this.filterResponse = result;
                this.activeLots = result.items;
            },
        });
    }
}
