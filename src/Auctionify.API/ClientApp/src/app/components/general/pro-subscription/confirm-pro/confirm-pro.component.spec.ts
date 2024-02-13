import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ConfirmProComponent } from './confirm-pro.component';

describe('ConfirmProComponent', () => {
  let component: ConfirmProComponent;
  let fixture: ComponentFixture<ConfirmProComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ConfirmProComponent]
    });
    fixture = TestBed.createComponent(ConfirmProComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
