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

  account:any = {}
  isSignedIn:boolean = false
  currentRoute: string = '';


  constructor(private service: SharedService, private router : Router) { }

  ngOnInit(): void {
    this.userInfo()
    this.currentRoute = this.router.url;
  }

  userInfo(): void {
    this.service.getUserbyToken(localStorage.getItem('token')).subscribe(
      data => {
        this.account = data;
        this.isSignedIn = true;
      },
      error => {
        this.account.Username = "Sign in";
      }
    );
  }

  logout(): void{
    localStorage.removeItem('token');
    this.router.navigateByUrl('/login').then(() =>{
      location.reload();
      this.router.navigate([decodeURI('/login')]);
    });
  }
}
