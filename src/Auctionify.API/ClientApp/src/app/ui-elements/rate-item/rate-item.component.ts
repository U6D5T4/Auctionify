import { Component, Input } from '@angular/core';
import { formatDate } from '@angular/common';

import { Rate } from 'src/app/models/rates/rate-models';

@Component({
    selector: 'app-rate-item',
    templateUrl: './rate-item.component.html',
    styleUrls: ['./rate-item.component.scss'],
})
export class RateItemComponent {
    @Input()
    rate!: Rate;

    formatDate(date: Date | null): string {
        return date ? formatDate(date, 'dd LLLL, h:mm', 'en-US') : '';
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
