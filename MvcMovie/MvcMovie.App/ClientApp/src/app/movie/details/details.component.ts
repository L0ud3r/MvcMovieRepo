import { Component, OnInit } from '@angular/core';
import { faArrowLeft, faPen } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-details',
  templateUrl: './details.component.html',
  styleUrls: ['./details.component.css']
})
export class DetailsMovieComponent implements OnInit {

  faArrowLeft = faArrowLeft
  faPen = faPen

  constructor() { }

  ngOnInit(): void {
  }

}
