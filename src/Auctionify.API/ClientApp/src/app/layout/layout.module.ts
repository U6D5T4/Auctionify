import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { HeaderComponent } from './header/header.component';
import { SearchComponent } from './search/search.component';
import { FooterComponent } from './footer/footer.component';

@NgModule({
  imports: [
    CommonModule
  ],
  
  declarations: [
    HeaderComponent,
    SearchComponent,
    FooterComponent
  ],
  
  exports: [
    HeaderComponent,
    FooterComponent
  ]
})

export class LayoutModule { }
