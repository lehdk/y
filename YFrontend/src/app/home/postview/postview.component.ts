import { Component, OnInit } from '@angular/core';
import { take } from 'rxjs';
import { YPost } from 'src/app/models/YPost';
import { PostService } from 'src/app/services/post.service';

@Component({
    selector: 'app-postview',
    templateUrl: './postview.component.html',
    styleUrls: ['./postview.component.scss'],
})
export class PostviewComponent implements OnInit {

    currentPage: number = 1;
    pageSize: number = 1;

    isLoading: boolean = false;

    selectedView: string = "latest";

    posts: YPost[] = [];

    constructor(private postService: PostService) { }

    ngOnInit() { }

    selectView(view: string): void {
        if(this.selectedView === view) 
            return;

        this.selectedView = view;
    }

    loadMorePosts() {
        if(this.isLoading)
            return;
        
        this.isLoading = true;

        this.postService.loadPosts(this.currentPage, this.pageSize).pipe(take(1)).subscribe({
            next: posts => {
                this.posts = this.posts.concat(posts);
    
                this.isLoading = false;
                this.currentPage++;
            },
            error: err => {
                this.isLoading = false;
            }
        });
    }
}
