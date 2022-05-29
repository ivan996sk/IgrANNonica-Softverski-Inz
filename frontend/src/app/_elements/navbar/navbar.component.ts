import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { Location } from '@angular/common';
import { AuthService } from '../../_services/auth.service';
import shared from 'src/app/Shared';
import { UserInfoService } from 'src/app/_services/user-info.service';
import { MatDialog } from '@angular/material/dialog';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css'],
  encapsulation: ViewEncapsulation.Emulated
})
export class NavbarComponent implements OnInit {

  currentUrl: string;
  shared = shared;

  constructor(public location: Location, private auth: AuthService, private userInfoService: UserInfoService, private matDialog: MatDialog) {
    shared.dialog = matDialog;
    this.currentUrl = this.location.path();
    this.location.onUrlChange(() => {
      this.currentUrl = this.location.path();
    })
  }

  ngOnInit(): void {
    this.auth.updateUser();
    if (this.auth.isAuthenticated() != false) {
      this.userInfoService.getUserInfo().subscribe((response) => {
        shared.photoId = response.photoId;
      });
    }
  }

  logOut() {
    this.auth.logOut();
  }
}
