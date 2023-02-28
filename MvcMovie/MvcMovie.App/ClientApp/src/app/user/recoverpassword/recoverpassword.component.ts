import { Component, OnInit } from '@angular/core';
import { SharedService } from 'src/app/shared.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-recoverpassword',
  templateUrl: './recoverpassword.component.html',
  styleUrls: ['./recoverpassword.component.css']
})
export class RecoverPasswordComponent implements OnInit {

  conta:any= {
    Email: "",
    Password: ""
  };


  constructor(private service: SharedService, private router : Router) { }

  ngOnInit(): void {
  }

  recoverPassword(): void {
    this.service.recoverPassword(this.conta).subscribe(
      data => {
        alert('Password changed!')
        this.router.navigateByUrl('/login').then(() =>{
          this.router.navigate([decodeURI('/login')]);
        });
      },
      error => {
        alert('Error on changing password...');
      }
    );
   }

}
