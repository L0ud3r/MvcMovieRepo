import { Component, OnInit } from '@angular/core';
import { SharedService } from 'src/app/shared.service';
import { faFilm, faTicket, faMask, faBars, faAngleUp, faUser, faSignOutAlt } from '@fortawesome/free-solid-svg-icons';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent {
  faFilm = faFilm
  faTicket = faTicket
  faMask = faMask
  faBars = faBars
  faAngleUp = faAngleUp
  faUser = faUser
  faSignOutAlt = faSignOutAlt

  conta:any = {}

  constructor(private service: SharedService, private router : Router) { }

  ngOnInit(): void {
    this.userInfo()
  }

  userInfo(): void {
    this.service.getUserbyToken(localStorage.getItem('token')).subscribe(
      data => {
        this.conta = data;
      },
      error => {
        this.conta.Username = "Sign in";
      }
    );
  }

}
