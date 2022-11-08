import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { User } from '../models/user';
import { Settings } from '../settings';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {
  private urlPrefix: string = '';
  public userJwt: string = '';

  constructor(private http: HttpClient, private settings: Settings) {
    this.urlPrefix = this.settings.domain + '/api';
    this.userJwt = '';
  }

  /** POST **/
  addUser(user: User) {
    var httpOptions = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' })
    };
    return this.http.post<string>(this.urlPrefix + '/Register/New', JSON.stringify(user, (_, v) => typeof v === 'bigint' ? v.toString() : v), httpOptions).pipe(
      catchError(this.handleError<string>('addUser'))
    ).toPromise();
  }

  /** POST **/
  getUser(user: User) {
    var httpOptions = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' })
    };
    return this.http.post<string>(this.urlPrefix + '/Login/Individual', JSON.stringify(user, (_, v) => typeof v === 'bigint' ? v.toString() : v), httpOptions).pipe(
      catchError(this.handleError<string>('getUser'))
    ).toPromise();
  }

  /**
   * Handle Http operation that failed.
   * Let the app continue.
   *
   * @param operation - name of the operation that failed
   * @param result - optional value to return as the observable result
   */
  private handleError<T>(operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {

      console.error(error);

      this.log(`${operation} failed: ${error.message}`);

      confirm("Please verify either email or phone to login");

      return throwError(error);
    };
  }

  private log(message: string) {
    console.log(`AuthenticationService: ${message}`);
  }
}
