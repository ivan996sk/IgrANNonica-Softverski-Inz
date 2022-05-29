import { Injectable } from '@angular/core';
import * as signalR from "@microsoft/signalr";
import { CookieService } from 'ngx-cookie-service';
import { Configuration } from './configuration.service';
@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  public hubConnection?: signalR.HubConnection;
  public startConnection = () => {

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(Configuration.settings.apiWSUrl, {
        accessTokenFactory: () => this.cookie.get("token"),
        withCredentials: false
      }).build();

    this.hubConnection.on("Notify", (message: string) => {
      console.log(" " + message);
    });

    this.hubConnection
      .start()
      .then(() => console.log("con Started"))
      .catch(err => console.log("Error" + err))
  }
  constructor(private cookie: CookieService) { }
}
