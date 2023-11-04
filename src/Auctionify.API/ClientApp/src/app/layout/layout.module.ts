import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { NavbarComponent } from './navbar/navbar.component';
import { SearchComponent } from './search/search.component';
import { FooterComponent } from './footer/footer.component';

@NgModule({

  imports: [
    CommonModule
  ],
  
  declarations: [
    NavbarComponent,
    SearchComponent,
    FooterComponent
  ],
  
  exports: [
    NavbarComponent,
    FooterComponent
  ]
})

export class LayoutModule { }
