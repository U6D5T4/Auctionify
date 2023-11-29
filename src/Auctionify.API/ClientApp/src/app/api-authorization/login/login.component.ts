import { Component, Injectable, NgZone } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { AuthorizeService } from '../authorize.service';
import { Dialog } from '@angular/cdk/dialog';
import { DialogPopupComponent } from 'src/app/ui-elements/dialog-popup/dialog-popup.component';
import { LoginResponse } from 'src/app/web-api-client';
import { Router } from '@angular/router';
import { CredentialResponse, PromptMomentNotification } from 'google-one-tap';
import { environment } from 'src/environments/environment';

@Injectable({
    providedIn: 'root',
})
@Component({
    selector: 'app-login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.scss'],
})
export class LoginComponent {
    passwordHidden: boolean = true;
    isLoading = false;

    togglePasswordVisibility() {
        this.passwordHidden = !this.passwordHidden;
    }

    private clientId = environment.clientId;

    constructor(
        private authService: AuthorizeService,
        public dialog: Dialog,
        private router: Router,
        private service: AuthorizeService,
        private _ngZone: NgZone
    ) {}
    loginForm = new FormGroup({
        email: new FormControl<string>('', [
            Validators.required,
            Validators.email,
        ]),
        password: new FormControl('', [Validators.required]),
    });

    ngOnInit(): void {
        // @ts-ignore
        window.onGoogleLibraryLoad = () => {
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
                { size: 'large', width: '100%' }
            );
            // @ts-ignore
            google.accounts.id.prompt(
                (notification: PromptMomentNotification) => {}
            );
        };
    }

    handleCredentialResponse(response: CredentialResponse) {
        this.service.loginWithGoogle(response.credential).subscribe({
            next: (x: any) => {
                this._ngZone.run(() => {
                    this.router.navigate(['/home']);
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
