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

        const customIcon = L.icon({
            iconUrl: "../../../assets/icons/map-marker.png",
            iconSize: [30, 30],
            iconAnchor: [22, 94],
            popupAnchor: [-3, -76]
        });
    
        // Add a marker to the map with the custom icon
        L.marker([lat, lng], { icon: customIcon }).addTo(this.map);
    }
}
