import { TestBed } from '@angular/core/testing';
import { CanActivateFn } from '@angular/router';

import { isSellerGuardGuard } from './is-seller-guard.guard';

describe('isSellerGuardGuard', () => {
  const executeGuard: CanActivateFn = (...guardParameters) => 
      TestBed.runInInjectionContext(() => isSellerGuardGuard(...guardParameters));

  beforeEach(() => {
    TestBed.configureTestingModule({});
  });

  it('should be created', () => {
    expect(executeGuard).toBeTruthy();
  });
});
