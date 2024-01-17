import { Injectable } from '@angular/core';
import { lastValueFrom } from 'rxjs';
import { IHttpConnectionOptions } from '@microsoft/signalr';
import * as signalR from '@microsoft/signalr';

import { SignalRActions } from './signalr-actions';
import { AuthorizeService } from 'src/app/api-authorization/authorize.service';

@Injectable({
    providedIn: 'root',
})
export class SignalRService {
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
            .withUrl('hubs/auction-hub', options)
            .withAutomaticReconnect()
            .build();

        this.connectionEstablished = this.startConnection();
    }

    private async startConnection(): Promise<void> {
        try {
            await this.connection.start();
        } catch (err: any) {
            console.error(err.toString());
        }
    }

    public async joinLotGroupAfterConnection(lotId: number) {
        await this.connectionEstablished;
        this.connection
            .invoke('JoinLotGroup', lotId)
            .then(() => {})
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
            .then(() => {})
            .catch((err) => {
                console.error(
                    `Error leaving group for Lot ID: ${lotId}`,
                    err.toString()
                );
            });
    }

    public onReceiveBidNotification(callback: () => void, lotId: number) {
        this.connection.on(SignalRActions.ReceiveBidNotification, () => {
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
                callback();
            }
        );
    }
}
