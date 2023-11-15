import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthorizeService } from 'src/app/api-authorization/authorize.service';

@Component({
  selector: 'app-navbar-seller',
  templateUrl: './navbar-seller.component.html',
  styleUrls: ['./navbar-seller.component.scss'],
})
export class NavbarSellerComponent {
  constructor(private authService: AuthorizeService, private router: Router) {}

  logOut() {
    this.authService.logout();
    this.router.navigate(['/home']);
  }
}
