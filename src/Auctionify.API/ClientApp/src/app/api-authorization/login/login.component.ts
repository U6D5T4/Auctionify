import { Component, Injectable } from '@angular/core';
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { AuthorizeService } from '../authorize.service';
import { Dialog } from '@angular/cdk/dialog';
import { DialogPopupComponent } from 'src/app/ui-elements/dialog-popup/dialog-popup.component';

@Injectable({
  providedIn: 'root'
})
@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {

  constructor(private authService: AuthorizeService, public dialog: Dialog) {

  }
  loginForm = new FormGroup({
    email:  new FormControl<string>('', [Validators.required, Validators.email]),
    password: new FormControl('', [Validators.required])
  })

  onSubmit() {
    if (this.loginForm.invalid) return;

    this.authService.login(
      this.loginForm.controls.email.value!,
      this.loginForm.controls.password.value!)
      .subscribe({
        next: (result) => {
          console.log(result);
        },
        error: (error) => {
          console.log(error);
          this.openDialog(error, true);
        }
      })
  }

  openDialog(text: string, error: boolean) {
    const dialogRef = this.dialog.open<string>(DialogPopupComponent, {
      width: '250px',
      data: {text, error},
    });
  }
}
