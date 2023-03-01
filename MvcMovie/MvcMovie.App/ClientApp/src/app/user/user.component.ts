import { Component, OnInit } from '@angular/core';
import { faHome, faTrash, faPen } from '@fortawesome/free-solid-svg-icons';
import { SharedService } from '../shared.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.css']
})
export class UserComponent implements OnInit {

  faHome = faHome
  faTrash = faTrash
  faPen = faPen

  account:any = {}

  constructor(private service: SharedService, private router: Router) { }

  ngOnInit(): void {
    this.userInfo()
  }

  userInfo(): void {
    this.service.getUserbyToken(localStorage.getItem('token')).subscribe(
      data => {
        this.account = data;
      },
      error => {
        this.account.Username = "Sign in";
      }
    );
  }

  deleteAccount():void{
    this.service.removeUser(this.account.id).subscribe(
      data =>{
        alert("Account removed successfully!")
        this.router.navigateByUrl('/login').then(() =>{
          location.reload();
          this.router.navigate([decodeURI('/login')]);
        });
      },
      error => {
        alert("Error on removing your account...")
      }
    )
  }
}
