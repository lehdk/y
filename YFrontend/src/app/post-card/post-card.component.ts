import { Component, Input, OnInit } from '@angular/core';
import { YPost } from '../models/YPost';
import { YUser } from '../models/YUser';
import { UserService } from '../services/user.service';
import { take } from 'rxjs';
import { Router } from '@angular/router';

@Component({
    selector: 'app-post-card',
    templateUrl: './post-card.component.html',
    styleUrls: ['./post-card.component.scss'],
})
export class PostCardComponent implements OnInit {

    @Input({ required: true }) post!: YPost;

    createdBy: YUser | null = null;
    loggedInUser = this.userService.getLoggedInUser();

    constructor(private userService: UserService, private router: Router) { }

    ngOnInit() {
        this.userService.getUserById(this.post.userId).pipe(take(1)).subscribe(user => {
            this.createdBy = user;
        });
    }

    goToUser(username: string) {
        this.router.navigate(['user', username]);
    }
}
