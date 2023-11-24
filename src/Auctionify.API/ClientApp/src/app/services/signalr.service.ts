import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from 'src/environments/environment';

@Injectable({
    providedIn: 'root',
})
export class SignalRService {
    private apiUrl = environment.apiUrl;

    private connection: signalR.HubConnection;

    constructor() {
        this.connection = new signalR.HubConnectionBuilder()
            .configureLogging(signalR.LogLevel.Information)
            .withUrl(this.apiUrl + 'api/auctionHub')
            .build();

        this.startConnection();
    }

    private startConnection() {
        this.connection
            .start()
            .then(() => {
                console.log('SignalR connected');
            })
            .catch((err) => {
                console.error(err.toString());
            });
    }

    public onReceiveBidNotification(callback: () => void) {
        this.connection.on('ReceiveBidNotification', () => {
            console.log('Bid received');
            callback();
        });
    }
}
