import { Component } from '@angular/core';
import { LotModel } from 'src/app/web-api-client';

@Component({
    selector: 'app-dashboard-active-lots',
    templateUrl: './dashboard-active-lots.component.html',
    styleUrls: ['./dashboard-active-lots.component.scss'],
})
export class DashboardActiveLotsComponent {
    sliders: string[] = [
        'Test 1',
        'Test 2',
        'Test 3',
        'Test 4',
        'Test 5',
        'Test 6',
        'Test 7',
        'Test 8',
        'Test 9',
    ];

    lot: LotModel = {
        isInWatchlist: false,
        id: 1,
        title: 'Sample Lot 1',
        description: 'This is a sample lot description for Lot 1.',
        startingPrice: 100,
        startDate: new Date('2023-10-12T10:00:00'),
        endDate: new Date('2023-10-15T15:00:00'),
        category: {
            id: 1,
            name: 'Electronics',
            parentCategoryId: null,
        },
        lotStatus: {
            id: 5,
            name: 'Active',
        },
        location: {
            id: 1,
            city: 'New York',
            state: 'NY',
            country: 'USA',
            address: '1234 Elm St',
        },
        currency: {
            id: 1,
            code: 'USD',
        },
        bids: [
            {
                id: 3,
                buyerId: 2,
                buyer: null!,
                newPrice: 500,
                timeStamp: new Date('2023-12-12T18:03:28.5757795'),
                bidRemoved: false,
                currency: null!,
            },
            {
                id: 2,
                buyerId: 2,
                buyer: null!,
                newPrice: 300,
                timeStamp: new Date('2023-12-12T18:03:20.5198332'),
                bidRemoved: false,
                currency: null!,
            },
            {
                id: 1,
                buyerId: 2,
                buyer: null!,
                newPrice: 150,
                timeStamp: new Date('2023-12-12T18:03:14.7389335'),
                currency: null!,
                bidRemoved: false,
            },
        ],
        bidCount: 3,
        mainPhotoUrl: null,
    };
}
