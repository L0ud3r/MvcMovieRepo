import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { UserComponent } from './user/user.component';
import { MovieComponent } from './movie/movie.component';
import { GenreComponent } from './genre/genre.component';
import { SharedComponent } from './shared/shared.component';
import { FavouriteComponent } from './favourite/favourite.component';

import { CreateGenreComponent } from './genre/create/create.component'
import { CreateMovieComponent } from './movie/create/create.component'
import { DetailsMovieComponent } from './movie/details/details.component'
import { EditMovieComponent } from './movie/edit/edit.component'
import { LoginComponent } from './user/login/login.component';
import { CreateUserComponent } from './user/create/create.component'
import { RecoverPasswordComponent } from './user/recoverpassword/recoverpassword.component';
import { EditUserComponent } from './user/edit/edit.component'


const routes: Routes = [
  {
    path: 'user',
    component: UserComponent
  },
  {
    path: 'movies',
    component: MovieComponent
  },
  {
    path: 'genres',
    component: GenreComponent
  },
  {
    path: 'favourites',
    component: FavouriteComponent
  },
  {
    path: 'genres/create',
    component: CreateGenreComponent
  },
  {
    path: 'movies/create',
    component: CreateMovieComponent
  },
  {
    path: 'movies/details/:id',
    component: DetailsMovieComponent
  },
  {
    path: 'movies/edit/:id',
    component: EditMovieComponent
  },
  {
    path: 'login',
    component: LoginComponent
  },
  {
    path: 'register',
    component: CreateUserComponent
  },
  {
    path: 'recoverpassword',
    component: RecoverPasswordComponent
  },
  {
    path: 'edit',
    component: EditUserComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
