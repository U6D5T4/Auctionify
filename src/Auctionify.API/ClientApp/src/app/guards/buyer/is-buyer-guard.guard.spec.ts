import { TestBed } from '@angular/core/testing';
import { CanActivateFn } from '@angular/router';

import { isBuyerGuardGuard } from './is-buyer-guard.guard';

describe('isBuyerGuardGuard', () => {
  const executeGuard: CanActivateFn = (...guardParameters) => 
      TestBed.runInInjectionContext(() => isBuyerGuardGuard(...guardParameters));

  beforeEach(() => {
    TestBed.configureTestingModule({});
  });

  it('should be created', () => {
    expect(executeGuard).toBeTruthy();
  });
});
