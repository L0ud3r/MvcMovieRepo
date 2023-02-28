import { Component, OnInit } from '@angular/core';
import { faCoffee } from '@fortawesome/free-solid-svg-icons';
import { SharedService } from 'src/app/shared.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-movie',
  templateUrl: './movie.component.html',
  styleUrls: ['./movie.component.css']
})

export class MovieComponent implements OnInit {

  faCoffee = faCoffee
  genres: string[] = [];
  movies: any[] = []

  model = {
    offset: 0,
    limit: 0,
    search: [
      {
        name: "",
        value: ""
      }
    ]
  }

  constructor(private service: SharedService, private router : Router) { }

  ngOnInit(): void {
    this.paginate()
    this.getAllGenres()
  }

  getAllGenres(): void {
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
}
