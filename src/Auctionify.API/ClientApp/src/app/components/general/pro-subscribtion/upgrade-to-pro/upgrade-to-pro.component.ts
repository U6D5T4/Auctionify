import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { Client } from 'src/app/web-api-client';

@Component({
    selector: 'app-upgrade-to-pro',
    templateUrl: './upgrade-to-pro.component.html',
    styleUrls: ['./upgrade-to-pro.component.scss'],
})
export class UpgradeToProComponent {
    public constructor(private client: Client, private router: Router) { }

    upgradeToPro() {
        this.client.subscribeUserToPro().subscribe({
            next: (res: boolean) => {
                if (res) {
                    this.router.navigate(['/home']).then(() => {
                        window.location.reload();
                    });
                }
            },
        });
    }
}
