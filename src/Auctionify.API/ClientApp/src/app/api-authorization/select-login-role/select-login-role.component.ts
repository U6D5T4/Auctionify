import { Component } from '@angular/core';
import { AuthorizeService, UserRole } from '../authorize.service';
import { FormControl, FormGroup } from '@angular/forms';
import { Dialog } from '@angular/cdk/dialog';
import { Router } from '@angular/router';
import { LoginResponse } from 'src/app/web-api-client';
import { ChoicePopupComponent } from 'src/app/ui-elements/choice-popup/choice-popup.component';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
    selector: 'app-select-login-role',
    templateUrl: './select-login-role.component.html',
    styleUrls: ['./select-login-role.component.scss'],
})
export class SelectLoginRoleComponent {
    isLoading = false;

    public roles = [
        {
            name: UserRole.Buyer,
            line1: 'I want to buy',
            line2: 'items on auction',
        },
        {
            name: UserRole.Seller,
            line1: 'I want to sell my',
            line2: 'unique items',
        },
    ];

    constructor(
        private authService: AuthorizeService,
        public dialog: Dialog,
        private router: Router,
        private snackBar: MatSnackBar
    ) {}

    selectLoginRoleForm = new FormGroup({
        role: new FormControl<UserRole>(UserRole.Seller),
    });

    onSubmit() {
        this.isLoading = true;
        if (this.selectLoginRoleForm.invalid) {
            this.isLoading = false;
            return;
        }

        const selectedRole = this.selectLoginRoleForm.controls.role.value;

        this.authService.loginWithSelectedRole(selectedRole!).subscribe({
            next: (success) => {
                this.router.navigate(['/home']);
            },
            error: (response: LoginResponse) => {
                this.isLoading = false;
                const dialogRef = this.dialog.open(ChoicePopupComponent, {
                    data: {
                        text: [
                            "You don't have " +
                                selectedRole +
                                ' account, do you want to create one?',
                        ],
                        isError: true,
                        continueBtnText: 'Create',
                        breakBtnText: 'Cancel',
                        additionalText: 'This will create a new role for you',
                        continueBtnColor: 'primary',
                        breakBtnColor: 'warn',
                    },
                    autoFocus: false,
                });

                dialogRef.closed.subscribe((result) => {
                    if (result === 'true') {
                        this.createNewUserRole(selectedRole!);
                    }
                });
            },
        });
    }

    createNewUserRole(role: string) {
        this.authService.createNewUserRole(role).subscribe({
            next: (success) => {
                this.router.navigate(['/home']);
                this.snackBar.open(
                    'Your ' + role + ' account has been successfully created',
                    'Close',
                    {
                        horizontalPosition: 'center',
                        verticalPosition: 'bottom',
                        duration: 5000,
                        panelClass: ['success-snackbar'],
                    }
                );
            },
            error: (response: LoginResponse) => {
                this.snackBar.open(response.message!, 'Close', {
                    horizontalPosition: 'center',
                    verticalPosition: 'bottom',
                    duration: 5000,
                    panelClass: ['error-snackbar'],
                });
            },
        });
    }
}
