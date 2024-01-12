import { Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root',
})
export class DateCalculationService {
    constructor() {}

    calculateDaysLeftActive(
        startDate: Date | null,
        endDate: Date | null
    ): number | null {
        if (!startDate || !endDate) {
            return null;
        }

        const start = new Date(startDate);
        const end = new Date(endDate);
        const timeDifference = end.getTime() - start.getTime();
        return Math.ceil(timeDifference / (1000 * 3600 * 24));
    }

    calculateDaysLeftUpcoming(startDate: Date | null): number | null {
        if (!startDate) {
            return null;
        }

        const now = new Date();
        const start = new Date(startDate);
        if (start <= now) {
            return 0;
        }

        const timeDifference = start.getTime() - now.getTime();
        return Math.ceil(timeDifference / (1000 * 3600 * 24));
    }
}
