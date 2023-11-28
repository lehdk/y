import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { IonicModule } from '@ionic/angular';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { LoginPageRoutingModule } from './login-routing.module';
import { LoginPageComponent } from './login.page/login.page.component';
import { RegisterComponent } from './register/register.component';



@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        IonicModule,
        LoginPageRoutingModule
    ],
    declarations: [LoginPageComponent, RegisterComponent]
})
export class LoginModule { }
