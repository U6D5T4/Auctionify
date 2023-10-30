import { CanActivateFn } from '@angular/router';

export const isBuyerGuardGuard: CanActivateFn = (route, state) => {
  return true;
};
