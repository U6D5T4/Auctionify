import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AverageRatingItemComponent } from './average-rating-item.component';

describe('AverageRatingItemComponent', () => {
  let component: AverageRatingItemComponent;
  let fixture: ComponentFixture<AverageRatingItemComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [AverageRatingItemComponent]
    });
    fixture = TestBed.createComponent(AverageRatingItemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
