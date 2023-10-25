import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable, mergeMap } from 'rxjs';
import { AuthorizeService } from './authorize.service';

@Injectable()
export class AuthorizeInterceptor implements HttpInterceptor {

  constructor(private authService: AuthorizeService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return this.authService.getAccessToken().pipe(mergeMap(token => this.processRequestWithToken(token, request, next)));
  }

  private processRequestWithToken(token: string | null, req: HttpRequest<any>, next: HttpHandler) {
    if (!!token && this.isSameOriginUrl(req)) {
      req = req.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      });
    }

    return next.handle(req);
  }

  private isSameOriginUrl(req: any) {
    // It's an absolute url with the same origin.
    if (req.url.startsWith(`${window.location.origin}/`)) {
      return true;
    }

    // It's a protocol relative url with the same origin.
    // For example: //www.example.com/api/Products
    if (req.url.startsWith(`//${window.location.host}/`)) {
      return true;
    }

    // It's a relative url like /api/Products
    if (/^\/[^\/].*/.test(req.url)) {
      return true;
    }

    // It's an absolute or protocol relative url that
    // doesn't have the same origin.
    return false;
  }
}
