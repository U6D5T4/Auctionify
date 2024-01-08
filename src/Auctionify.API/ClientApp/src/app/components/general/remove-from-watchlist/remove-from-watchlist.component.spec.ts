import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RemoveFromWatchlistComponent } from './remove-from-watchlist.component';

describe('RemoveFromWatchlistComponent', () => {
  let component: RemoveFromWatchlistComponent;
  let fixture: ComponentFixture<RemoveFromWatchlistComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [RemoveFromWatchlistComponent]
    });
    fixture = TestBed.createComponent(RemoveFromWatchlistComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
