import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Route, Router } from '@angular/router';
import { faArrowLeft, faPen } from '@fortawesome/free-solid-svg-icons';
import { SharedService } from 'src/app/shared.service';

@Component({
  selector: 'app-details',
  templateUrl: './details.component.html',
  styleUrls: ['./details.component.css']
})
export class DetailsMovieComponent implements OnInit {

  faArrowLeft = faArrowLeft
  faPen = faPen

  movie : any = {}

  constructor(private service: SharedService, private router: ActivatedRoute) { }

  ngOnInit(): void {
    this.movieDetails()
  }

  movieDetails():void{
    const idMovie = this.router.snapshot.paramMap.get('id');

    this.service.movieDetails(idMovie).subscribe(
      data => {
        this.movie = data
      },
      error => {
        alert('Error on getting movie details...')
      }
    )
  }

}
