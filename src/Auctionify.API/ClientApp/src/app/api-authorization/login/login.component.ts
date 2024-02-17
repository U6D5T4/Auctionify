import { Component, Injectable, NgZone } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Dialog } from '@angular/cdk/dialog';
import { CredentialResponse, PromptMomentNotification } from 'google-one-tap';

import { DialogPopupComponent } from 'src/app/ui-elements/dialog-popup/dialog-popup.component';
import { LoginResponse } from 'src/app/web-api-client';
import { AuthorizeService } from '../authorize.service';

@Injectable({
    providedIn: 'root',
})
@Component({
    selector: 'app-login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.scss'],
})
export class LoginComponent {
    clientId!: string;
    passwordHidden: boolean = true;
    isLoading = false;

    constructor(
        private authService: AuthorizeService,
        public dialog: Dialog,
        private router: Router,
        private _ngZone: NgZone
    ) {}
    loginForm = new FormGroup({
        email: new FormControl<string>('', [
            Validators.required,
            Validators.email,
        ]),
        password: new FormControl('', [Validators.required]),
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
        this.authService.loginWithGoogle(response.credential).subscribe({
            next: (x: any) => {
                this._ngZone.run(() => {
                    if (this.authService.isUserLoggedIn()) {
                        if (
                            this.authService.isUserBuyer() ||
                            this.authService.isUserSeller()
                        ) {
                            this.router.navigate(['/home']);
                        } else {
                            if (this.authService.areLoginRolesProvided()) {
                                this.router.navigate([
                                    '/auth/select-login-role',
                                ]);
                            } else {
                                this.router.navigate(['/auth/register-role']);
                            }
                        }
                    }
                });
            },
            error: (error: any) => {},
        });
    }

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
                    if (this.authService.isUserLoggedIn()) {
                        if (
                            this.authService.isUserBuyer() ||
                            this.authService.isUserSeller()
                        ) {
                            this.router.navigate(['/home']);
                        } else {
                            if (this.authService.areLoginRolesProvided()) {
                                this.router.navigate([
                                    '/auth/select-login-role',
                                ]);
                            } else {
                                this.router.navigate(['/auth/register-role']);
                            }
                        }
                    }
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
