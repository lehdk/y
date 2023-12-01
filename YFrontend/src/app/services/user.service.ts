import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { YUser } from '../models/YUser';
import { take } from 'rxjs';
import { Router } from '@angular/router';
import { GetTokenResponse } from '../models/responseModels/GetTokenResponse';

@Injectable({
    providedIn: 'root'
})
export class UserService {

    private readonly LOCAL_STORATE_KEY = "TOKEN";

    readonly url: string = "https://localhost:7130/api/UserProfile";

    constructor(private http: HttpClient, private router: Router) { }

    createUser(username: string, email: string, password: string) {

        const body = {
            username: username,
            email: email,
            password: password
        };

        let response = this.http.post<YUser>(this.url, body);

        return response;
    }

    getUser(username: string) {
        return this.http.get<YUser>(`${this.url}/${username}`);
    }

    login(username: string, password: string) {

        const body = {
            username: username,
            password: password
        };

        let response = this.http.post<GetTokenResponse>(`${this.url}/token`, body);

        response.pipe(take(1)).subscribe(tokenResponse => {
            if (!tokenResponse) {
                this.router.navigate(['login']);
            }

            localStorage.setItem(this.LOCAL_STORATE_KEY, tokenResponse.token);

            this.router.navigate(['']);
        });
    }

    getToken(): string {
        const token: string | null = localStorage.getItem(this.LOCAL_STORATE_KEY);

        
        if(!token) {
            this.router.navigate(['login']);
            return "";
        }
        
        const isTokenExpired = this.isTokenExpired(token);

        if(isTokenExpired) {
            this.router.navigate(['login']);
            return "";
        }

        return token;
    }

    private isTokenExpired(token: string): boolean {
        const expirationTime = (JSON.parse(atob(token.split('.')[1]))).exp * 1000;
        const currentTime = new Date().getTime();

        return expirationTime <= currentTime;
    }
}
