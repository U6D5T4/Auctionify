import { Component, Inject, Input, LOCALE_ID } from '@angular/core';
import { formatDate } from '@angular/common';

import { Rate } from 'src/app/models/rates/rate-models';
import { Router } from '@angular/router';

@Component({
    selector: 'app-rate-item',
    templateUrl: './rate-item.component.html',
    styleUrls: ['./rate-item.component.scss'],
})
export class RateItemComponent {
    @Input()
    rate!: Rate;
    @Input()
    IsSentRates!: boolean;

    constructor(
        private router: Router,
        @Inject(LOCALE_ID) public locale: string
    ) {}

    onClick() {
        if (this.IsSentRates) {
            this.router.navigate([`profile/user/${this.rate.senderId}`]);
        } else {
            this.router.navigate([`profile/user/${this.rate.receiverId}`]);
        }
    }

    formatDate(date: Date | null): string {
        return date ? formatDate(date, 'dd LLLL, HH:mm (z)', this.locale) : '';
    }

    getStars(count: number): string[] {
        const stars: string[] = [];
        for (let i = 1; i <= 5; i++) {
            if (i <= count) {
                stars.push('star');
            } else {
                stars.push('star_border');
            }
        }
        return stars;
    }
}
