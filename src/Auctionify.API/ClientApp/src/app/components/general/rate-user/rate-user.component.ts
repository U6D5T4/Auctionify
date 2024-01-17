import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, ParamMap, Route, Router } from '@angular/router';

import { AuthorizeService } from 'src/app/api-authorization/authorize.service';
import { RateUserCommandModel } from 'src/app/models/rates/rate-models';
import {
    BuyerGetLotResponse,
    Client,
    SellerGetLotResponse,
} from 'src/app/web-api-client';

@Component({
    selector: 'app-rate-user',
    templateUrl: './rate-user.component.html',
    styleUrls: ['./rate-user.component.scss'],
})
export class RateUserComponent implements OnInit {
    lotData!: SellerGetLotResponse | BuyerGetLotResponse | null;
    isLoading = false;
    lotId: number = 0;
    selectedRating: number = 0;
    comment: string = '';
    rateData!: RateUserCommandModel | null;
    errorMessage!: string;

    constructor(
        private client: Client,
        private authorizeService: AuthorizeService,
        private route: ActivatedRoute,
        private snackBar: MatSnackBar,
        private router: Router
    ) {}

    ngOnInit() {
        this.getLotFromRoute();
    }

    getLotFromRoute() {
        this.route.paramMap.subscribe(async (params: ParamMap) => {
            this.lotId = Number(params.get('id'));
            if (this.authorizeService.isUserBuyer()) {
                this.client
                    .getOneLotForBuyer(this.lotId)
                    .subscribe((data: BuyerGetLotResponse) => {
                        this.lotData = data;
                    });
            } else {
                this.client
                    .getOneLotForSeller(this.lotId)
                    .subscribe((data: SellerGetLotResponse) => {
                        this.lotData = data;
                    });
            }
        });
    }

    OnSubmit() {
        const rateData: RateUserCommandModel = {
            lotId: this.lotId,
            comment: this.comment,
            ratingValue: this.selectedRating,
        };

        this.isLoading = true;

        if (this.authorizeService.isUserSeller()) {
            this.client.addRateToBuyer(rateData).subscribe(
                (response) => {
                    this.errorMessage = '';
                    this.showSnackBar('Rate submitted successfully', 'success');
                    this.isLoading = false;
                    this.router.navigate(["/home"])
                },
                (error) => {
                    if (
                        JSON.parse(error) &&
                        JSON.parse(error)?.errors?.length
                    ) {
                        this.errorMessage =
                            JSON.parse(error)?.errors[0]?.ErrorMessage;
                        this.showSnackBar(this.errorMessage, 'error');
                    }
                }
            );
        } else if (this.authorizeService.isUserBuyer()) {
            this.client.addRateToSeller(rateData).subscribe(
                (response) => {
                    this.errorMessage = '';
                    this.showSnackBar('Rate submitted successfully', 'success');
                    this.isLoading = false;
                    this.router.navigate(["/home"])
                },
                (error) => {
                    if (
                        JSON.parse(error) &&
                        JSON.parse(error)?.errors?.length
                    ) {
                        this.errorMessage =
                            JSON.parse(error)?.errors[0]?.ErrorMessage;
                        this.showSnackBar(this.errorMessage, 'error');
                    }
                }
            );
        }
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

    onStarClick(rating: number): void {
        this.selectedRating = rating;
    }

    getUserProfileImage(src: string | undefined): string {
        return src ? src : '../../../../../assets/images/User.png';
    }

    isUserSeller(): boolean {
        return this.authorizeService.isUserSeller();
    }
}
