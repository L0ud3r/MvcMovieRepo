import { Component, OnInit } from '@angular/core';
import { faCoffee, faHeart, faPencil, faTrashCan, faEye, faPlus } from '@fortawesome/free-solid-svg-icons';
import { SharedService } from 'src/app/shared.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-movie',
  templateUrl: './movie.component.html',
  styleUrls: ['./movie.component.css']
})

export class MovieComponent implements OnInit {

  faCoffee = faCoffee
  faHeart = faHeart
  faPencil = faPencil
  faTrashCan = faTrashCan
  faEye = faEye
  faPlus = faPlus

  genres: string[] = [];
  movies: any[] = []
  idMovie: number = 0

  model = {
    offset: 0,
    limit: 0,
    search: [
      {
        name: "Genre",
        value: ""
      },
      {
        name: "Title",
        value: ""
      }
    ]
  }

  constructor(private service: SharedService, private router : Router) { }

  ngOnInit(): void {
    this.paginate()
    this.getUsedGenres()
  }

  getUsedGenres(): void {
    this.service.usedGenres().subscribe(
      (data: { disabled: boolean, group: any, selected: boolean, text: string, value: any }[]) => {
        this.genres = data.map(genre => genre.text);
      },
      error => {
        alert('Error on obtaining movie genres...');
      }
    );
  }

  paginate():void{
    this.service.paginateMovies(this.model).subscribe(
      (data : any) => {
        this.movies = data.rows;
      },
      error => {
        alert('Error on obtaining movies...');
      }
    );
  }

  saveMovieId(id:number):void{
    this.idMovie = id
  }

  deleteMovie():void{
    this.service.movieDelete(this.idMovie).subscribe(
      data => {
        this.paginate()
      },
      error => {
        alert('Error on deleting genre...')
      }
    )
  }

  addFavourites(id:number):void{
    this.service.updateFavourite(id, localStorage.getItem('token')!).subscribe(
      data => {
        alert("Movie added to favorites!")
      },
      error => {
        alert("Error on adding movie to favorites...")
      }
    )
  }
}
