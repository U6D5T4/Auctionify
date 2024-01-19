import { Component, OnInit } from '@angular/core';
import { Dialog } from '@angular/cdk/dialog';
import { ActivatedRoute } from '@angular/router';

import { DialogPopupComponent } from 'src/app/ui-elements/dialog-popup/dialog-popup.component';
import { GetUserById } from 'src/app/models/users/user-models';
import { Client } from 'src/app/web-api-client';

@Component({
    selector: 'app-public-user-profile',
    templateUrl: './public-user-profile.component.html',
    styleUrls: ['./public-user-profile.component.scss'],
})
export class PublicUserProfileComponent implements OnInit {
    userProfileData: GetUserById | null = null;

    constructor(
        private client: Client,
        public dialog: Dialog,
        private activeRoute: ActivatedRoute
    ) {}

    ngOnInit(): void {
        this.fetchUserProfileData();
    }

    private fetchUserProfileData() {
        const userId = this.activeRoute.snapshot.params['id'];

        this.client.getUserById(userId).subscribe(
            (data: GetUserById) => {
                this.userProfileData = data;
            },
            (error) => {
                this.openDialog(
                    error.errors! || [
                        'Something went wrong, please try again later',
                    ],
                    true
                );
            }
        );
    }

    openDialog(text: string[], error: boolean) {
        const dialogRef = this.dialog.open<string>(DialogPopupComponent, {
            data: {
                text,
                isError: error,
            },
        });

        dialogRef.closed.subscribe((res) => {});
    }
}
