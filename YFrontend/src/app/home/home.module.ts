import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { IonicModule } from '@ionic/angular';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HomePage } from './home.page';

import { HomePageRoutingModule } from './home-routing.module';
import { PostviewComponent } from './postview/postview.component';
import { PostCardComponent } from '../post-card/post-card.component';
import { CreatePostComponent } from '../create-post/create-post.component';
import { HttpClientModule } from '@angular/common/http';


@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    IonicModule,
    HomePageRoutingModule,
    ReactiveFormsModule,
    HttpClientModule
  ],
  declarations: [HomePage, PostviewComponent, PostCardComponent, CreatePostComponent]
})
export class HomePageModule {}
