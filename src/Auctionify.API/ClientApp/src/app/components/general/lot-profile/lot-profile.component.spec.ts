import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LotProfileComponent } from './lot-profile.component';

describe('LotProfileComponent', () => {
  let component: LotProfileComponent;
  let fixture: ComponentFixture<LotProfileComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [LotProfileComponent]
    });
    fixture = TestBed.createComponent(LotProfileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
