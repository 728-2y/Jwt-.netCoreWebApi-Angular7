import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {LoginComponent} from './authorization/login/login.component';
import {SliderComponent} from './home/slider/slider.component';

const routes: Routes = [
  {path: 'login', component: LoginComponent},
  {path: 'home', component: SliderComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
