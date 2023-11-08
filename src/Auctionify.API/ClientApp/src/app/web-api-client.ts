//----------------------
// <auto-generated>
//     Generated using the NSwag toolchain v13.20.0.0 (NJsonSchema v10.9.0.0 (Newtonsoft.Json v13.0.0.0)) (http://NSwag.org)
// </auto-generated>
//----------------------

/* tslint:disable */
/* eslint-disable */
// ReSharper disable InconsistentNaming

import { mergeMap, catchError } from 'rxjs/operators';
import { Observable, throwError, of } from 'rxjs';
import { Injectable, Inject, Optional, InjectionToken } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { UserRole } from './api-authorization/authorize.service';
import { CreateLotModel } from './models/lots/lot-models';

export const API_BASE_URL = new InjectionToken('API_BASE_URL');

@Injectable({
    providedIn: 'root',
})
export class Client {
    private http: HttpClient;
    private baseUrl: string;
    protected jsonParseReviver: ((key: string, value: any) => any) | undefined =
        undefined;

    constructor(
        @Inject(HttpClient) http: HttpClient,
        @Optional() @Inject(API_BASE_URL) baseUrl?: string
    ) {
        this.http = http;
        this.baseUrl = baseUrl !== undefined && baseUrl !== null ? baseUrl : '';
    }

    login(body: LoginViewModel | undefined): Observable<LoginResponse> {
        let url_ = this.baseUrl + '/api/auth/login';

        const content_ = JSON.stringify(body);

        let options_: Object = {
            body: content_,
            observe: 'response',
            headers: new HttpHeaders({
                'Content-Type': 'application/json',
                Accept: 'text/json',
            }),
        };

        return this.http
            .request('post', url_, options_)
            .pipe(
                mergeMap((response: any): Observable<LoginResponse> => {
                    let data: LoginResponse = {};

                    if (response.body !== null) {
                        data = response.body;
                    }

                    return of(data);
                })
            )
            .pipe(
                catchError((error) => {
                    return throwError(() => error);
                })
            );
    }

    loginWithGoogle(credentials: string): Observable<any> {
        const header = new HttpHeaders().set(
            'Content-type',
            'application/json'
        );
        let url_ = this.baseUrl + 'api/auth/login-with-google';

        return this.http.post(url_, JSON.stringify(credentials), {
            headers: header,
            withCredentials: true,
        });
    }

    register(
        body: RegisterViewModel | undefined
    ): Observable<RegisterResponse> {
        let url_ = this.baseUrl + '/api/auth/register';

        const content_ = JSON.stringify(body);

        let options_: any = {
            body: content_,
            observe: 'response',
            headers: new HttpHeaders({
                'Content-Type': 'application/json',
                Accept: 'text/json',
            }),
        };

        return this.http.request('post', url_, options_).pipe(
            mergeMap((response: any): Observable<RegisterResponse> => {
                let data: RegisterResponse = {};

                if (response.body !== null) {
                    data = response.body;
                }

                return of(data);
            })
        );
    }

    getAllCategories(): Observable<CategoryResponse[]> {
        let url_ = this.baseUrl + '/api/categories';

        let options_: any = {
            observe: 'response',
            headers: new HttpHeaders({
                'Content-Type': 'application/json',
                Accept: 'text/json',
            }),
        };

        return this.http.request('get', url_, options_).pipe(
            mergeMap((response: any): Observable<CategoryResponse[]> => {
                let data: CategoryResponse[] = [];

                if (response.body !== null) {
                    data = response.body;
                }

                return of(data);
            })
        );
    }

    getAllCurrencies(): Observable<CurrencyResponse[]> {
        let url_ = this.baseUrl + '/api/currencies';

        let options_: any = {
            observe: 'response',
            headers: new HttpHeaders({
                'Content-Type': 'application/json',
                Accept: 'text/json',
            }),
        };

        return this.http.request('get', url_, options_).pipe(
            mergeMap((response: any): Observable<CurrencyResponse[]> => {
                let data: CurrencyResponse[] = [];

                if (response.body !== null) {
                    data = response.body;
                }

                return of(data);
            })
        );
    }

