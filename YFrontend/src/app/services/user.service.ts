import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { YUser } from '../models/YUser';
import { BehaviorSubject, Observable, take } from 'rxjs';
import { Router } from '@angular/router';
import { GetTokenResponse } from '../models/responseModels/GetTokenResponse';

@Injectable({
    providedIn: 'root'
})
export class UserService {

    private readonly LOCAL_STORATE_KEY = "TOKEN";

    readonly url: string = "https://localhost:7130/api/UserProfile";

    constructor(private http: HttpClient, private router: Router) { }

    private loggedInUser: BehaviorSubject<YUser | null> = new BehaviorSubject<YUser | null>(null);

    createUser(username: string, email: string, password: string) {

        const body = {
            username: username,
            email: email,
            password: password
        };

        let response = this.http.post<YUser>(this.url, body);

        return response;
    }

    getLoggedInUser(): BehaviorSubject<YUser | null> {
        if(!this.loggedInUser.value) {
            const username = this.getCurrentUsername();
             this.getUser(username).pipe(take(1)).subscribe({
                next: user => {
                    this.loggedInUser.next(user);
                },

                error: err => {
                    this.loggedInUser.next(null);
                }
             });
        }

        return this.loggedInUser;
    }

    getUser(username: string) {
        return this.http.get<YUser>(`${this.url}/${username}`);
    }
    
    getUserById(userId: string) {
        return this.http.get<YUser>(`${this.url}/id/${userId}`);
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
            this.navigateToLogin();
            return "";
        }
        
        const isTokenExpired = this.isTokenExpired(token);

        if(isTokenExpired) {
            this.navigateToLogin();
            return "";
        }

        return token;
    }

    private isTokenExpired(token: string): boolean {
        const expirationTime = (JSON.parse(atob(token.split('.')[1]))).exp * 1000;
        const currentTime = new Date().getTime();
        
        return expirationTime <= currentTime;
    }
    
    private navigateToLogin() {
        this.router.navigate(['login']);
    }
    
    private getCurrentUsername(): string {
        const token: string | null = localStorage.getItem(this.LOCAL_STORATE_KEY);
        
        if(!token) {
            this.navigateToLogin();
            return "";
        }

        
        const isExpired = this.isTokenExpired(token);
        
        if(isExpired) {
            this.navigateToLogin();
            return "";
        }

        const username = (JSON.parse(atob(token.split('.')[1])))['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'];
        
        return username;
    }
}
