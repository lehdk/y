import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { UserService } from './services/user.service';
import { YUser } from './models/YUser';

@Component({
  selector: 'app-root',
  templateUrl: 'app.component.html',
  styleUrls: ['app.component.scss'],
})
export class AppComponent implements OnInit {

  loggedInUser: YUser | null = null;
  
  constructor(private router: Router, private userService: UserService) {}
  
  ngOnInit() {
    this.userService.getLoggedInUser().subscribe(next => {
      this.loggedInUser = next;
    });
  }

  home() {
    this.router.navigate(['']);
  }

  goToLoggedIn() {
    this.router.navigate(['user', this.loggedInUser?.username]);
  }
}
