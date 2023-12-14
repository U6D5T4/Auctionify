import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, ParamMap } from '@angular/router';

@Component({
    selector: 'app-email-sent',
    templateUrl: './email-sent.component.html',
    styleUrls: ['./email-sent.component.scss'],
})
export class EmailSentComponent implements OnInit {
    emailAdress: string = '';

    constructor(private route: ActivatedRoute) {}

    ngOnInit(): void {
        this.route.paramMap.subscribe((params: ParamMap) => {
            const email = params.get('email');

            if (email) this.emailAdress = email;
        });
    }
}
