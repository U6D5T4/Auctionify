import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AfterDeletionComponent } from './after-deletion.component';

describe('AfterDeletionComponent', () => {
  let component: AfterDeletionComponent;
  let fixture: ComponentFixture<AfterDeletionComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [AfterDeletionComponent]
    });
    fixture = TestBed.createComponent(AfterDeletionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
