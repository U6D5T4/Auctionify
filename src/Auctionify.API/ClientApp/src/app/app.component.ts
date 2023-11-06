import { Component } from '@angular/core';
import { AuthorizeService } from './api-authorization/authorize.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent {
  isUserBuyer: boolean = false;
  isUserSeller: boolean = true;

  constructor(private authService: AuthorizeService) {
    this.isUserBuyer = authService.isUserBuyer();
    this.isUserSeller = authService.isUserSeller();
  }
  title = 'ClientApp';
}
