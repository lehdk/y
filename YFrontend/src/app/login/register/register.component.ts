import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';

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

    constructor() { }

    ngOnInit() { }

    register(): void {
        // TODO: Register here
    }
}
