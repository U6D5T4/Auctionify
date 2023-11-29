import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LocationPopUpComponent } from './location.component';

describe('LocationComponent', () => {
    let component: LocationPopUpComponent;
    let fixture: ComponentFixture<LocationPopUpComponent>;

    beforeEach(() => {
        TestBed.configureTestingModule({
            declarations: [LocationPopUpComponent],
        });
        fixture = TestBed.createComponent(LocationPopUpComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
