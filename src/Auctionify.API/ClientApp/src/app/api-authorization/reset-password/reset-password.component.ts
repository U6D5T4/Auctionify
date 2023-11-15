import { Component, Injectable, OnInit } from '@angular/core';

import { ActivatedRoute } from '@angular/router';
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { AuthorizeService } from '../authorize.service';
import { Dialog } from '@angular/cdk/dialog';
import { DialogPopupComponent } from 'src/app/ui-elements/dialog-popup/dialog-popup.component';
import { Router } from '@angular/router';
import { ResetPasswordResponse } from 'src/app/web-api-client';

@Injectable({
  providedIn: 'root'
})

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.scss']
})

export class ResetPasswordComponent implements OnInit {
  isLoading = false;
  passwordHidden: boolean = true;

  ngOnInit(): void {
    this.getUserIdAndTokenFromUrl();
  }

  togglePasswordVisibility() {
    this.passwordHidden = !this.passwordHidden;
  }

  constructor(
    private authService: AuthorizeService, 
    public dialog: Dialog, 
    private router: Router,
    private activatedRoute: ActivatedRoute) {

  }
  resetPasswordForm = new FormGroup({
    token: new FormControl(''),
    email: new FormControl(''),
    newPassword: new FormControl('', [Validators.required]),
    confirmPassword: new FormControl('', [Validators.required])
  })

  getUserIdAndTokenFromUrl(): void {
    this.activatedRoute.queryParams.subscribe(params => {
      let token = params['token'];
      const email = params['email'];

      this.resetPasswordForm.get('token')?.setValue(token);
      this.resetPasswordForm.get('email')?.setValue(email);
    });
  }

  onSubmit() {
    this.isLoading = true;
    if (this.resetPasswordForm.invalid) {
      this.isLoading = false;
      return;
    }

    this.authService.resetPassword(
      this.resetPasswordForm.controls.token.value!,
      this.resetPasswordForm.controls.email.value!,
      this.resetPasswordForm.controls.newPassword.value!,
      this.resetPasswordForm.controls.confirmPassword.value!)
      .subscribe({
        next: (result) => {
          this.router.navigate(['/auth/login']);
        },
        error: (error: ResetPasswordResponse) => {
          this.openDialog(error.errors || ['An error occurred. Please try again.'], true);
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
      this.resetPasswordForm.controls.newPassword.reset();
      this.resetPasswordForm.controls.confirmPassword.reset();
    })
  }

  goToLoginPage() {
    this.router.navigate(['auth/login']);
  }
}
