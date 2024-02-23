import { Injectable } from '@angular/core';
import { HttpClient } from '@microsoft/signalr';
import * as google from 'google-one-tap';
import { Observable, tap, mapTo } from 'rxjs';
import { Client } from 'src/app/web-api-client';

@Injectable({
    providedIn: 'root',
})
export class GoogleMapService {
    constructor(private client: Client) {}

    loadApi(): Observable<void> {
        return this.client.loadApiKeyFor().pipe(
            tap((apiKey) => {
                this.loadGoogleMapsScript(apiKey);
            }),
            mapTo(void 0)
        );
    }

    private loadGoogleMapsScript(apiKey: string) {
        const script = document.createElement('script');
        script.src = `https://maps.googleapis.com/maps/api/js?key=${apiKey}&libraries=places`;
        script.async = true;
        script.defer = true;
        document.head.appendChild(script);
    }
}
