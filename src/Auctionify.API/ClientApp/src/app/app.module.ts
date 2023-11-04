import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HomeComponent } from './components/home/home.component';
import { ApiAuthorizationModule } from './api-authorization/api-authorization.module';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { AuthorizeInterceptor } from './api-authorization/authorize.interceptor';
import { UiElementsModule } from './ui-elements/ui-elements.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { SellerModule } from './components/seller/seller.module';
import { DashboardComponent } from './components/seller/dashboard/dashboard.component';

@NgModule({
  declarations: [AppComponent, HomeComponent, DashboardComponent],
  imports: [
    BrowserModule,
    AppRoutingModule,
    ApiAuthorizationModule,
    HttpClientModule,
    UiElementsModule,
    BrowserAnimationsModule,
    SellerModule,
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AuthorizeInterceptor, multi: true },
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
