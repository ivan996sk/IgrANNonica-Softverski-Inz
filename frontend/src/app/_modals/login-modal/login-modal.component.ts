import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { CookieService } from 'ngx-cookie-service';
import { AuthService } from 'src/app/_services/auth.service';
import { UserInfoService } from 'src/app/_services/user-info.service';
import shared from '../../Shared';
import {AfterViewInit, ElementRef} from '@angular/core';
import { MatSelect } from '@angular/material/select';

@Component({
  selector: 'app-login-modal',
  templateUrl: './login-modal.component.html',
  styleUrls: ['./login-modal.component.css']
})
export class LoginModalComponent implements AfterViewInit {

  @ViewChild('closeButton') closeButton?: ElementRef;
  @ViewChild('usernameInput') usernameInput!: ElementRef;
  @ViewChild('pass') passwordInput!: ElementRef;

  username: string = '';
  password: string = '';

  passwordShown: boolean = false;

  wrongCreds: boolean = false;

  constructor(
    private authService: AuthService,
    private cookie: CookieService,
    private router: Router,
    private userInfoService: UserInfoService
  ) { }

  ngAfterViewInit(): void {
    this.usernameInput.nativeElement.focus();
  }

  doLoginWithEnterKey(keyboardEvent: KeyboardEvent) {
    if (keyboardEvent.code == "Enter" || keyboardEvent.code == "NumpadEnter") 
      this.doLogin();
  }

  doLogin() {
    if (this.username.length > 0 && this.password.length > 0) {
      this.authService.login(this.username, this.password).subscribe((response) => {

        if (response == "Username doesn't exist" || response == "Wrong password") {
          this.wrongCreds = true;
          this.password = '';
          this.passwordShown = false;
          this.passwordInput.nativeElement.type = "password";
        }
        else {
          this.wrongCreds = false;
          this.authService.authenticate(response);
          (<HTMLSelectElement>this.closeButton?.nativeElement).click();
          this.userInfoService.getUserInfo().subscribe((response) => {
            shared.photoId = response.photoId;
          });
          location.reload();
        }
        
      });
    }
    else {
      this.wrongCreds = true;
      this.password = '';
    }
  }
  resetData() {
    this.wrongCreds = false;
    this.username = '';
    this.password = '';
  }

  togglePasswordShown() {
    this.passwordShown = !this.passwordShown;

    if (this.passwordShown)
      this.passwordInput.nativeElement.type = "text";
    else 
      this.passwordInput.nativeElement.type = "password";
  }

  cleanWarnings() {
    this.wrongCreds = false;
  }
}
