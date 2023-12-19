import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { YPost } from '../models/YPost';
import { CreatePost } from '../models/requestModels/CreatePost';
import { BehaviorSubject, take } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class PostService {
    
    readonly url: string = "https://localhost:7130/api/posts";

    currentPage: number = 1;
    pageSize: number = 5;

    isLoading: boolean = false;

    selectedView: string = "latest";
    
    // posts: YPost[] = [];

    public posts: BehaviorSubject<YPost[]> = new BehaviorSubject<YPost[]>([]);

    constructor(private http: HttpClient) { }

    getPostByUser(userId: string, page: number, pageSize: number) {
        return this.http.get<YPost[]>(`${this.url}?page=${page}&pageSize=${pageSize}&userId=${userId}`);
    }

    loadMorePosts() {
        if(this.isLoading)
            return;
        
        this.isLoading = true;

        this.loadPosts(this.currentPage, this.pageSize).pipe(take(1)).subscribe({
            next: posts => {
                const current = this.posts.getValue();
                this.posts.next(current.concat(posts));
    
                this.isLoading = false;
                this.currentPage++;
            },
            error: err => {
                this.isLoading = false;
            }
        });
    }

    clear() {
        this.posts.next([]);
    }

    loadPosts(page: number, pageSize: number) {
        return this.http.get<YPost[]>(`${this.url}?page=${page}&pageSize=${pageSize}`);
    }

    createPost(data: CreatePost) {
        let response = this.http.post<YPost>(this.url, data);

        response.subscribe(res => {
            var temp = this.posts.getValue();
            temp.unshift(res);

            this.posts.next(temp);
        });
    }
}
