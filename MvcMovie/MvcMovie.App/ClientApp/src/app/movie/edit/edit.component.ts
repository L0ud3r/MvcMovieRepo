import { Component, OnInit } from '@angular/core';
import { faArrowLeft } from '@fortawesome/free-solid-svg-icons';
import { SharedService } from 'src/app/shared.service';

@Component({
  selector: 'app-edit',
  templateUrl: './edit.component.html',
  styleUrls: ['./edit.component.css']
})
export class EditMovieComponent implements OnInit {

  faArrowLeft = faArrowLeft

  genres: string[] = [];

  constructor(private service: SharedService) { }

  ngOnInit(): void {
    this.getAllGenres()
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

}
