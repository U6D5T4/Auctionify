import { Component, Injectable, OnInit, effect } from '@angular/core';
import { Router } from '@angular/router';
import { AuthorizeService } from 'src/app/api-authorization/authorize.service';

@Injectable({
    providedIn: 'root',
})
@Component({
    selector: 'app-home',
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.scss'],
})
export class HomeComponent {
    constructor(private router: Router, private authService: AuthorizeService) {
        effect(() => {
            if (this.authService.isUserSeller())
                this.router.navigate(['/seller']);
        });

        effect(() => {
            if (this.authService.isUserBuyer())
                this.router.navigate(['/buyer']);
        });
    }
}
