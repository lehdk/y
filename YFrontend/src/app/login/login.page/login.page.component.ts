import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { UserService } from 'src/app/services/user.service';

@Component({
    selector: 'app-login.page',
    templateUrl: './login.page.component.html',
    styleUrls: ['./login.page.component.scss'],
})
export class LoginPageComponent implements OnInit {

    loginForm: FormGroup = new FormGroup({
        username: new FormControl<string>('', [ Validators.required ]),
        password: new FormControl<string>('', [ Validators.required ])
    });

    constructor(private userService: UserService) { }

    ngOnInit() { }

    login(): void {
        const username: string = this.loginForm.controls['username'].value;
        const password: string = this.loginForm.controls['password'].value;

        if(!username || !password)
            return;

        this.userService.login(username, password);
    }
}
