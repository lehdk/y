import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { YPost } from 'src/app/models/YPost';
import { YUser } from 'src/app/models/YUser';
import { PostService } from 'src/app/services/post.service';
import { UserService } from 'src/app/services/user.service';

@Component({
    selector: 'app-userview',
    templateUrl: './userview.component.html',
    styleUrls: ['./userview.component.scss'],
})
export class UserviewComponent implements OnInit, OnDestroy {

    page: number = 1;
    pageSize: number = 2;
    isLoading: boolean = false;

    currentUser: YUser | null = null;

    _onDestroy: Subscription[] = [];

    posts: YPost[] = [];

    constructor(private route: ActivatedRoute, private router: Router, private userService: UserService, private postService: PostService) {
    }

    ngOnInit() {
        const username = this.route.snapshot.paramMap.get('username');

        if(!username) {
            this.router.navigate(['']);
            return;
        }

        this.userService.getUser(username).subscribe(user => {
            if(!user) {
                this.router.navigate(['']);
                return;
            }

            this.currentUser = user;
        });
    }

    loadMorePosts() {
        if(!this.currentUser || this.isLoading)
            return;

        this.isLoading = true;

        this.postService.getPostByUser(this.currentUser.guid, this.page, this.pageSize).subscribe(next => {
            this.posts = this.posts.concat(next);
            this.isLoading = false;
            this.page++;
        });
    }

    ngOnDestroy(): void {
        for(let s of this._onDestroy) {
            s.unsubscribe();
        }
    }
}