    createLot(body: CreateLotModel): Observable<any> {
        let url_ = this.baseUrl + '/api/lots';

        let formData = new FormData();

        formData.append('title', body.title);
        formData.append('description', body.description);
        formData.append('city', body.city);
        formData.append('address', body.address);
        formData.append('country', body.country);
        formData.append('startDate', new Date(body.startDate!).toISOString());
        formData.append('endDate', new Date(body.endDate!).toISOString());
        formData.append('startingPrice', body.startingPrice?.toString() ?? '');
        formData.append('categoryId', body.categoryId?.toString() ?? '');
        formData.append('currencyId', body.currencyId?.toString() ?? '');
        formData.append('isDraft', body.isDraft?.toString()!);

        if (body.photos !== null) {
            for (const photo of body.photos) {
                formData.append('photos', photo);
            }
        }

        if (body.additionalDocuments !== null) {
            for (const file of body.additionalDocuments) {
                formData.append('additionalDocuments', file);
            }
        }

        let options_: any = {
            body: formData,
        };

        return this.http.request('post', url_, options_).pipe(
            catchError((error) => {
                return throwError(() => error.error);
            })
        );
    }

    getOneLotForSeller(id: number): Observable<SellerGetLotResponse> {
        let url_ = this.baseUrl + `/api/lots/${id}/sellers`;

        let options_: any = {
            observe: 'response',
            headers: new HttpHeaders({
                'Content-Type': 'application/json',
                Accept: 'text/json',
            }),
        };

        return this.http.request('get', url_, options_).pipe(
            mergeMap((response: any): Observable<SellerGetLotResponse> => {
                if (response.body !== null) {
                    let data: SellerGetLotResponse = response.body;

                    return of(data);
                } else return throwError(() => new Error('data is empty!'));
            })
        );
    }
}

export interface CategoryDto {
    id: number;
    name: string;
    parentCategoryId: number | null;
}

export interface LotStatusDto {
    id: number;
    name: string;
}

export interface LocationDto {
    id: number;
    city: string;
    country: string;
    address: string;
    state: string | null;
}

export interface CurrencyDto {
    id: number;
    code: string;
}

export interface BidDto {
    id: number;
    buyerId: number;
    newPrice: number;
    timeStamp: Date;
    buyer: UserDto;
}

export interface UserDto {
    id: number;
    firstName: string;
    lastName: string;
    phoneNumber: string;
    email: string;
}

export interface SellerGetLotResponse {
    id: number;
    title: string;
    description: string;
    startingPrice: number | null;
    startDate: Date | null;
    endDate: Date | null;
    photosUrl: string[] | null;
    additionalDocumentsUrl: string[] | null;
    category: CategoryDto;
    lotStatus: LotStatusDto;
    location: LocationDto;
    currency: CurrencyDto;
    bids: BidDto[];
}

export interface CreateLotResponse {
    title: string;
    description: string;
    startingPrice: number | null;
    startDate: Date | null;
    endDate: Date | null;
    categoryId: number | null;
    city: string;
    state: string | null;
    country: string | null;
    address: string | null;
    currencyId: number | null;
    photos: File[] | null;
    additionalDocuments: File[] | null;
}

export interface CurrencyResponse {
    id: number;
    code: string;
}

export interface CategoryResponse {
    id: number;
    name: string;
    children: CategoryResponse[];
    parentCategoryId: number | null;
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

export interface LoginViewModel {
    email: string;
    password: string;
}

export interface RegisterViewModel {
    email: string;
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
    headers: { [key: string]: any };
    result: any;

    constructor(
        message: string,
        status: number,
        response: string,
        headers: { [key: string]: any },
        result: any
    ) {
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

function throwException(
    message: string,
    status: number,
    response: string,
    headers: { [key: string]: any },
    result?: any
): Observable<any> {
    if (result !== null && result !== undefined)
        return throwError(() => result);
    else
        return throwError(
            () => new ApiException(message, status, response, headers, null)
        );
}

function blobToText(blob: any): Observable<string> {
    return new Observable<string>((observer: any) => {
        if (!blob) {
            observer.next('');
            observer.complete();
        } else {
            let reader = new FileReader();
            reader.onload = (event) => {
                observer.next((event.target as any).result);
                observer.complete();
            };
            reader.readAsText(blob);
        }
    });
}
