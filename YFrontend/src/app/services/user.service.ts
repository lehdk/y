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

            this.router.navigate(['home']);
        });
    }
}
