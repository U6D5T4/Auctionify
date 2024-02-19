import { Dialog } from '@angular/cdk/dialog';
import { Component, HostListener, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

import { Client } from 'src/app/web-api-client';
import { DialogPopupComponent } from 'src/app/ui-elements/dialog-popup/dialog-popup.component';
import { GetUserById } from 'src/app/models/users/user-models';
import { Rate } from 'src/app/models/rates/rate-models';

@Component({
    selector: 'app-public-user-rating',
    templateUrl: './public-user-rating.component.html',
    styleUrls: ['./public-user-rating.component.scss'],
})
export class PublicUserRatingComponent implements OnInit {
    userProfileData: GetUserById | null = null;
    currentUserId: number = 0;
    averageRatingWidth: number = 700;
    IsSentRates: boolean = true;
    userRates: Rate[] | null = null;

    constructor(
        private client: Client,
        private activeRoute: ActivatedRoute,
        private router: Router,
        private dialog: Dialog
    ) {}

    ngOnInit(): void {
        this.fetchUserProfileData();
        this.getPublicUserRates();
    }

    getPublicUserRates() {
        const userId = this.activeRoute.snapshot.params['id'];

        this.client.getUserRates(userId).subscribe({
            next: (data) => {
                this.userRates = data;
                console.log(this.userRates);
            },
            error: (error) =>
                this.openDialog(
                    error.errors! || [
                        'Something went wrong, please try again later',
                    ],
                    true
                ),
        });
    }

    fetchUserProfileData() {
        const userId = this.activeRoute.snapshot.params['id'];

        if (this.currentUserId == userId) {
            this.router.navigate(['profile']);
        }

        this.client.getUserById(userId).subscribe({
            next: (data: GetUserById) => {
                this.userProfileData = data;
            },
            error: (error) =>
                this.openDialog(
                    error.errors! || [
                        'Something went wrong, please try again later',
                    ],
                    true
                ),
        });
    }

    openDialog(text: string[], error: boolean) {
        const dialogRef = this.dialog.open<string>(DialogPopupComponent, {
            data: {
                text,
                isError: error,
            },
        });

        dialogRef.closed.subscribe((res) => {});
    }

    getAverageStars(rate: number | null): string[] {
        const averageRating = rate;

        const roundedAverage = Math.round(averageRating!);

        const stars: string[] = [];
        for (let i = 1; i <= 5; i++) {
            if (i <= roundedAverage) {
                stars.push('star');
            } else if (i - roundedAverage === 0.5) {
                stars.push('star_half');
            } else {
                stars.push('star_border');
            }
        }

        return stars;
    }
}
