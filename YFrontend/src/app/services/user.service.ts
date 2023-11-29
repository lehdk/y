import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { YUser } from '../models/YUser';

@Injectable({
    providedIn: 'root'
})
export class UserService {

    readonly url: string = "https://localhost:7130/api/UserProfile";

    constructor(private http: HttpClient) { }

    createUser(username: string, email: string, password: string) {

        const body = {
            username: username,
            email: email,
            password: password
        };

        let response = this.http.post<YUser>(this.url, body);

        return response;
    }
}
