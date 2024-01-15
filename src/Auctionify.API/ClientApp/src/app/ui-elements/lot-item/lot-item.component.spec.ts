import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LotItemComponent } from './lot-item.component';

describe('LotItemComponent', () => {
  let component: LotItemComponent;
  let fixture: ComponentFixture<LotItemComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [LotItemComponent]
    });
    fixture = TestBed.createComponent(LotItemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
