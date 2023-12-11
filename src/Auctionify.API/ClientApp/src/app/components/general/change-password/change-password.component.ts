import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Dialog } from '@angular/cdk/dialog';

import { DialogPopupComponent } from 'src/app/ui-elements/dialog-popup/dialog-popup.component';
import { AuthorizeService } from 'src/app/api-authorization/authorize.service';
import { BuyerModel, SellerModel } from 'src/app/models/users/user-models';
import { ChangePasswordResponse, Client } from 'src/app/web-api-client';

export interface ChangeUserPasswordFormModel {
  oldPassword: FormControl<string | null>;
  newPassword: FormControl<string | null>;
  confirmNewPassword: FormControl<string | null>;
}

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.scss']
})
export class ChangePasswordComponent {
  userProfileData: BuyerModel | SellerModel | null = null;
  isLoading = false;

  constructor(
    private authorizeService: AuthorizeService, 
    private client: Client, 
    private router: Router,
    public dialog: Dialog,
    ) {}

  ngOnInit(): void {
    this.fetchUserProfileData();
  }

  changePasswordForm = new FormGroup<ChangeUserPasswordFormModel>({
    oldPassword: new FormControl<string>('', [
      Validators.required,
      Validators.minLength(8),
    ]),
    newPassword: new FormControl<string>('', [
      Validators.required,
      Validators.minLength(8),
    ]),
    confirmNewPassword: new FormControl<string>('', [
      Validators.required,
      Validators.minLength(8),
    ]),
  });

  OnSubmit() {

    this.isLoading = true;

    if (this.changePasswordForm.invalid) {
      this.isLoading = false;
      return;
    }

    this.authorizeService.changePassword(
      this.changePasswordForm.controls.oldPassword.value!,
      this.changePasswordForm.controls.newPassword.value!,
      this.changePasswordForm.controls.confirmNewPassword.value!)
      .subscribe({
        next: (result) => {
          this.router.navigate(['/profile'])
        },
        error: (error: ChangePasswordResponse) => {
          this.openDialog(error.errors! || ['Something went wrong, please try later'], true);
        }
      })
  }

  openDialog(text: string[], error: boolean) {
    const dialogRef = this.dialog.open<string>(DialogPopupComponent, {
      data: {
        text,
        isError: error
      },
    });

    dialogRef.closed.subscribe((res) => {
      this.isLoading = false;
    })
  }

  private fetchUserProfileData() {
    if (this.isUserBuyer()) {
      this.client.getBuyer()
        .subscribe(
          (data: BuyerModel) => {
            this.userProfileData = data;
          },
          (error) => {
            // Handle error if needed
          }
        );
      console.log('User is a buyer.');
    } else if (this.isUserSeller()) {
      this.client.getSeller()
        .subscribe(
          (data: SellerModel) => {
            this.userProfileData = data;
          },
          (error) => {
            // Handle error if needed
          }
        );
      console.log('User is a seller.');
    }
  }

  isUserSeller(): boolean {
    return this.authorizeService.isUserSeller();
  }

  isUserBuyer(): boolean {
    return this.authorizeService.isUserBuyer();
  }
}
