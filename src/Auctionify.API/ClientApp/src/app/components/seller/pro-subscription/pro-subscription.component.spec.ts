import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProSubscriptionComponent } from './pro-subscription.component';

describe('ProSubscriptionComponent', () => {
  let component: ProSubscriptionComponent;
  let fixture: ComponentFixture<ProSubscriptionComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ProSubscriptionComponent]
    });
    fixture = TestBed.createComponent(ProSubscriptionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
