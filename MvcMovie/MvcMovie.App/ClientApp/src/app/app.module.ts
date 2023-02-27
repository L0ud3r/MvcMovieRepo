import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

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
import { CreateUserComponent } from './user/create/create.component'
import { LoginComponent } from './user/login/login.component'
import { EditUserComponent } from './user/edit/edit.component'
import { RecoverPasswordComponent } from './user/recoverpassword/recoverpassword.component'

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    CounterComponent,
    FetchDataComponent,
    UserComponent,
    MovieComponent,
    GenreComponent,
    SharedComponent,
    FavouriteComponent,
    CreateGenreComponent,
    CreateMovieComponent,
    DetailsMovieComponent,
    EditMovieComponent,
    CreateUserComponent,
    LoginComponent,
    EditUserComponent,
    RecoverPasswordComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      //{ path: '', component: HomeComponent, pathMatch: 'full' },
      { path: '', component: LoginComponent, pathMatch: 'full' },
      { path: 'counter', component: CounterComponent },
      { path: 'fetch-data', component: FetchDataComponent },
    ])
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
