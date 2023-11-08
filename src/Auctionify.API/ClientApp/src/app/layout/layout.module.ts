import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatSelectModule } from '@angular/material/select';

import { HeaderComponent } from './header/header.component';
import { SearchComponent } from './search/search.component';
import { FooterComponent } from './footer/footer.component';
import { RouterModule } from '@angular/router';
import { NavbarSellerComponent } from './navbar-seller/navbar-seller.component';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';

@NgModule({
  imports: [
    CommonModule,
    RouterModule,
    MatSidenavModule,
    MatFormFieldModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
  ],

  declarations: [
    HeaderComponent,
    SearchComponent,
    FooterComponent,
    NavbarSellerComponent,
  ],
  
  exports: [
    HeaderComponent,
    FooterComponent,
    NavbarSellerComponent,
  ]
})
export class LayoutModule {}
