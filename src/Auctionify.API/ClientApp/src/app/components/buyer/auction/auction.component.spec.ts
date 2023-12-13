import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AuctionComponent } from './auction.component';

describe('AuctionComponent', () => {
  let component: AuctionComponent;
  let fixture: ComponentFixture<AuctionComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [AuctionComponent]
    });
    fixture = TestBed.createComponent(AuctionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
