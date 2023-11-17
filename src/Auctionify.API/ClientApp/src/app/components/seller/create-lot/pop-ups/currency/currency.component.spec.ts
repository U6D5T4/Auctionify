import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CurrencyPopUpComponent } from './currency.component';

describe('CurrencyComponent', () => {
    let component: CurrencyPopUpComponent;
    let fixture: ComponentFixture<CurrencyPopUpComponent>;

    beforeEach(() => {
        TestBed.configureTestingModule({
            declarations: [CurrencyPopUpComponent],
        });
        fixture = TestBed.createComponent(CurrencyPopUpComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
