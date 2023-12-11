import { Component, OnInit } from '@angular/core';

import { AuthorizeService } from 'src/app/api-authorization/authorize.service';
import { BuyerModel, SellerModel } from 'src/app/models/users/user-models';
import { Client } from 'src/app/web-api-client';

@Component({
  selector: 'app-user-profile',
  templateUrl: './user-profile.component.html',
  styleUrls: ['./user-profile.component.scss']
})
export class UserProfileComponent {
  userProfileData: BuyerModel | SellerModel | null = null;

  constructor(private authorizeService: AuthorizeService, private client: Client) {}

  ngOnInit(): void {
    this.fetchUserProfileData();
  }

  private fetchUserProfileData() {
    if (this.isUserBuyer()) {
      this.client.getBuyer().subscribe((data: BuyerModel) => {
        this.userProfileData = data;
      });
      console.log('User is a buyer.');
    } else if (this.isUserSeller()) {
      this.client.getSeller().subscribe((data: SellerModel) => {
        this.userProfileData = data;
      });
      console.log('User is a seller.');
    }
  }
  
  isUserSeller(): boolean {
    return this.authorizeService.isUserSeller();
  }

  isUserBuyer(): boolean {
    return this.authorizeService.isUserBuyer();
  }
}
