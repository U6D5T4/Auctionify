import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LotProfileSellerComponent } from './lot-profile-seller.component';

describe('LotProfileSellerComponent', () => {
  let component: LotProfileSellerComponent;
  let fixture: ComponentFixture<LotProfileSellerComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [LotProfileSellerComponent]
    });
    fixture = TestBed.createComponent(LotProfileSellerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
