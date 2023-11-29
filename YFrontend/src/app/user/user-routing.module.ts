import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { UserviewComponent } from './userview/userview.component';

const routes: Routes = [
    { path: ':username', component: UserviewComponent }
];

@NgModule({
  imports: [
    RouterModule.forChild(routes),
],
  exports: [RouterModule]
})
export class UserViewRoutingModule {}
