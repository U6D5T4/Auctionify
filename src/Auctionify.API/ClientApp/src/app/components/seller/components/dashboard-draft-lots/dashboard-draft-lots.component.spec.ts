import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DashboardDraftLotsComponent } from './dashboard-draft-lots.component';

describe('DashboardDraftLotsComponent', () => {
  let component: DashboardDraftLotsComponent;
  let fixture: ComponentFixture<DashboardDraftLotsComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [DashboardDraftLotsComponent]
    });
    fixture = TestBed.createComponent(DashboardDraftLotsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
