import { Component, OnInit } from '@angular/core';
import { SharedService } from 'src/app/shared.service';
import { Router } from '@angular/router';
import { faFilm } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-create',
  templateUrl: './create.component.html',
  styleUrls: ['./create.component.css']
})
export class CreateUserComponent implements OnInit {

  faFilm = faFilm

  conta:any= {
    Username: "",
    Email: "",
    Password: ""
  };

  confirmPassword : String = ""

  constructor(private service: SharedService, private router : Router) { }

  ngOnInit(): void {
  }

  registerUser() : void{
    this.service.register(this.conta).subscribe(
      data => {
        alert("Account created successfully!")
        this.router.navigateByUrl('/login').then(() =>{
          this.router.navigate([decodeURI('/login')]);
        });
      },
      error => {
        alert('Error on creating account...');
    });
  }

}
