import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddBidComponent } from './add-bid.component';

describe('AddBidComponent', () => {
  let component: AddBidComponent;
  let fixture: ComponentFixture<AddBidComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [AddBidComponent]
    });
    fixture = TestBed.createComponent(AddBidComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
