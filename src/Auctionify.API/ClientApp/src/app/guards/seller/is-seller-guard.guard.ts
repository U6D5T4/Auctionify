import { CanActivateFn } from '@angular/router';

export const isSellerGuardGuard: CanActivateFn = (route, state) => {
  return true;
};
