import { Component, Injectable, NgZone } from '@angular/core';

import { FormControl, FormGroup, Validators } from "@angular/forms";
import { Dialog } from '@angular/cdk/dialog';
import { Router } from '@angular/router';

import { DialogPopupComponent } from 'src/app/ui-elements/dialog-popup/dialog-popup.component';
import { RegisterResponse } from 'src/app/web-api-client';
import { environment } from 'src/environments/environment';
import { AuthorizeService } from '../authorize.service';

@Injectable({
  providedIn: 'root'
})

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
})
export class RegisterComponent {
  isLoading = false;
  passwordHidden: boolean = true;
  

  constructor(
    private authService: AuthorizeService, 
    public dialog: Dialog, 
    private router: Router,
    private _ngZone: NgZone) {

  }

  private clientId = environment.clientId;

  togglePasswordVisibility() {
    this.passwordHidden = !this.passwordHidden;
  }

  registerForm = new FormGroup({
    email:  new FormControl<string>('', [Validators.required, Validators.email]),
    password: new FormControl('', [Validators.required]),
    confirmPassword: new FormControl('', [Validators.required])
  })

  onSubmit() {
    this.isLoading = true;
    if (this.registerForm.invalid) {
      this.isLoading = false;
      return;
    }

    this.authService.register(
      this.registerForm.controls.email.value!,
      this.registerForm.controls.password.value!,
      this.registerForm.controls.confirmPassword.value!)
      .subscribe({
        next: (result) => {
          this.router.navigate(['/home'])
        },
        error: (error: RegisterResponse) => {
          this.openDialog(error.errors!, true);
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
      this.registerForm.controls.password.reset();
      this.registerForm.controls.confirmPassword.reset();
    })
  }
}
