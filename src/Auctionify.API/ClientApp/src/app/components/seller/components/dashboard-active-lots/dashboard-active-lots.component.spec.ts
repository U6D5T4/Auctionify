import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DashboardActiveLotsComponent } from './dashboard-active-lots.component';

describe('DashboardActiveLotsComponent', () => {
  let component: DashboardActiveLotsComponent;
  let fixture: ComponentFixture<DashboardActiveLotsComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [DashboardActiveLotsComponent]
    });
    fixture = TestBed.createComponent(DashboardActiveLotsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
