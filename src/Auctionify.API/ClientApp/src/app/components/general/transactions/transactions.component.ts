import { Component, Input, OnInit } from '@angular/core';
import { Client, TransactionModel } from 'src/app/web-api-client';

enum TransactionStatus {
    Withdraw = 'Withdraw',
    Cancelled = 'Cancelled',
    Winner = 'Winner',
    Sold = 'Sold',
    Loss = 'Loss',
    Expired = 'Expired',
}

@Component({
    selector: 'app-transactions',
    templateUrl: './transactions.component.html',
    styleUrls: ['./transactions.component.scss'],
})
export class TransactionsComponent implements OnInit {
    private readonly MAIN_GRAY_COLOR: string = '#828282';
    private readonly MAIN_RED_COLOR: string = '#ab2d25';
    private readonly MAIN_GREEN_COLOR: string = '#2b9355';
    private readonly DEFAULT_BLACK_COLOR: string = '#000000';

    @Input() transactionsCount: number = 100; // default to 100
    @Input() showHeader: boolean = true; // Default to true

    transactions: TransactionModel[] = [];

    constructor(private apiClient: Client) {}

    ngOnInit() {
        this.loadUserTransactions();
    }

    loadUserTransactions() {
        this.apiClient
            .getUserTransactions(0, this.transactionsCount)
            .subscribe((response: any) => {
                this.transactions = response.items;
            });
    }

    getStatusColor(status: TransactionStatus | string | null): string {
        if (status === null) {
            return this.DEFAULT_BLACK_COLOR;
        }

        if (typeof status === 'string') {
            status = status as TransactionStatus;
        }

        switch (status) {
            case TransactionStatus.Withdraw:
            case TransactionStatus.Cancelled:
                return this.MAIN_GRAY_COLOR;
            case TransactionStatus.Winner:
            case TransactionStatus.Sold:
                return this.MAIN_GREEN_COLOR;
            case TransactionStatus.Loss:
            case TransactionStatus.Expired:
                return this.MAIN_RED_COLOR;
            default:
                return this.DEFAULT_BLACK_COLOR;
        }
    }
}
