import { Component, OnInit } from '@angular/core';
import { ModalController } from '@ionic/angular';
import { take } from 'rxjs';
import { CreatePostComponent } from 'src/app/create-post/create-post.component';
import { YPost } from 'src/app/models/YPost';
import { CreatePost } from 'src/app/models/requestModels/CreatePost';
import { PostService } from 'src/app/services/post.service';

@Component({
    selector: 'app-postview',
    templateUrl: './postview.component.html',
    styleUrls: ['./postview.component.scss'],
})
export class PostviewComponent implements OnInit {

    currentPage: number = 1;
    pageSize: number = 1;

    isLoading: boolean = false;

    selectedView: string = "latest";

    posts: YPost[] = [];

    constructor(private postService: PostService, private modalController: ModalController) { }

    ngOnInit() {
        this.postService.posts.subscribe(next => {
            this.posts = next;
        });
    }

    selectView(view: string): void {
        if(this.selectedView === view) 
            return;

        this.selectedView = view;
    }

    loadMorePosts() {
        this.postService.loadMorePosts();
    }

    async createPost() {
        const modal = await this.modalController.create({
            component: CreatePostComponent
        });

        modal.onDidDismiss().then(data => {
            const postData: CreatePost | null = data.data;
            
            if(!postData) return;

            this.postService.createPost(postData);
        });

        await modal.present();
    }
}
