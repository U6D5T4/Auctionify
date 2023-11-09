import { Component, Injectable } from '@angular/core';
import { AuthorizeService, UserRole } from '../authorize.service';
import { Dialog } from '@angular/cdk/dialog';
import { DialogPopupComponent } from 'src/app/ui-elements/dialog-popup/dialog-popup.component';
import { Router } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { AssignRoleResponse } from 'src/app/web-api-client';

@Injectable({
  providedIn: 'root'
})

@Component({
  selector: 'app-register-role',
  templateUrl: './register-role.component.html',
  styleUrls: ['./register-role.component.scss']
})
export class RegisterRoleComponent {
  public roles = [
    {
      name: UserRole.Seller,
      desc: ""
    },
    {
      name: UserRole.Buyer,
      desc: "bla bla"
    },
  ];
  isLoading = false;

  constructor(private authService: AuthorizeService, 
              public dialog: Dialog, 
              private router: Router) {
  }

  assignRoleForm = new FormGroup({
    role: new FormControl<UserRole>(UserRole.Seller)
  })

  onSubmit() {
    this.isLoading = true;
    if (this.assignRoleForm.invalid) {
      this.isLoading = false;
      return;
    }

    const selectedRole = this.assignRoleForm.controls.role.value;
    

    this.authService.assignRoleToUser(
      selectedRole!
    ).subscribe({
      next: (success) => {
        this.router.navigate(['/home']);
      },
        error: (error: AssignRoleResponse) => {
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
    })
  }
}
