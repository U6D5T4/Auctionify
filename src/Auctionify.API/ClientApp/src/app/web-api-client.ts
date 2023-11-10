//----------------------
// <auto-generated>
//     Generated using the NSwag toolchain v13.20.0.0 (NJsonSchema v10.9.0.0 (Newtonsoft.Json v13.0.0.0)) (http://NSwag.org)
// </auto-generated>
//----------------------

/* tslint:disable */
/* eslint-disable */
// ReSharper disable InconsistentNaming

import { mergeMap, catchError  } from 'rxjs/operators';
import { Observable, throwError, of } from 'rxjs';
import { Injectable, Inject, Optional, InjectionToken } from '@angular/core';
import { HttpClient, HttpEvent, HttpHeaders, HttpResponse, HttpResponseBase } from '@angular/common/http';
import { UserRole } from './api-authorization/authorize.service';

export const API_BASE_URL = new InjectionToken('API_BASE_URL');

@Injectable({
    providedIn: 'root'
})
export class Client {
    private http: HttpClient;
    private baseUrl: string;
    protected jsonParseReviver: ((key: string, value: any) => any) | undefined = undefined;

    constructor(@Inject(HttpClient) http: HttpClient, @Optional() @Inject(API_BASE_URL) baseUrl?: string) {
        this.http = http;
        this.baseUrl = baseUrl !== undefined && baseUrl !== null ? baseUrl : "";
    }

    login(body: LoginViewModel | undefined) : Observable<LoginResponse> {
        let url_ = this.baseUrl + "/api/auth/login";

        const content_ = JSON.stringify(body);
        
        let options_ : Object = {
            body: content_,
            observe: "response",
            headers: new HttpHeaders({
                "Content-Type": "application/json",
                "Accept": "text/json"
            })
        };

        return this.http.request("post", url_, options_).pipe(mergeMap((response: any) : Observable<LoginResponse> => {
            let data: LoginResponse = {};

            if (response.body !== null) {
                data = response.body
            }

            return of(data);
        })).pipe(catchError((error) => {
            return throwError(() => error);
        }));
    }

    register(body: RegisterViewModel | undefined) : Observable<RegisterResponse> {
        let url_ = this.baseUrl + "/api/auth/register";

        const content_ = JSON.stringify(body);
        
        let options_ : any = {
            body: content_,
            observe: "response",
            headers: new HttpHeaders({
                "Content-Type": "application/json",
                "Accept": "text/json"
            })
        };

        return this.http.request("post", url_, options_).pipe(mergeMap((response: any) : Observable<RegisterResponse> => {
            let data: RegisterResponse = {};

            if (response.body !== null) {
                data = response.body
            }

            return of(data);
        }));
    }

    resetPassword(body: ResetPasswordViewModel | undefined) : Observable<ResetPasswordResponse> {
        let url_ = this.baseUrl + "/api/auth/reset-password";

        const content_ = JSON.stringify(body);
        
        let options_ : any = {
            body: content_,
            observe: "response",
            headers: new HttpHeaders({
                "Content-Type": "application/json",
                "Accept": "text/json"
            })
        };

        return this.http.request("post", url_, options_).pipe(mergeMap((response: any) : Observable<ResetPasswordResponse> => {
            let data: ResetPasswordResponse = {};

            if (response.body !== null) {
                data = response.body
            }

            return of(data);
        }));
    }

    forgetPassword(body: ForgetPasswordViewModel | undefined) : Observable<ForgetPasswordResponse> {
        let url_ = this.baseUrl + "/api/auth/forget-password";

        const content_ = JSON.stringify(body);
        
        let options_ : any = {
            body: content_,
            observe: "response",
            headers: new HttpHeaders({
                "Content-Type": "application/json",
                "Accept": "text/json"
            })
        };

        return this.http.request("post", url_, options_).pipe(mergeMap((response: any) : Observable<ForgetPasswordResponse> => {
            let data: ForgetPasswordResponse = {};

            if (response.body !== null) {
                data = response.body
            }

            return of(data);
        }));
    }
}

export interface LoginResponse {
    message?: string | undefined;
    isSuccess?: boolean;
    errors?: string[] | undefined;
    result?: TokenModel;
}

export interface TokenModel {
    accessToken: string;
    expireDate: string;
    role: UserRole;
}

export interface RegisterResponse {
    message?: string | undefined;
    isSuccess?: boolean;
    errors?: string[] | undefined;
}

export interface ForgetPasswordResponse {
    message?: string | undefined;
    isSuccess?: boolean;
    errors?: string[] | undefined;
}

export interface ResetPasswordResponse {
    message?: string | undefined;
    isSuccess?: boolean;
    errors?:  string[] | undefined;
}

export interface LoginViewModel {
    email: string;
    password: string;
}

export interface RegisterViewModel {
    firstName: string;
    email: string;
    password: string;
    confirmPassword: string;
}

export interface ForgetPasswordViewModel {
    email: string;
}

export interface ResetPasswordViewModel {
    email: string,
    token: string,
    password: string;
    confirmPassword: string;
}

export interface FileParameter {
    data: any;
    fileName: string;
}

export class ApiException extends Error {
    override message: string;
    status: number;
    response: string;
    headers: { [key: string]: any; };
    result: any;

    constructor(message: string, status: number, response: string, headers: { [key: string]: any; }, result: any) {
        super();

        this.message = message;
        this.status = status;
        this.response = response;
        this.headers = headers;
        this.result = result;
    }

    protected isApiException = true;

    static isApiException(obj: any): obj is ApiException {
        return obj.isApiException === true;
    }
}

function throwException(message: string, status: number, response: string, headers: { [key: string]: any; }, result?: any): Observable<any> {
    if (result !== null && result !== undefined)
        return throwError(() => result);
    else
        return throwError(() => new ApiException(message, status, response, headers, null));
}

function blobToText(blob: any): Observable<string> {
    return new Observable<string>((observer: any) => {
        if (!blob) {
            observer.next("");
            observer.complete();
        } else {
            let reader = new FileReader();
            reader.onload = event => {
                observer.next((event.target as any).result);
                observer.complete();
            };
            reader.readAsText(blob);
        }
    });
}