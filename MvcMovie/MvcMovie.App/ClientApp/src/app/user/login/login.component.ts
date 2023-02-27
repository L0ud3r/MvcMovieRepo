import { Component, OnInit } from '@angular/core';
import { SharedService } from 'src/app/shared.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  conta:any= {
    Email: "",
    Password: ""
  };

  constructor(private service: SharedService, private router : Router) { }

  ngOnInit(): void {
  }

  login(): void {
    console.log(this.conta);

    this.service.login(this.conta).subscribe(
      data => {
        console.log(data);
        localStorage.setItem('token', data.toString());
        this.router.navigateByUrl('/movies').then(() =>{
          this.router.navigate([decodeURI('/movies')]);
        });
      },
      error => {
        alert('Login failed. Please check your credentials and try again.');
      }
    );
   }
}
