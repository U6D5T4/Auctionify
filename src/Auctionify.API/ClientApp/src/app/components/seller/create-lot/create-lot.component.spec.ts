import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateLotComponent } from './create-lot.component';

describe('CreateLotComponent', () => {
  let component: CreateLotComponent;
  let fixture: ComponentFixture<CreateLotComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [CreateLotComponent]
    });
    fixture = TestBed.createComponent(CreateLotComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
