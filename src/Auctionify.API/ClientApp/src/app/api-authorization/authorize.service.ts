import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, catchError, map, of, throwError } from 'rxjs';
import {
  Client,
  LoginResponse,
  LoginViewModel,
  RegisterResponse,
  RegisterViewModel,
} from '../web-api-client';
import { HttpErrorResponse, HttpResponseBase } from '@angular/common/http';

export enum UserRole {
  Administrator = 1,
  Seller,
  Buyer,
  User,
}

export interface IUser {
  userToken: string;
  role: UserRole;
  expireDate: string;
}

@Injectable({
  providedIn: 'root',
})
export class AuthorizeService {
  private tokenString: string = 'token';
  private expireString: string = 'expires_at';
  private user: BehaviorSubject<IUser | null> =
    new BehaviorSubject<IUser | null>(null);

  constructor(private client: Client) {}

  login(email: string, password: string) : Observable<string | boolean> {
    const loginData: LoginViewModel = {
      email,
      password,
    };

    return this.client
      .login(loginData)
      .pipe(map((response): boolean => {
        if (response.result === undefined) throw new Error("user not found");

          let newUser: IUser = {
            userToken: response.result.accessToken,
            role: response.result.role,
            expireDate: response.result.expireDate,
          }

          localStorage.setItem(this.tokenString, response.result.accessToken);
          localStorage.setItem(this.expireString, newUser.expireDate);

          this.user.next(newUser);

          return true;
      }))
      .pipe(catchError((err: HttpErrorResponse): Observable<string>  => {
        return throwError(() => err.error);
      }));
  }

  register(
    firstName: string,
    email: string,
    password: string,
    confirmPassword: string
  ) : Observable<RegisterResponse | undefined> {
    const registerData: RegisterViewModel = {
      firstName,
      email,
      password,
      confirmPassword,
    };

    return this.client.register(registerData).pipe(map((result) => {
      return result;
    }))
  }

  logout(): boolean {
    localStorage.removeItem(this.tokenString);
    localStorage.removeItem(this.expireString);
    this.user.next(null);

    return true;
  }

  getAccessToken(): Observable<string | null> {
    const localToken = localStorage.getItem(this.tokenString);

    if(localToken === null && localToken !== this.user.value?.userToken) return of(null);
    return of(localToken);
  }
}
