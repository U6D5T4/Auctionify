import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ImagePopupComponent } from './image-popup.component';

describe('ImagePopupComponent', () => {
  let component: ImagePopupComponent;
  let fixture: ComponentFixture<ImagePopupComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ImagePopupComponent]
    });
    fixture = TestBed.createComponent(ImagePopupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
