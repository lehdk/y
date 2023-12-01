import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { YPost } from '../models/YPost';

@Injectable({
    providedIn: 'root'
})
export class PostService {
    
    readonly url: string = "https://localhost:7130/api/posts";

    constructor(private http: HttpClient) { }

    loadPosts(page: number, pageSize: number) {
        return this.http.get<YPost[]>(`${this.url}?page=${page}&pageSize=${pageSize}`);
    }
}
