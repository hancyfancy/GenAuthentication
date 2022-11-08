import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  public title = 'Gen Authentication';

  constructor(private router: Router) { }

  ngOnInit(): void {
    this.goToLogin();
  }

  goToLogin(): void {
    const navigationDetails: string[] = ['/login'];
    this.router.navigate(navigationDetails);
  }
}
