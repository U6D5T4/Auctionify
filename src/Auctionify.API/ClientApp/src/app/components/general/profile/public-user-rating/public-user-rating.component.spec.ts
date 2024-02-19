import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PublicUserRatingComponent } from './public-user-rating.component';

describe('PublicUserRatingComponent', () => {
  let component: PublicUserRatingComponent;
  let fixture: ComponentFixture<PublicUserRatingComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [PublicUserRatingComponent]
    });
    fixture = TestBed.createComponent(PublicUserRatingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
