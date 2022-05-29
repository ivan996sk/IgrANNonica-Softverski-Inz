import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { AuthService } from 'src/app/_services/auth.service';
import User from 'src/app/_data/User';
import { DOCUMENT } from '@angular/common';
import { Inject }  from '@angular/core';
import shared from 'src/app/Shared';

@Component({
  selector: 'app-register-modal',
  templateUrl: './register-modal.component.html',
  styleUrls: ['./register-modal.component.css']
})
export class RegisterModalComponent implements OnInit {

  firstName: string = '';
  lastName: string = '';
  username: string = '';
  email: string = '';
  pass1: string = '';
  pass2: string = '';

  wrongFirstNameBool: boolean = false;
  wrongLastNameBool: boolean = false;
  wrongUsernameBool: boolean = false;
  wrongEmailBool: boolean = false;
  wrongPass1Bool: boolean = false;
  wrongPass2Bool: boolean = false;
  usernameAlreadyExistsBool: boolean = false;
  emailAlreadyExistsBool: boolean = false;

  pattName: RegExp = /^[a-zA-ZšŠđĐčČćĆžŽ]+([ \-][a-zA-ZšŠđĐčČćĆžŽ]+)*$/;
  pattUsername: RegExp = /^[a-zA-Z0-9]{4,18}$/;
  pattTwoSpaces: RegExp = /  /;
  pattEmail: RegExp = /^[a-zA-Z0-9]+([\.\-\+][a-zA-Z0-9]+)*\@([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}$/;
  pattPassword: RegExp = /.{6,30}$/;

  shared = shared;

  password1Shown: boolean = false;
  password2Shown: boolean = false;
  @ViewChild('password1') password1Input!: ElementRef;
  @ViewChild('password2') password2Input!: ElementRef;
  @ViewChild('closeButton') closeButton!: ElementRef;


  constructor(
    private authService: AuthService,
    @Inject(DOCUMENT) document: Document
  ) { }

  ngOnInit(): void {
  }

  doRegisterWithEnterKey(keyboardEvent: KeyboardEvent) {
    if (keyboardEvent.code == "Enter" || keyboardEvent.code == "NumpadEnter") 
      this.doRegister();
  }

  doRegister() {
    this.validation();
  }
  resetData() {
    this.firstName = this.lastName = this.username = this.email = this.pass1 = this.pass2 = '';
    this.wrongFirstNameBool = this.wrongLastNameBool = this.wrongUsernameBool = this.wrongEmailBool = this.wrongPass1Bool = this.wrongPass2Bool = this.usernameAlreadyExistsBool = this.emailAlreadyExistsBool = false;
    this.password1Shown = false;
    this.password2Shown = false;
  }

  isCorrectName(element: string): boolean {
    if (this.pattName.test(element) && !(this.pattTwoSpaces.test(element)) && (element.length >= 1 && element.length <= 30))
      return true;
    return false;
  }
  isCorrectUsername(element: string): boolean {
    if (this.pattUsername.test(element) && !(this.pattTwoSpaces.test(element)) && (element.length >= 1 && element.length <= 30))
      return true;
    return false;
  }
  isCorrectEmail(element: string): boolean {
    if (this.pattEmail.test(element.toLowerCase()) && element.length <= 320)
      return true;
    return false;
  }
  isCorrectPassword(element: string): boolean {
    if (this.pattPassword.test(element))
      return true;
    return false;
  }

  firstNameValidation() {
    if (this.isCorrectName(this.firstName) == true) {
      this.wrongFirstNameBool = false;
      return;
    }
    //(<HTMLSelectElement>document.getElementById('firstName')).focus();
    this.wrongFirstNameBool = true;
  }
  lastNameValidation() {
    if (this.isCorrectName(this.lastName) == true) {
      this.wrongLastNameBool = false;
      return;
    }
    //(<HTMLSelectElement>document.getElementById('lastName')).focus();
    this.wrongLastNameBool = true;
  }
  usernameValidation() {
    if (this.isCorrectUsername(this.username) == true) {
      this.wrongUsernameBool = false;
      return;
    }
    //(<HTMLSelectElement>document.getElementById('username-register')).focus();
    this.wrongUsernameBool = true;
  }
  emailValidation() {
    if (this.isCorrectEmail(this.email) == true) {
      this.wrongEmailBool = false;
      return;
    }
    //(<HTMLSelectElement>document.getElementById('email')).focus();
    this.wrongEmailBool = true;
  }
  passwordValidation() {
    if (this.isCorrectPassword(this.pass1) && this.isCorrectPassword(this.pass2) && this.pass1 == this.pass2) {
      this.wrongPass1Bool = false;
      this.wrongPass2Bool = false;
      return;
    }
    this.pass1 = ''; //brisi obe ukucane lozinke
    this.pass2 = '';
    this.password1Shown = false;
    this.password2Shown = false;
    this.password1Input.nativeElement.type = "password";
    this.password2Input.nativeElement.type = "password";
    //(<HTMLSelectElement>document.getElementById('pass1')).focus();
    this.wrongPass1Bool = true;
    this.wrongPass2Bool = true;
  }

  validation() {
    this.firstName = this.firstName.trim();
    this.lastName = this.lastName.trim();
    this.username = this.username.trim();
    this.email = this.email.trim();

    this.firstNameValidation();
    this.lastNameValidation();
    this.usernameValidation();
    this.emailValidation();
    this.passwordValidation();

    if (!(this.wrongFirstNameBool || this.wrongLastNameBool || this.wrongUsernameBool ||
      this.wrongEmailBool || this.wrongPass1Bool || this.wrongPass2Bool)) { //sve ok, registruj ga

      let user: User = {
        firstName: this.firstName,
        lastName: this.lastName,
        username: this.username,
        password: this.pass1,
        email: this.email,
        photoId: "1",
        isPermament:true,
        dateCreated:new Date()
      }


      this.authService.register(user)
        .subscribe(
          (response) => {
            if (response == 'User added') {
              this.usernameAlreadyExistsBool = false;
              this.emailAlreadyExistsBool = false;
              //nakon sto je registrovan, nek bude ulogovan
              this.authService.login(this.username, this.pass1).subscribe((response) => {

                this.authService.authenticate(response);
                this.closeButton.nativeElement.click();
                location.reload();
                //(<HTMLSelectElement>document.getElementById('linkToLoginModal')).click();
              }, (error) => console.warn(error));             
            }
            else if (response == 'Email Already Exists') {
              /*shared.openDialog("Greška!", "Nalog sa unetim email-om već postoji!");
              (<HTMLSelectElement>document.getElementById('email')).focus();*/
              this.usernameAlreadyExistsBool = false;
              this.emailAlreadyExistsBool = true;
            }
            else if (response == 'Username Already Exists') {
              /*shared.openDialog("Greška!", "Nalog sa unetim korisničkim imenom već postoji!");
              (<HTMLSelectElement>document.getElementById('username-register')).focus();*/
              this.emailAlreadyExistsBool = false;
              this.usernameAlreadyExistsBool = true;
            }
          }
        );
    }
  }

  togglePasswordShown(whichPassword: number) {
    if (whichPassword == 1) {
      this.password1Shown = !this.password1Shown;

      if (this.password1Shown)
        this.password1Input.nativeElement.type = "text";
      else 
        this.password1Input.nativeElement.type = "password";
    }
    else {
      this.password2Shown = !this.password2Shown;

      if (this.password2Shown)
        this.password2Input.nativeElement.type = "text";
      else 
        this.password2Input.nativeElement.type = "password";
    }
  }
  cleanWarnings() {
    this.resetData();
  }
}
