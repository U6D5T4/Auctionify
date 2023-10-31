import { TestBed } from '@angular/core/testing';
import { CanActivateFn } from '@angular/router';

import { isSellerGuard } from './is-seller.guard';

describe('isSellerGuardGuard', () => {
  const executeGuard: CanActivateFn = (...guardParameters) => 
      TestBed.runInInjectionContext(() => isSellerGuard(...guardParameters));

  beforeEach(() => {
    TestBed.configureTestingModule({});
  });

  it('should be created', () => {
    expect(executeGuard).toBeTruthy();
  });
});
