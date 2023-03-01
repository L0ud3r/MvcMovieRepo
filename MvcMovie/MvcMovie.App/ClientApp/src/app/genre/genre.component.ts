import { Component, OnInit } from '@angular/core';
import { faTrash, faPlus } from '@fortawesome/free-solid-svg-icons';
import { SharedService } from '../shared.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-genre',
  templateUrl: './genre.component.html',
  styleUrls: ['./genre.component.css']
})
export class GenreComponent implements OnInit {

  faTrash = faTrash
  faPlus = faPlus

  model = {
    offset: 0,
    limit: 0,
    search: [
      {
        name: "Genre",
        value: ""
      }
    ]
  }

  genres : any[] = []
  genreId : number = 0

  constructor(private service: SharedService, private router: Router) { }

  ngOnInit(): void {
    this.paginate()
  }

  paginate():void{
    this.service.paginateGenres(this.model).subscribe(
      (data : any) => {
        this.genres = data.rows;
        console.log(this.genres);
      },
      error => {
        alert('Error on obtaining movies...');
      }
    )
  }

  saveGenreId(id: number): void {
    this.genreId = id
  }

  deleteGenre():void{
    this.service.deleteGenre(this.genreId).subscribe(
      data => {
        this.paginate()
      },
      error => {
        alert('Error on deleting genre...')
      }
    )
  }
}
