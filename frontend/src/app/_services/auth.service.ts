import { EventEmitter, Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { JwtHelperService } from '@auth0/angular-jwt';
import { CookieService } from 'ngx-cookie-service';
import shared from '../Shared';
import { Configuration } from './configuration.service';

const jwtHelper = new JwtHelperService();

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  shared = shared;
  public loggedInEvent: EventEmitter<boolean> = new EventEmitter();

  constructor(private http: HttpClient, private cookie: CookieService) { }

  login(username: string, password: string) {
    return this.http.post(`${Configuration.settings.apiURL}/auth/login`, { username, password }, { responseType: 'text' });
  }

  register(user: any) {
    return this.http.post(`${Configuration.settings.apiURL}/auth/register`, { ...user },{ headers: this.authHeader() , responseType: 'text' });
  }

  getGuestToken() {
    return this.http.post(`${Configuration.settings.apiURL}/auth/guestToken`, {}, { responseType: 'text' });
  }

  isAuthenticated(): boolean {
    if (this.cookie.check('token')) {
      var token = this.cookie.get('token');
      var property = jwtHelper.decodeToken(this.cookie.get('token'));
      var username = property['name'];
      var userId = property['id'];
      return !jwtHelper.isTokenExpired(token) && username != "";
    }
    return false;
  }

  lastToken?: string;
  refresher: any;

  enableAutoRefresh() {
    this.lastToken = this.cookie.get('token');
    clearTimeout(this.refresher);
    let exp = jwtHelper.getTokenExpirationDate(this.lastToken);
    if (!exp) {
      exp = new Date();
    }
    var property = jwtHelper.decodeToken(this.cookie.get('token'));
    var username = property['name'];

      this.refresher = setTimeout(() => {
        this.http.post(`${Configuration.settings.apiURL}/auth/renewJwt`, {}, { headers: this.authHeader(), responseType: 'text' }).subscribe((response) => {
          this.authenticate(response);
        });
      }, exp.getTime() - new Date().getTime() - 60000);
  
  }

  addGuestToken() {
    this.getGuestToken().subscribe((token) => {
      this.authenticate(token);
      location.reload();
    });
  }

  authenticate(token: string) {
    let exp = jwtHelper.getTokenExpirationDate(token);
    if (!exp) {
      exp = new Date();
    }
    this.cookie.set('token', token, exp);
    this.updateUser();
  }

  updateUser() {
    if (this.cookie.check('token')) {
      const token = this.cookie.get('token');
      const decodedToken = jwtHelper.decodeToken(token);
      this.shared.loggedIn = this.isAuthenticated();
      this.shared.username = decodedToken.name;
      this.shared.userId = decodedToken.id;
      this.enableAutoRefresh();
    }
  }

  logOut() {
    this.cookie.delete('token');
    if (this.refresher)
      clearTimeout(this.refresher);
    this.shared.loggedIn = false;
    this.addGuestToken();
  }

  authHeader() {
    return new HttpHeaders().set("Authorization", "Bearer " + this.cookie.get('token'));
  }
  alreadyGuest(){
    if(this.cookie.check('token')){
      const token = this.cookie.get('token');
      const decodedToken = jwtHelper.decodeToken(token);
      if(decodedToken.role=="Guest")
        return true;
    }
    return false;
  }


}
