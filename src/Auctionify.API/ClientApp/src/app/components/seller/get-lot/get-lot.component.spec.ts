import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GetLotComponent } from './get-lot.component';

describe('GetLotComponent', () => {
  let component: GetLotComponent;
  let fixture: ComponentFixture<GetLotComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [GetLotComponent]
    });
    fixture = TestBed.createComponent(GetLotComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
