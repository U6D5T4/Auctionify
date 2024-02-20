import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProPageComponent } from './pro-page.component';

describe('ProPageComponent', () => {
  let component: ProPageComponent;
  let fixture: ComponentFixture<ProPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProPageComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ProPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
