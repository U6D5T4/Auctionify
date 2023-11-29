import { Component, OnInit, Inject } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

import { SignalRService } from 'src/app/services/signalr.service';
import { BidDto, Client } from 'src/app/web-api-client';

@Component({
    selector: 'app-add-bid',
    templateUrl: './add-bid.component.html',
    styleUrls: ['./add-bid.component.scss'],
})
export class AddBidComponent implements OnInit {
    bidForm!: FormGroup;
    lotId!: number;
    bids: BidDto[] = [];
    errorMsg!: string;

    constructor(
        private dialogRef: MatDialogRef<AddBidComponent>,
        private apiClient: Client,
        private signalRService: SignalRService,
        @Inject(MAT_DIALOG_DATA) private data: any
    ) {
        this.lotId = this.data?.lotId;
    }

    ngOnInit() {
        this.bidForm = new FormGroup({
            bid: new FormControl(null, Validators.required),
        });

        this.getAllBidsForLot();

        this.signalRService.onReceiveBidNotification(() => {
            this.getAllBidsForLot();
        });
    }

    getAllBidsForLot() {
        this.apiClient.getAllBidsOfUserForLot(this.lotId, 0, 10).subscribe({
            next: (bids) => {
                this.bids = bids;
                console.log('Bids for lot:', this.bids);
            },
            error: (error) => {
                console.error('Failed to fetch bids:', error);
            },
        });
    }

    onSubmit() {
        if (this.bidForm.valid) {
            const bidData = {
                lotId: this.lotId,
                bid: this.bidForm.value.bid,
            };

            this.apiClient.addBidForLot(bidData).subscribe({
                next: (response) => {
                    console.log('Bid placed successfully!', response);
                    this.bidForm.reset();
                    this.errorMsg = '';
                },
                error: (error) => {
                    console.log(error);
                    this.errorMsg = error?.error?.errors[0]?.ErrorMessage;
                },
            });
        }
    }

    closeDialog() {
        this.dialogRef.close();
    }
}
