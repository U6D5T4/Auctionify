import { Component } from '@angular/core';

import { FormControl, FormGroup, Validators } from "@angular/forms";
import { AuthorizeService } from '../authorize.service';
import { Dialog } from '@angular/cdk/dialog';
import { Router } from '@angular/router';

@Component({
  selector: 'app-forget-password',
  templateUrl: './forget-password.component.html',
  styleUrls: ['./forget-password.component.scss']
})
export class ForgetPasswordComponent {
  isLoading = false;

  constructor(private authService: AuthorizeService, public dialog: Dialog, private router: Router) {

  }
  forgetPasswordForm = new FormGroup({
    email: new FormControl('', [Validators.required])
  })

  onSubmit(){

  }
}

