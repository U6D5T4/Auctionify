import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CreateLotComponent } from './create-lot/create-lot.component';
import { sellerRoutingModule } from './seller-routing.module';
import { MatFormFieldModule } from '@angular/material/form-field';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { LocationPopUpComponent } from './create-lot/pop-ups/location/location.component';
import { CurrencyPopUpComponent } from './create-lot/pop-ups/currency/currency.component';
import { FilesPopUpComponent } from './create-lot/pop-ups/files/files.component';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { DashboardComponent } from './dashboard/dashboard.component';
import { DashboardActiveLotsComponent } from './components/dashboard-active-lots/dashboard-active-lots.component';
import { UiElementsModule } from 'src/app/ui-elements/ui-elements.module';
import { GeneralModule } from '../general/general.module';
import { DashboardDraftLotsComponent } from './components/dashboard-draft-lots/dashboard-draft-lots.component';
import { ActiveLotsComponent } from './active-lots/active-lots.component';
import { AnalyticsComponent } from './analytics/analytics.component';
import { NgApexchartsModule } from 'ng-apexcharts';
import { ExportToFileComponent } from './export-to-file/export-to-file.component';

@NgModule({
    declarations: [
        CreateLotComponent,
        LocationPopUpComponent,
        CurrencyPopUpComponent,
        FilesPopUpComponent,
        DashboardComponent,
        DashboardActiveLotsComponent,
        DashboardDraftLotsComponent,
        ActiveLotsComponent,
        AnalyticsComponent,
        ExportToFileComponent,
    ],
    imports: [
        CommonModule,
        sellerRoutingModule,
        MatFormFieldModule,
        ReactiveFormsModule,
        MatButtonModule,
        MatIconModule,
        MatInputModule,
        MatSelectModule,
        MatDatepickerModule,
        MatNativeDateModule,
        MatProgressSpinnerModule,
        MatSnackBarModule,
        UiElementsModule,
        GeneralModule,
        NgApexchartsModule,
        FormsModule,
    ],
    exports: [CreateLotComponent, DashboardComponent],
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class SellerModule {}
