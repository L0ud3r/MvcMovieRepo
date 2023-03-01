import { Component, OnInit } from '@angular/core';
import { SharedService } from 'src/app/shared.service';
import { Router } from '@angular/router';
import { faArrowLeft } from '@fortawesome/free-solid-svg-icons';


@Component({
  selector: 'app-create',
  templateUrl: './create.component.html',
  styleUrls: ['./create.component.css']
})
export class CreateMovieComponent implements OnInit {

  faArrowLeft = faArrowLeft

  movie = {
    Title: "",
    ReleaseDate: "",
    Genre: "",
    Price: "",
    Rating: "",
  }

  genres: string[] = [];

  constructor(private service: SharedService, private router : Router) { }

  ngOnInit(): void {
    this.getAllGenres();
  }

  createMovie(): void {
    this.service.movieCreate(this.movie).subscribe(
      data => {
        alert('Movie created sucessfully!')
        this.router.navigateByUrl('/movies').then(() =>{
          this.router.navigate([decodeURI('/movies')]);
        });
      },
      error => {
        alert('Error on creating movie...');
      }
    );
  }

  getAllGenres(): void {
    this.service.listGenres().subscribe(
      (data: { disabled: boolean, group: any, selected: boolean, text: string, value: any }[]) => {
        this.genres = data.map(genre => genre.text);
      },
      error => {
        alert('Error on obtaining movie genres...');
      }
    );
  }

  /*getAllGenres(): void {
    this.service.listGenres().subscribe(
      data => {
        for(let i = 0; i < data.length; i++) {
          this.genres.push(data[i].text)
        }

        //this.genres = data.map(genre => genre.text);
        console.log(this.genres[0])
      },
      error => {
        alert('Error on obtaining movie genres...');
      }
    );
  }*/
}
