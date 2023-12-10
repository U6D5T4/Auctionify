import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EmailSentComponent } from './email-sent.component';

describe('EmailSentComponent', () => {
  let component: EmailSentComponent;
  let fixture: ComponentFixture<EmailSentComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [EmailSentComponent]
    });
    fixture = TestBed.createComponent(EmailSentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
