import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ConversationWindowComponent } from './conversation-window.component';

describe('ConversationWindowComponent', () => {
  let component: ConversationWindowComponent;
  let fixture: ComponentFixture<ConversationWindowComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ConversationWindowComponent]
    });
    fixture = TestBed.createComponent(ConversationWindowComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
