import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { AuthorizeService } from 'src/app/api-authorization/authorize.service';

export const isProGuard: CanActivateFn = (route, state) => {
    const authService: AuthorizeService = inject(AuthorizeService);
    const router = inject(Router);

    return authService.isUserPro().pipe(
        map((e) => e),
        catchError((err) => {
            router.parseUrl('home');
            return of(false);
        })
    );
};
