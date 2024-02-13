import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ConfirmProSubscriptionComponent } from './confirm-pro-subscription.component';

describe('ConfirmProSubscriptionComponent', () => {
  let component: ConfirmProSubscriptionComponent;
  let fixture: ComponentFixture<ConfirmProSubscriptionComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ConfirmProSubscriptionComponent]
    });
    fixture = TestBed.createComponent(ConfirmProSubscriptionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
