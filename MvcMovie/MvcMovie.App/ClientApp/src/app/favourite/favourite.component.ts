import { Component, OnInit } from '@angular/core';
import { SharedService } from '../shared.service';
import { Router } from '@angular/router';
import { faHeartBroken, faEye } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-favourite',
  templateUrl: './favourite.component.html',
  styleUrls: ['./favourite.component.css']
})
export class FavouriteComponent implements OnInit {

  constructor(private service: SharedService, private router: Router) { }

  faHeartBroken = faHeartBroken
  faEye = faEye

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
    this.service.paginateFavourites(this.model, localStorage.getItem('token')!).subscribe(
      (data : any) => {
        this.movies = data.rows;
        console.log(this.movies)
      },
      error => {
        alert('Error on obtaining movies...');
      }
    );
  }
}
