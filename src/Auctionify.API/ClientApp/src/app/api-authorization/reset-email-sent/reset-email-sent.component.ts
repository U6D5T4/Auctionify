import { Component } from '@angular/core';
import { ActivatedRoute, ParamMap } from '@angular/router';

@Component({
    selector: 'app-reset-email-sent',
    templateUrl: './reset-email-sent.component.html',
    styleUrls: ['./reset-email-sent.component.scss'],
})
export class ResetEmailSentComponent {
    emailAdress: string = '';

    constructor(private route: ActivatedRoute) {}

    ngOnInit(): void {
        this.route.paramMap.subscribe((params: ParamMap) => {
            const email = params.get('email');

            if (email) this.emailAdress = email;
        });
    }
}
