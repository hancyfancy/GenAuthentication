import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { User } from '../../models/user';
import { AuthenticationService } from '../../services/authentication.service';
import { Settings } from '../../settings';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  protected user: User = new User();

  constructor(private authenticationService: AuthenticationService, private settings: Settings, private router: Router, private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.user = new User();
  }

  get(username: string): void {
    this.authenticationService.getUser(new User(0n, username))
      .then(() => {
        this.goToValidate();
      })
      .catch((error) => {
        console.log("Promise rejected with " + JSON.stringify(error));
      });
  }

  goToValidate(): void {
    const navigationDetails: string[] = ['/validate'];
    this.router.navigate(navigationDetails);
  }

  goToRegister(): void {
    const navigationDetails: string[] = ['/register'];
    this.router.navigate(navigationDetails);
  }
}
