import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { faArrowLeft } from '@fortawesome/free-solid-svg-icons';
import { SharedService } from 'src/app/shared.service';
import { format } from 'date-fns';

@Component({
  selector: 'app-edit',
  templateUrl: './edit.component.html',
  styleUrls: ['./edit.component.css']
})
export class EditMovieComponent implements OnInit {

  faArrowLeft = faArrowLeft

  genres: string[] = [];

  movie : any = {}

  movieEditted = {
    Id: 0,
    Title: "",
    ReleaseDate: "",
    Genre: "",
    Price: "",
    Rating: "",
  }

  date : any;

  constructor(private service: SharedService, private route: ActivatedRoute, private router: Router) { }

  ngOnInit(): void {
    this.getAllGenres()
    this.movieInfo()
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

  movieInfo():void{
    this.movie.id = this.route.snapshot.paramMap.get('id');

    this.service.movieDetails(this.movie.id).subscribe(
      data => {
        this.movie = data

        const selectElement = document.getElementById('genre') as HTMLSelectElement;

        for(let i = 0; i < selectElement.options.length; i++){
          if(selectElement.options[i].innerHTML === this.movie.genre)
            selectElement.options[i].selected = true;
        }

        const releaseDate = new Date(this.movie.releaseDate).toISOString().slice(0, 10);
        this.movie.releaseDate = releaseDate;
      },
      error => {
        alert('Error on getting movie details...')
      }
    )
  }

  movieEdit():void{
    this.movieEditted.Id = this.movie.id

    if(!this.movieEditted.Title)
      this.movieEditted.Title = this.movie.title
    if(!this.movieEditted.ReleaseDate)
      this.movieEditted.ReleaseDate = this.movie.releaseDate
    if(!this.movieEditted.Genre)
      this.movieEditted.Genre = this.movie.genre
    if(!this.movieEditted.Price)
      this.movieEditted.Price = this.movie.price
    if(!this.movieEditted.Rating)
      this.movieEditted.Rating = this.movie.rating

    this.service.movieEdit(this.movieEditted).subscribe(
      data => {
        console.log(data)
        alert('Movie editted successfully!')
        this.router.navigateByUrl('/movies').then(() =>{
          this.router.navigate([decodeURI('/movies')]);
        });
      },
      error => {
        alert('Error on editing movie...')
      }
    )
  }
}
