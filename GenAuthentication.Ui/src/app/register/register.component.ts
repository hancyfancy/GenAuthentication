import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { User } from '../../models/user';
import { AuthenticationService } from '../../services/authentication.service';
import { Settings } from '../../settings';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  protected user: User = new User();

  constructor(private authenticationService: AuthenticationService, private settings: Settings, private router: Router, private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.user = new User();
  }

  add(username: string, email: string, phone: string): void {
    this.authenticationService.addUser(new User(0n, username, email, phone))
      .then(() => {
        this.goToLogin();
      })
      .catch((error) => {
        console.log("Promise rejected with " + JSON.stringify(error));
      });
  }

  goToLogin(): void {
    const navigationDetails: string[] = ['/login'];
    this.router.navigate(navigationDetails);
  }
}
