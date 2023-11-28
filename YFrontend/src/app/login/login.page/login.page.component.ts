import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';

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

    constructor() { }

    ngOnInit() { }

    login() {
        console.log("hey hey");
    }
}
