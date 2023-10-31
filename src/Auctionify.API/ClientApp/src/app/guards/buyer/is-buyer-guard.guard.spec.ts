import { TestBed } from '@angular/core/testing';
import { CanActivateFn } from '@angular/router';

import { isBuyerGuard } from './is-buyer.guard';

describe('isBuyerGuardGuard', () => {
  const executeGuard: CanActivateFn = (...guardParameters) => 
      TestBed.runInInjectionContext(() => isBuyerGuard(...guardParameters));

  beforeEach(() => {
    TestBed.configureTestingModule({});
  });

  it('should be created', () => {
    expect(executeGuard).toBeTruthy();
  });
});
