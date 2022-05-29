import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { LineChartComponent } from '../_charts/line-chart/line-chart.component';
@Component({
  selector: 'app-metric-view',
  templateUrl: './metric-view.component.html',
  styleUrls: ['./metric-view.component.css']
})
export class MetricViewComponent implements OnInit {
  @ViewChild(LineChartComponent) linechartComponent!: LineChartComponent;


  constructor() { }

  ngOnInit(): void {
  }

  history: any[] = [];

  update(history: any[],totalEpochs:number) {
    const myAcc: number[] = [];
    const myMae: number[] = [];
    const myMse: number[] = [];
    const myLoss: number[] = [];
    const myValLoss: number[] = [];
    const myValAcc: number[] = [];
    const myValMAE: number[] = [];
    const myValMSE: number[] = [];

    const myEpochs: number[] = [];
    this.history = history;
    this.history.forEach((metrics, epoch) => {
      if(totalEpochs>100)
      {
        let epochEstimate=epoch*Math.round(Math.sqrt(totalEpochs))
        if(epochEstimate>totalEpochs)
          epochEstimate=totalEpochs;
        myEpochs.push(epochEstimate);
      }
      else
        myEpochs.push(epoch + 1);
      for (let key in metrics) {
        let value = metrics[key];
        if (key === 'accuracy') {
          myAcc.push(parseFloat(value));
        }
        else if (key === 'loss') {
          myLoss.push(parseFloat(value));
        }
        else if (key === 'mae') {
          myMae.push(parseFloat(value));
        }
        else if (key === 'mse') {
          myMse.push(parseFloat(value));
        }
        else if (key === 'val_acc') {
          myValAcc.push(parseFloat(value));
        }
        else if (key === 'val_loss') {
          myValLoss.push(parseFloat(value));
        }
        else if (key === 'val_mae') {
          myValMAE.push(parseFloat(value));
        }
        else if (key === 'val_mse') {
          myValMSE.push(parseFloat(value));
        }
      }
    });

    this.linechartComponent.update(myEpochs, myAcc, myLoss, myMae, myMse, myValAcc,myValLoss,myValMAE,myValMSE);
  }
}