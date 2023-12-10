import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthorizeService } from 'src/app/api-authorization/authorize.service';

@Component({
  selector: 'app-profile-navbar',
  templateUrl: './profile-navbar.component.html',
  styleUrls: ['./profile-navbar.component.scss']
})
export class ProfileNavbarComponent {
  constructor(private authService: AuthorizeService, private router: Router) {}

  logOut() {
    this.authService.logout();
    this.router.navigate(['/home']);
  }
}
