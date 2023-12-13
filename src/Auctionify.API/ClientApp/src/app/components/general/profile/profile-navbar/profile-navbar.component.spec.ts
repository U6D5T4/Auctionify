import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProfileNavbarComponent } from './profile-navbar.component';

describe('ProfileNavbarComponent', () => {
  let component: ProfileNavbarComponent;
  let fixture: ComponentFixture<ProfileNavbarComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ProfileNavbarComponent]
    });
    fixture = TestBed.createComponent(ProfileNavbarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
