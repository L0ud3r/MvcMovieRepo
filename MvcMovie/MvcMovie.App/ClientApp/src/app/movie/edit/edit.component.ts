import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
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

  constructor(private service: SharedService, private router: ActivatedRoute) { }

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
    const idMovie = this.router.snapshot.paramMap.get('id');

    this.service.movieDetails(idMovie).subscribe(
      data => {
        this.movie = data

        const selectElement = document.getElementById('genre') as HTMLSelectElement;

        for(let i = 0; i < selectElement.options.length; i++){
          if(selectElement.options[i].innerHTML === this.movie.genre)
            selectElement.options[i].selected = true;
        }

        const date = new Date(this.movie.releaseDate);
        const formattedDate = format(date, 'dd/MM/yyyy');
      },
      error => {
        alert('Error on getting movie details...')
      }
    )
  }

}
