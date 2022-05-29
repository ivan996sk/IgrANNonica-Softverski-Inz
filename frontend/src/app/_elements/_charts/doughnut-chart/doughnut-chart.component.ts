import { AfterViewInit, Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import {Chart} from 'node_modules/chart.js';

@Component({
  selector: 'app-doughnut-chart',
  templateUrl: './doughnut-chart.component.html',
  styleUrls: ['./doughnut-chart.component.css']
})
export class DoughnutChartComponent implements AfterViewInit {

  @Input()width: number = 800;
  @Input()height: number = 450;
  
  @ViewChild('doughnut') chartRef!: ElementRef;
  constructor() { }

  ngAfterViewInit(): void {
    const myChart = new Chart(this.chartRef.nativeElement, {
      type: 'doughnut',
      data: {
        labels: ["Africa", "Asia", "Europe", "Latin America", "North America"],
        datasets: [{
          label: "Population (millions)",
          backgroundColor: ["#3e95cd", "#8e5ea2","#3cba9f","#e8c3b9","#c45850"],
          data: [2478,5267,734,784,433]
        }]
      },
      /*options: {
        title: {
          display: true,
          text: 'Predicted world population (millions) in 2050'
        }
      }*/
    });
  }

}
