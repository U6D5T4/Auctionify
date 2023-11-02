import { Component, OnInit } from '@angular/core';
import { AuthorizeService } from '../../api-authorization/authorize.service';
import { LoginResponse } from 'src/app/web-api-client';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent implements OnInit {
  constructor(private authorize: AuthorizeService) {}

  async ngOnInit(): Promise<void> {
    this.authorize.login('seller@localhost.com', 'Test13!').subscribe({
      next: (res) => {
        console.log(res);

        this.authorize.register('asdasfasd','asdasdasd', 'adasdasdasd', 'asdasdasd').subscribe({
          next: (res) => console.log(res)
        });
      },
      error: (err: LoginResponse) => console.log(err.errors),
    });
  }
}
