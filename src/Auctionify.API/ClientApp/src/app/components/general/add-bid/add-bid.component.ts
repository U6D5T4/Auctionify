import { Component, OnInit, Inject } from '@angular/core';
import {
    FormBuilder,
    FormControl,
    FormGroup,
    FormGroupDirective,
    NgForm,
    Validators,
} from '@angular/forms';
import { DIALOG_DATA, DialogRef } from '@angular/cdk/dialog';
import { ErrorStateMatcher } from '@angular/material/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { BidDto, Client } from '../../../web-api-client';

/** Error when invalid control is dirty, touched, or submitted. */
export class CustomErrorStateMatcher implements ErrorStateMatcher {
    isErrorState(
        control: FormControl | null,
        form: FormGroupDirective | NgForm | null
    ): boolean {
        const isSubmitted = form && form.submitted;
        return !!(
            control &&
            control.invalid &&
            (control.dirty || control.touched || isSubmitted)
        );
    }
}

@Component({
    selector: 'app-add-bid',
    templateUrl: './add-bid.component.html',
    styleUrls: ['./add-bid.component.scss'],
})
export class AddBidComponent implements OnInit {
    bidForm!: FormGroup;
    lotId!: number;
    bidCount!: number;
    currency!: string;
    startingPrice!: number;
    currentHighestBid!: number;
    bids: BidDto[] = [];
    errorMessage!: string;
    matcher = new CustomErrorStateMatcher();

    constructor(
        private dialogRef: DialogRef<AddBidComponent>,
        private apiClient: Client,
        @Inject(DIALOG_DATA) private data: any,
        private formBuilder: FormBuilder,
        private snackBar: MatSnackBar
    ) {
        this.lotId = this.data?.lotId;
        this.bidCount = this.data?.bidCount;
        this.currency = this.data?.currency.code;
        this.startingPrice = this.data?.startingPrice;
        this.currentHighestBid = this.data?.currentHighestBid;
    }

    ngOnInit() {
        this.bidForm = this.formBuilder.group({
            bid: [
                null,
                [
                    Validators.required,
                    this.maxBidValidator.bind(this),
                    this.minStartingBidValidation.bind(this),
                    this.minCurrentHighestBidValidation.bind(this),
                ],
            ],
        });

        this.getAllBidsForLot();
    }

    getAllBidsForLot() {
        this.apiClient.getAllBidsOfUserForLot(this.lotId, 0, 3).subscribe({
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
                    console.log(response);
                    this.bidForm.reset();
                    this.errorMessage = '';
                    this.closeDialog();
                    this.showSnackBar('Bid submitted successfully', 'success');
                },
                error: (error) => {
                    if (
                        JSON.parse(error) &&
                        JSON.parse(error)?.errors?.length
                    ) {
                        this.errorMessage =
                            JSON.parse(error)?.errors[0]?.ErrorMessage;
                        this.bidForm.reset();
                        this.showSnackBar(this.errorMessage, 'error');
                    }
                },
            });
        }
    }

    closeDialog() {
        this.dialogRef.close();
    }

    showSnackBar(message: string, messageType: 'success' | 'error') {
        let panelClass = ['custom-snackbar'];
        if (messageType === 'success') {
            panelClass.push('success-snackbar');
        } else if (messageType === 'error') {
            panelClass.push('error-snackbar');
        }

        this.snackBar.open(message, 'Close', {
            duration: 3000,
            horizontalPosition: 'center',
            verticalPosition: 'bottom',
            panelClass: panelClass,
        });
    }

    maxBidValidator(control: FormControl) {
        const maxBid = 1000000000000; // 1 trillion
        if (control.value && control.value > maxBid) {
            return { maxBidExceeded: true };
        }
        return null;
    }

    minStartingBidValidation(control: FormControl) {
        if (
            this.bidCount === 0 &&
            control.value &&
            control.value <= this.startingPrice
        ) {
            return { minStartingBid: true };
        }
        return null;
    }

    minCurrentHighestBidValidation(control: FormControl) {
        if (
            this.bidCount > 0 &&
            control.value &&
            control.value <= this.currentHighestBid
        ) {
            return { minCurrentHighestBid: true };
        }
        return null;
    }
}
