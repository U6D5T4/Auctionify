import { Component, Input, OnInit } from '@angular/core';
import { Client, TransactionModel } from 'src/app/web-api-client';

@Component({
    selector: 'app-transactions',
    templateUrl: './transactions.component.html',
    styleUrls: ['./transactions.component.scss'],
})
export class TransactionsComponent implements OnInit {
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
}
