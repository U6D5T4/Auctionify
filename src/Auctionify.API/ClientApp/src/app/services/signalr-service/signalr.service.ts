import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';

import { environment } from 'src/environments/environment';
import { SignalRActions } from './signalr-actions';
import { AuthorizeService } from '../../api-authorization/authorize.service';
import { mergeMap } from 'rxjs';

@Injectable({
    providedIn: 'root',
})
export class SignalRService {
    private apiUrl = environment.apiUrl;
    private connection: signalR.HubConnection;
    private connectionEstablished: Promise<void>;

    constructor(AuthorizeService: AuthorizeService) {
        AuthorizeService.getAccessToken().subscribe((token) => {
            if (token != null) {
                this.connection = new signalR.HubConnectionBuilder()
                    .configureLogging(signalR.LogLevel.Information)
                    .withUrl(this.apiUrl + 'api/auctionHub', {
                        accessTokenFactory: () => {
                            return token;
                        },
                    })
                    .withAutomaticReconnect()
                    .build();

                this.connectionEstablished = this.startConnection();
            }
        });

        this.connection = new signalR.HubConnectionBuilder()
            .configureLogging(signalR.LogLevel.Information)
            .withUrl(this.apiUrl + 'api/auctionHub')
            .withAutomaticReconnect()
            .build();

        this.connectionEstablished = this.startConnection();
    }

    private async startConnection(): Promise<void> {
        try {
            await this.connection.start();
            console.log('SignalR connected');
        } catch (err: any) {
            console.error(err.toString());
        }
    }

    public async joinLotGroupAfterConnection(lotId: number) {
        await this.connectionEstablished;
        this.connection
            .invoke('JoinLotGroup', lotId)
            .then(() => {
                console.log(`Joined group for Lot ID: ${lotId}`);
            })
            .catch((err) => {
                console.error(
                    `Error joining group for Lot ID: ${lotId}`,
                    err.toString()
                );
            });
    }

    public onReceiveBidNotification(callback: () => void, lotId: number) {
        this.connection.on(SignalRActions.ReceiveBidNotification, () => {
            console.log(`Bid received for Lot ID: ${lotId}`);
            callback();
        });
    }

    public onReceiveWithdrawBidNotification(
        callback: () => void,
        lotId: number
    ) {
        this.connection.on(
            SignalRActions.ReceiveWithdrawBidNotification,
            () => {
                console.log(`Withdraw bid received for Lot ID: ${lotId}`);
                callback();
            }
        );
    }
}
