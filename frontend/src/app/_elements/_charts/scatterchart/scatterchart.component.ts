import { Component, OnInit } from '@angular/core';
import {Chart} from 'node_modules/chart.js';

@Component({
  selector: 'app-scatterchart',
  templateUrl: './scatterchart.component.html',
  styleUrls: ['./scatterchart.component.css']
})
export class ScatterchartComponent implements OnInit {

  constructor() { }

  ngOnInit(){
    const myChart = new Chart("ScatterCharts", {
      type: 'scatter',
      data: {
          datasets: [{
              label: 'Scatter Example:',
              data: [{x: 1, y: 11}, {x:2, y:12}, {x: 1, y: 2}, {x: 2, y: 4}, {x: 3, y: 8},{x: 4, y: 16}, {x: 1, y: 3}, {x: 3, y: 4}, {x: 4, y: 6}, {x: 6, y: 9},
                {x: 11, y: 9},
                {x: 12, y: 8},
                {x: 13, y: 6},
                {x: 14, y: 0},
                {x: 15, y: 5},
                {x: 16, y: 3},
                {x: 17, y: 2}],
                borderColor: 'white',
            }]
      },
      options: {
          scales: {
              x:{
                ticks: {
                  color: 'white'
                },
                grid: {
                  color: "rgba(0, 99, 171, 0.5)"
                }
              },
              y: {
                  beginAtZero: true,
                  ticks: {
                    color: 'white'
                  },
                  grid: {
                    color: "rgba(0, 99, 171, 0.5)"
                  }
              }
              
          }
          
          
      }
  });
  }
}
