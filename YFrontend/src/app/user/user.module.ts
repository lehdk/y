import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { IonicModule } from '@ionic/angular';
import { HttpClientModule } from '@angular/common/http';
import { UserViewRoutingModule } from './user-routing.module';
import { UserviewComponent } from './userview/userview.component';

@NgModule({
    imports: [
      CommonModule,
      FormsModule,
      ReactiveFormsModule,
      IonicModule,
      HttpClientModule,
      UserViewRoutingModule
    ],
  declarations: [UserviewComponent],
})
export class UserModule { }
