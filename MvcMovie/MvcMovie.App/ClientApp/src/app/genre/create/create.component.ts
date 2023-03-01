import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { faArrowLeft } from '@fortawesome/free-solid-svg-icons';
import { SharedService } from 'src/app/shared.service';

@Component({
  selector: 'app-create',
  templateUrl: './create.component.html',
  styleUrls: ['./create.component.css']
})
export class CreateGenreComponent implements OnInit {

  faArrowLeft = faArrowLeft

  genre = {
    Name: ""
  }

  constructor(private service: SharedService, private router: Router) { }

  ngOnInit(): void {
  }

  createGenre():void{
      this.service.createGenre(this.genre).subscribe(
        data => {
          alert("Movie genre created successfully!")
          this.router.navigateByUrl('/genres').then(() =>{
            this.router.navigate([decodeURI('/genres')]);
          });
        },
        error => {
          alert("Error on creating movie genre...\nCheck if this genre already exists!")
        }
      )
  }

}
