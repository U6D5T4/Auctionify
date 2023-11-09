import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, catchError, map, of, throwError } from 'rxjs';
import {
  AssignRoleResponse,
  AssignRoleViewModel,
  Client,
  LoginResponse,
  LoginViewModel,
  RegisterResponse,
  RegisterViewModel,
} from '../web-api-client';
import { HttpErrorResponse, HttpResponseBase } from '@angular/common/http';

export enum UserRole {
  Administrator = 'Administrator',
  Seller = 'Seller',
  Buyer = 'Buyer',
  User = 'User',
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

    constructor(private client: Client, private httpClient: HttpClient) {
      //this.initializeAuthorizeService();
    }

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

  assignRoleToUser(role: UserRole): Observable<AssignRoleResponse | boolean> {
    const userToken = localStorage.getItem(this.tokenString);

    const roleAssignmentData: AssignRoleViewModel = {
      token: userToken ?? '',
      role
    };
  
    return this.client.assignRoleToUser(roleAssignmentData).pipe(map((result) => {
      return result;
    }))
  }

  register(email: string, password: string, confirmPassword: string ) : Observable<RegisterResponse | boolean > {
    const registerData: RegisterViewModel = {
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
