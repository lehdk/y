import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { take } from 'rxjs';
import { UserService } from 'src/app/services/user.service';

@Component({
    selector: 'app-register',
    templateUrl: './register.component.html',
    styleUrls: ['./register.component.scss'],
})
export class RegisterComponent implements OnInit {

    registerForm = new FormGroup({
        username: new FormControl<string>('', [ Validators.required ]),
        email: new FormControl<string>('', [ Validators.required ]),
        password: new FormControl<string>('', [ Validators.required]),
        repeatPassword: new FormControl<string>('', [ Validators.required]),
    },
    {
        validators: [ this.repeatPasswordValidator ]
    });

    repeatPasswordValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {

            // TODO: Fix this validation
            
            const password = control.get('password');
            const repeat = control.get('repeatPassword');            

            if(password && repeat && password.value !== repeat.value) {
                return { passwordMismatch: true };
            }

            return null;
        };
    }

    constructor(private userService: UserService, private router: Router) { }

    ngOnInit() { }

    register(): void {
        const username = this.registerForm.controls.username.value;
        const email = this.registerForm.controls.email.value;
        const password = this.registerForm.controls.password.value;

        if(!username || !email || !password) {
            return;
        }
        
        let response = this.userService.createUser(username, email, password);

        response.pipe(take(1)).subscribe(data => {
            this.router.navigate(['login']);
        });
    }
}
