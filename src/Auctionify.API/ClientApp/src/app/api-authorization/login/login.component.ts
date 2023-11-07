import { Component, Injectable } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { AuthorizeService } from '../authorize.service';
import { Dialog } from '@angular/cdk/dialog';
import { DialogPopupComponent } from 'src/app/ui-elements/dialog-popup/dialog-popup.component';
import { LoginResponse } from 'src/app/web-api-client';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent {
  isLoading = false;

  constructor(
    private authService: AuthorizeService,
    public dialog: Dialog,
    private router: Router
  ) {}
  loginForm = new FormGroup({
    email: new FormControl<string>('', [Validators.required, Validators.email]),
    password: new FormControl('', [Validators.required]),
  });

  onSubmit() {
    this.isLoading = true;
    if (this.loginForm.invalid) {
      this.isLoading = false;
      return;
    }

    this.authService
      .login(
        this.loginForm.controls.email.value!,
        this.loginForm.controls.password.value!
      )
      .subscribe({
        next: (result) => {
          this.router.navigate(['/home']);
        },
        error: (error: LoginResponse) => {
          this.openDialog(error.errors!, true);
        },
      });
  }

  openDialog(text: string[], error: boolean) {
    const dialogRef = this.dialog.open<string>(DialogPopupComponent, {
      data: {
        text,
        isError: error,
      },
    });

    dialogRef.closed.subscribe((res) => {
      this.isLoading = false;
      this.loginForm.controls.password.reset();
    });
  }
}
