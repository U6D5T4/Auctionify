import { CanActivateFn, Router } from '@angular/router';
import { AuthorizeService } from '../api-authorization/authorize.service';
import { inject } from '@angular/core';

export const isLoggedInGuard: CanActivateFn = (route, state) => {
    const authService = inject(AuthorizeService);
    const router: Router = inject(Router);

    const isLoggedIn = authService.isUserLoggedIn();
    const areLoginRolesProvided = authService.areLoginRolesProvided();

    switch (route.routeConfig?.path) {
        case 'auth/login':
        case 'auth/register':
            if (isLoggedIn && !areLoginRolesProvided)
                return router.parseUrl('/home');
            return true;
        default:
            if (isLoggedIn && !areLoginRolesProvided) return true;
    }

    return router.parseUrl('auth/login');
};
