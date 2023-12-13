import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WithdrawBidComponent } from './withdraw-bid.component';

describe('WithdrawBidComponent', () => {
  let component: WithdrawBidComponent;
  let fixture: ComponentFixture<WithdrawBidComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [WithdrawBidComponent]
    });
    fixture = TestBed.createComponent(WithdrawBidComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
