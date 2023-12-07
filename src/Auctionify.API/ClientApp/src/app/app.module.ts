import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HomeComponent } from './components/home/home.component';
import { ApiAuthorizationModule } from './api-authorization/api-authorization.module';
import { AuthorizeInterceptor } from './api-authorization/authorize.interceptor';
import { DashboardComponent } from './components/seller/dashboard/dashboard.component';
import { LayoutModule } from './layout/layout.module';

@NgModule({
    declarations: [AppComponent, HomeComponent, DashboardComponent],
    imports: [
        BrowserModule,
        AppRoutingModule,
        ApiAuthorizationModule,
        HttpClientModule,
        BrowserAnimationsModule,
        LayoutModule,
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
