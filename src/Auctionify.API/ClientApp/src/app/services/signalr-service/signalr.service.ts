import { Injectable } from '@angular/core';
import { lastValueFrom } from 'rxjs';
import { IHttpConnectionOptions } from '@microsoft/signalr';
import * as signalR from '@microsoft/signalr';

import { environment } from 'src/environments/environment';
import { SignalRActions } from './signalr-actions';
import { AuthorizeService } from 'src/app/api-authorization/authorize.service';

@Injectable({
    providedIn: 'root',
})
export class SignalRService {
    private apiUrl = environment.apiUrl;
    private connection: signalR.HubConnection;
    private connectionEstablished: Promise<void>;

    constructor(authorizeService: AuthorizeService) {
        const tokenPromise = lastValueFrom(authorizeService.getAccessToken());

        const options: IHttpConnectionOptions = {
            accessTokenFactory: async () => {
                const token = await tokenPromise;
                return token ?? ''; // If token is null or undefined, default to an empty string
            },
        };

        this.connection = new signalR.HubConnectionBuilder()
            .configureLogging(signalR.LogLevel.Information)
            .withUrl(this.apiUrl + 'api/hubs/auctionHub', options)
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

    public async leaveLotGroup(lotId: number) {
        await this.connectionEstablished;
        this.connection
            .invoke('LeaveLotGroup', lotId)
            .then(() => {
                console.log(`Left group for Lot ID: ${lotId}`);
            })
            .catch((err) => {
                console.error(
                    `Error leaving group for Lot ID: ${lotId}`,
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
