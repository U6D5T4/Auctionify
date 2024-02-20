import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChatsComponent } from './chats.component';

describe('ChatsComponent', () => {
  let component: ChatsComponent;
  let fixture: ComponentFixture<ChatsComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ChatsComponent]
    });
    fixture = TestBed.createComponent(ChatsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
