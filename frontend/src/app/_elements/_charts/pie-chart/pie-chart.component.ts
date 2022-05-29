import { AfterViewInit, Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import {Chart} from 'chart.js';

@Component({
  selector: 'app-pie-chart',
  templateUrl: './pie-chart.component.html',
  styleUrls: ['./pie-chart.component.css']
})
export class PieChartComponent implements AfterViewInit {

  @Input()width?: number;
  @Input()height?: number;
  @Input()uniqueValues?: string[] = [];
  @Input()uniqueValuesPercent?: number[] = [];

  updatePieChart(uniqueValues: string[], uniqueValuesPercent: number[]){
    this.pieChartData.datasets =  [{
        label: "%",
        backgroundColor: ["#3e95cd", "#8e5ea2","#3cba9f","#e8c3b9","#c45850", "#000000"],
        data: uniqueValuesPercent,
      }];
      this.pieChartData.labels = uniqueValues
      this.myChart?.update() 
    };
  
  @ViewChild('piechart') chartRef!: ElementRef;
  constructor() { }

  pieChartData = {
    datasets: [{
      label: "Population (millions)",
      backgroundColor: ["#3e95cd", "#8e5ea2","#3cba9f","#e8c3b9","#c45850"],
      data: [2478,5267,734,784,433]
    }], labels : [""]
}

  myChart?: Chart;
  ngAfterViewInit(): void {
    let rem = 100;
    const percents : number[] = []
    this.uniqueValuesPercent?.forEach(percent => {
      rem-=percent*100;
      percents.push(percent*100)
      
    })
    const data = {
      datasets: [{
        label: "%",
        backgroundColor: ["#3e95cd", "#8e5ea2","#3cba9f","#e8c3b9","#c45850", "#000000"],
        data: [...percents, rem]
      }], labels : [...this.uniqueValues!,"Ostalo"]
    }
    
    const myChart = new Chart(this.chartRef.nativeElement, {
    type: 'pie',
    data: data     
  ,
    options: {
      plugins:{   
        legend: {
          display: false
                },
             },
             layout: {
              padding: 15
            }
    }
  });}

}