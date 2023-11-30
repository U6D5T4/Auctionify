import { Component } from '@angular/core';

import { FormControl, FormGroup, Validators } from "@angular/forms";

import { AuthorizeService } from '../authorize.service';
import { Dialog } from '@angular/cdk/dialog';
import { DialogPopupComponent } from 'src/app/ui-elements/dialog-popup/dialog-popup.component';
import { Router } from '@angular/router';
import { ForgetPasswordResponse } from 'src/app/web-api-client';
import { catchError } from 'rxjs/operators';
import { of } from 'rxjs';

@Component({
  selector: 'app-forget-password',
  templateUrl: './forget-password.component.html',
  styleUrls: ['./forget-password.component.scss']
})
export class ForgetPasswordComponent {
  isLoading = false;

  constructor(
    private authService: AuthorizeService, 
    public dialog: Dialog, 
    private router: Router) {

  }
  forgetPasswordForm = new FormGroup({
    email: new FormControl<string>('', [Validators.required, Validators.email])
  })

  onSubmit(){
    this.isLoading = true;
    if (this.forgetPasswordForm.invalid) {
      this.isLoading = false;
      return;
    }

    this.authService.forgetPassword(this.forgetPasswordForm.controls.email.value!)
      .pipe(
        catchError((error: ForgetPasswordResponse) => {
          this.openDialog(error.errors || ['Email is not correct'], true);
          return of(null);
        })
      )
      .subscribe({
        next: (result) => {
          if (result) {
            this.router.navigate(['/home'])
          }
        },
        complete: () => {
          this.isLoading = false;
        }
      });
  }

  goToLoginPage() {
    this.router.navigate(['auth/login']);
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
      this.forgetPasswordForm.controls.email.reset();
    })
  }
}

