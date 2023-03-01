import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { faArrowLeft } from '@fortawesome/free-solid-svg-icons';
import { SharedService } from 'src/app/shared.service';

@Component({
  selector: 'app-edit',
  templateUrl: './edit.component.html',
  styleUrls: ['./edit.component.css']
})
export class EditUserComponent implements OnInit {

  faArrowLeft = faArrowLeft

  account : any = {}

  accountEditted = {
    Id: 0,
    Username: "",
    Email: "",
    Password: "password"
  }

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
        alert("Error on loading user data...");
      }
    );
  }

  editAccount():void{
    this.accountEditted.Id = this.account.id
    this.accountEditted.Username = this.account.username
    this.accountEditted.Email = this.account.email

    this.service.editUser(this.accountEditted).subscribe(
      data => {
        alert("User editted successfully!");
        this.router.navigateByUrl('/user').then(() =>{
          location.reload();
          this.router.navigate([decodeURI('/user')]);
        });
      },
      error => {
        alert("Error on editing user...");
      }
    );
  }
}
