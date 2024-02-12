import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RateUserComponent } from './rate-user.component';

describe('RateUserComponent', () => {
  let component: RateUserComponent;
  let fixture: ComponentFixture<RateUserComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [RateUserComponent]
    });
    fixture = TestBed.createComponent(RateUserComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
