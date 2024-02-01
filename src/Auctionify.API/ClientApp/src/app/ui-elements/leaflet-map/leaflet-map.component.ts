import {
    Component,
    Input,
    AfterViewInit,
    OnDestroy,
    ViewChild,
    ElementRef,
} from '@angular/core';
import * as L from 'leaflet';

@Component({
    selector: 'app-leaflet-map',
    templateUrl: './leaflet-map.component.html',
    styleUrls: ['./leaflet-map.component.scss'],
})
export class LeafletMapComponent implements AfterViewInit, OnDestroy {
    @Input() latitude!: string;
    @Input() longitude!: string;
    @ViewChild('map') mapContainer!: ElementRef;

    private map!: L.Map;

    ngAfterViewInit() {
        this.initMap();
    }

    ngOnDestroy() {
        if (this.map) {
            this.map.remove();
        }
    }

    private initMap(): void {
        if (!this.mapContainer) return;

        const lat = parseFloat(this.latitude);
        const lng = parseFloat(this.longitude);

        if (isNaN(lat) || isNaN(lng)) {
            return;
        }

        this.map = L.map(this.mapContainer.nativeElement, {
            attributionControl: false,
            center: [lat, lng],
            zoom: 15,
        });

        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            maxZoom: 19,
        }).addTo(this.map);
    }
}
