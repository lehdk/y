import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { YUser } from 'src/app/models/YUser';
import { UserService } from 'src/app/services/user.service';

@Component({
    selector: 'app-userview',
    templateUrl: './userview.component.html',
    styleUrls: ['./userview.component.scss'],
})
export class UserviewComponent implements OnInit, OnDestroy {

    currentUser: YUser | null = null;

    _onDestroy: Subscription[] = [];

    constructor(private route: ActivatedRoute, private router: Router, private userService: UserService) {
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

    ngOnDestroy(): void {
        for(let s of this._onDestroy) {
            s.unsubscribe();
        }
    }

}
