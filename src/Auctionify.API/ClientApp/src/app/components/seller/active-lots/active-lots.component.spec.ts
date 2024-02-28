import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ActiveLotsComponent } from './active-lots.component';

describe('ActiveLotsComponent', () => {
  let component: ActiveLotsComponent;
  let fixture: ComponentFixture<ActiveLotsComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ActiveLotsComponent]
    });
    fixture = TestBed.createComponent(ActiveLotsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
