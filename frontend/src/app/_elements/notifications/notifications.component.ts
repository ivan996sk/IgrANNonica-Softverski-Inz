import { Component, OnInit } from '@angular/core';
import { SignalRService } from 'src/app/_services/signal-r.service';
import Notification from 'src/app/_data/Notification';

@Component({
  selector: 'app-notifications',
  templateUrl: './notifications.component.html',
  styleUrls: ['./notifications.component.css']
})
export class NotificationsComponent implements OnInit {

  notifications: Notification[] = [];
  closed: boolean = false;

  constructor(private signalRService: SignalRService) {
  }

  ngOnInit(): void {
    if (this.signalRService.hubConnection) {
      this.signalRService.hubConnection.on("NotifyDataset", (dName: string, dId: string) => {
        this.notifications.push(new Notification(`Obrađen izvor podataka: ${dName}`, dId, 1.0, false));
      });

      this.signalRService.hubConnection.on("NotifyEpoch", (mName: string, mId: string, stat: string, totalEpochs: number, currentEpoch: number) => {
        const existingNotification = this.notifications.find(x => x.id === mId)
        const progress = ((currentEpoch + 1) / totalEpochs);
        if (!existingNotification)
          this.notifications.push(new Notification(`Treniranje modela: ${mName}`, mId, progress, true));
        else {
          existingNotification.progress = progress;
        }
      });

      this.signalRService.hubConnection.on("NotifyModel", (mName: string, mId: string, stat: string, totalEpochs: number, currentEpoch: number) => {
        const existingNotification = this.notifications.find(x => x.id === mId)
        const progress = ((currentEpoch + 1) / totalEpochs);
        if (!existingNotification)
          this.notifications.push(new Notification(`Treniranje modela: ${mName}`, mId, progress, true));
        else {
          existingNotification.progress = progress;
        }
      });
    } else {
      console.warn("Notifications: No connection!");
    }
  }

  removeNotification(notification: Notification) {
    this.notifications.splice(this.notifications.indexOf(notification), 1);
  }

}
