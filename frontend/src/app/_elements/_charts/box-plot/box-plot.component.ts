import { AfterViewInit, Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { Chart, LinearScale, CategoryScale } from 'chart.js';
import { BoxPlotController, BoxAndWiskers } from '@sgratzl/chartjs-chart-boxplot';

function randomValues(count: number, min: number, max: number) {
  const delta = max - min;
  return Array.from({ length: count }).map(() => Math.random() * delta + min);
}

Chart.register(BoxPlotController, BoxAndWiskers, LinearScale, CategoryScale);

@Component({
  selector: 'app-box-plot',
  templateUrl: './box-plot.component.html',
  styleUrls: ['./box-plot.component.css']
})
export class BoxPlotComponent implements AfterViewInit {

  @Input() width!: number;
  @Input() height!: number;
  @Input() mean!: number;
  @Input() median!: number;
  @Input() min!: number;
  @Input() max!: number;
  @Input() q1!: number;
  @Input() q3!: number;

  

  /*
  updatePieChart(uniqueValues: string[], uniqueValuesPercent: number[]){
    
    this.pieChartData.datasets =  [{
        label: "%",
        backgroundColor: ["#3e95cd", "#8e5ea2","#3cba9f","#e8c3b9","#c45850", "#000000"],
        data: uniqueValuesPercent,
      }];
      this.pieChartData.labels = uniqueValues
      this.myChart?.update() 
    };
  */

  @ViewChild('boxplot') chartRef!: ElementRef;
  constructor() {
    //this.updateChart();
  }

  
  ngAfterViewInit(): void {
    const boxplotData = {
      // define label tree
      //labels: ['January'/*, 'February', 'March', 'April', 'May', 'June', 'July'*/],
      labels:[""],
      datasets: [{
        label: 'Dataset 1',
        backgroundColor: '#0063AB',
        borderColor: '#dfd7d7',
        borderWidth: 1,
        outlierColor: '#999999',
        scaleFontColor: '#0063AB',
        padding: 10,
        itemRadius: 0,
        data: [
          {min:this.min,q1:this.q1,q3:this.q3,median:this.median,max:this.max,mean:this.mean}
        ]
      }]
    };
    const myChart = new Chart(this.chartRef.nativeElement, {
      type: "boxplot",
      data: boxplotData,
      options: {
        plugins: {
          legend: {
            display: false
          },
        },
        scales: {
          x: {
            ticks: {
              color: '#dfd7d7'
            },
            grid: {
              color: "rgba(0, 99, 171, 0.5)"
            }
          },
          y: {
            ticks: {
              color: '#dfd7d7'
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
