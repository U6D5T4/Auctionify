import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SelectLoginRoleComponent } from './select-login-role.component';

describe('SelectLoginRoleComponent', () => {
  let component: SelectLoginRoleComponent;
  let fixture: ComponentFixture<SelectLoginRoleComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [SelectLoginRoleComponent]
    });
    fixture = TestBed.createComponent(SelectLoginRoleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
