import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HomeComponent } from './components/home/home.component';
import { ApiAuthorizationModule } from './api-authorization/api-authorization.module';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { AuthorizeInterceptor } from './api-authorization/authorize.interceptor';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { SellerModule } from './components/seller/seller.module';
import { LayoutModule } from './layout/layout.module';
import { BuyerModule } from './components/buyer/buyer.module';
import { GeneralModule } from './components/general/general.module';

@NgModule({
    declarations: [AppComponent, HomeComponent],
    imports: [
        BrowserModule,
        AppRoutingModule,
        ApiAuthorizationModule,
        HttpClientModule,
        BrowserAnimationsModule,
        SellerModule,
        BuyerModule,
        LayoutModule,
        GeneralModule,
    ],

    providers: [
        {
            provide: HTTP_INTERCEPTORS,
            useClass: AuthorizeInterceptor,
            multi: true,
        },
    ],
    bootstrap: [AppComponent],
})
export class AppModule {}
