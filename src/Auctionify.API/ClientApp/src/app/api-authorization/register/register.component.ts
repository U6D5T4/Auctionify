import { Component, Injectable, NgZone } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Dialog } from '@angular/cdk/dialog';
import { Router } from '@angular/router';
import { CredentialResponse, PromptMomentNotification } from 'google-one-tap';

import { DialogPopupComponent } from 'src/app/ui-elements/dialog-popup/dialog-popup.component';
import { RegisterResponse } from 'src/app/web-api-client';
import { AuthorizeService } from '../authorize.service';

@Component({
    selector: 'app-register',
    templateUrl: './register.component.html',
    styleUrls: ['./register.component.scss'],
})
export class RegisterComponent {
    clientId!: string;
    passwordHidden: boolean = true;
    isLoading = false;

    constructor(
        private authService: AuthorizeService,
        public dialog: Dialog,
        private router: Router,
        private service: AuthorizeService,
        private _ngZone: NgZone
    ) {}

    registerForm = new FormGroup({
        email: new FormControl<string>('', [
            Validators.required,
            Validators.email,
        ]),
        password: new FormControl('', [Validators.required]),
        confirmPassword: new FormControl('', [Validators.required]),
    });

    togglePasswordVisibility() {
        this.passwordHidden = !this.passwordHidden;
    }

    async ngOnInit(): Promise<void> {
        try {
            this.clientId = await this.authService.fetchGoogleClientId();
                // @ts-ignore
                google.accounts.id.initialize({
                    client_id: this.clientId,
                    callback: this.handleCredentialResponse.bind(this),
                    auto_select: false,
                    cancel_on_tap_outside: true,
                });

                // @ts-ignore
                google.accounts.id.renderButton(
                    // @ts-ignore
                    document.getElementsByClassName('google-link__label')[0],
                    { size: 'large', width: '100' }
                );

                // @ts-ignore
                google.accounts.id.prompt(
                    (notification: PromptMomentNotification) => {}
                );
        } catch (error) {
            console.error('Error fetching Google Client ID:', error);
        }
    }

    handleCredentialResponse(response: CredentialResponse) {
        this.service.loginWithGoogle(response.credential).subscribe({
            next: (x: any) => {
                this._ngZone.run(() => {
                    if (this.service.isUserLoggedIn()) {
                        if (
                            this.service.isUserBuyer() ||
                            this.service.isUserSeller()
                        ) {
                            this.router.navigate(['/home']);
                        } else {
                            this.router.navigate(['/auth/register-role']);
                        }
                    }
                });
            },
            error: (error: any) => {},
        });
    }

    onSubmit() {
        this.isLoading = true;
        if (this.registerForm.invalid) {
            this.isLoading = false;
            return;
        }

        this.authService
            .register(
                this.registerForm.controls.email.value!,
                this.registerForm.controls.password.value!,
                this.registerForm.controls.confirmPassword.value!
            )
            .subscribe({
                next: (result) => {
                    this.router.navigate([
                        `auth/email-sent/${this.registerForm.value.email}`,
                    ]);
                },
                error: (error: RegisterResponse) => {
                    this.openDialog(
                        error.errors! || [
                            'Something went wrong, please try later',
                        ],
                        true
                    );
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
            this.registerForm.controls.password.reset();
            this.registerForm.controls.confirmPassword.reset();
        });
    }
}
