import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LotProfileBuyerComponent } from './lot-profile-buyer.component';

describe('LotProfileBuyerComponent', () => {
  let component: LotProfileBuyerComponent;
  let fixture: ComponentFixture<LotProfileBuyerComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [LotProfileBuyerComponent]
    });
    fixture = TestBed.createComponent(LotProfileBuyerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
