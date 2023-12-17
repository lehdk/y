import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ModalController } from '@ionic/angular';
import { CreatePost } from '../models/requestModels/CreatePost';

@Component({
    selector: 'app-create-post',
    templateUrl: './create-post.component.html',
    styleUrls: ['./create-post.component.scss'],
})
export class CreatePostComponent implements OnInit {

    createPostForm = new FormGroup({
        headline: new FormControl('', [Validators.required]),
        content: new FormControl('', [Validators.required])
    });

    constructor(private modalController: ModalController) { }

    ngOnInit() { }

    post() {
        var data: CreatePost = {
            headline: this.createPostForm.controls['headline'].value!,
            content: this.createPostForm.controls['content'].value!
        };

        this.returnResponse(data);
        
    }

    cancel() {
        this.returnResponse();
    }

    returnResponse(data: CreatePost | null = null) {
        this.modalController.dismiss(data);
    }
}
