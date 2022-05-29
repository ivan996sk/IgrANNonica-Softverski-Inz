import { Component, AfterViewInit, ElementRef, ViewChild, Input } from '@angular/core';
import { Chart } from 'chart.js';
import Experiment from 'src/app/_data/Experiment';
import Model, { ProblemType } from 'src/app/_data/Model';

@Component({
  selector: 'app-line-chart',
  templateUrl: './line-chart.component.html',
  styleUrls: ['./line-chart.component.css']
})

export class LineChartComponent implements AfterViewInit {

  dataAcc: number[] = [];
  dataMAE: number[] = [];
  dataMSE: number[] = [];
  dataLOSS: number[] = [];
  dataValAcc:number[]=[];
  dataValMAE:number[]=[];
  dataValMSE:number[]=[];
  dataValLoss:number[]=[];
  dataEpoch: number[] = [];

  @ViewChild('wrapper')
  wrapper!: ElementRef;
  @ViewChild('canvas')
  canvas!: ElementRef;
  @Input() experiment!:Experiment;
  constructor() {
    
  }
  width = 700;
  height = 400;
  history: any[] = [];
  myChartAcc!: Chart;
  myChartMae!: Chart;
  myChartMse!: Chart;
  myChartLoss!: Chart;
  ProblemType=ProblemType;
  resize() {
    this.width = this.wrapper.nativeElement.offsetWidth;
    this.height = this.wrapper.nativeElement.offsetHeight;

    if (this.canvas) {
      this.canvas.nativeElement.width = this.width;
      this.canvas.nativeElement.height = this.height;
    }
  }
  update(myEpochs: number[], myAcc: number[], myLoss: number[], myMae: number[], myMse: number[], myValAcc:number[],myValLoss:number[],myValMae:number[],myValMse:number[]) {
   
    this.dataEpoch.length = 0;
    this.dataEpoch.push(...myEpochs);

    this.dataAcc.length = 0;
    this.dataAcc.push(...myAcc);

    this.dataLOSS.length = 0;
    this.dataLOSS.push(...myLoss);

    this.dataMAE.length = 0;
    this.dataMAE.push(...myMae);

    this.dataMSE.length = 0;
    this.dataMSE.push(...myMse);

    this.dataValAcc.length = 0;
    this.dataValAcc.push(...myValAcc);

    this.dataValLoss.length = 0;
    this.dataValLoss.push(...myValLoss);

    this.dataValMAE.length = 0;
    this.dataValMAE.push(...myValMae);

    this.dataValMSE.length = 0;
    this.dataValMSE.push(...myValMse);

    this.myChartAcc.update();
    this.myChartLoss.update();
    this.myChartMae.update();
    this.myChartMse.update();
  }
  updateAll(history: any[],totalEpochs:number) {
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

    this.update(myEpochs, myAcc, myLoss, myMae, myMse, myValAcc,myValLoss,myValMAE,myValMSE);
  }
  ngAfterViewInit(): void {
    
    window.addEventListener('resize', () => { this.resize() });
    this.resize();
    this.myChartAcc = new Chart("myChartacc",
      {
        type: 'line',
        data: {
          labels: this.dataEpoch,
          datasets: [{

            label: 'Accuracy',
            data: this.dataAcc,
            borderWidth: 1,
            
          },
          {
            label: 'Val_Accuracy',
            data: this.dataValAcc,
            borderWidth: 1
          }          
          ]
        },
        options: {
          responsive: true,
          maintainAspectRatio: true,
          plugins: {
            legend: {
                labels: {
                    // This more specific font property overrides the global property
                    color:'white',
                    font: {
                      size: 10
                  }
                }
            }
          },
          scales: {
            x: {
              ticks: {
                color: 'white'
              },
              grid: {
                color: "rgba(0, 99, 171, 0.5)"
              },
              title: {
                display: true,
                text: 'Epoha',
                color:"white"
              }
            },
            y: {
              beginAtZero: true,
              ticks: {
                color: 'white'
              
              },
              grid: {
                color: "rgba(0, 99, 171, 0.5)"
              },
              title: {
                display: true,
                text: 'Vrednost',
                color:"white"
              }
            }

          },
          animation: {
            duration: 0
          }

        }
      },
      
    );
    if(this.experiment.type==ProblemType.BinaryClassification || this.experiment.type==ProblemType.MultiClassification){}
    this.myChartLoss = new Chart("myChartloss",
      {
        type: 'line',
        data: {
          labels: this.dataEpoch,
          datasets: [
          {
            label: 'Loss',
            data: this.dataLOSS,
            borderWidth: 1
          },
          {
            label: 'Val_Loss',
            data: this.dataValLoss,
            borderWidth: 1
          },
          ]
        },
        options: {
          responsive: true,
          maintainAspectRatio: true,

          plugins: {
            legend: {
                labels: {
                    // This more specific font property overrides the global property
                    color:'white',
                    font: {
                      size: 10
                  }
                }
            }
          },
          scales: {
            x: {
              ticks: {
                color: 'white'
              },
              grid: {
                color: "rgba(0, 99, 171, 0.5)"
              },
              title: {
                display: true,
                text: 'Epoha',
                color:"white"
              }
            },
            y: {
              beginAtZero: true,
              ticks: {
                color: 'white'
              
              },
              grid: {
                color: "rgba(0, 99, 171, 0.5)"
              },
              title: {
                display: true,
                text: 'Vrednost',
                color:"white"
              }
            }

          },
          animation: {
            duration: 0
          }

        }
      },
      
    );
    this.myChartMse = new Chart("myChartmse",
      {
        type: 'line',
        data: {
          labels: this.dataEpoch,
          datasets: [
          {
            label: 'MSE',
            data: this.dataMSE,
            borderWidth: 1
          },
          {
            label: 'Val_MSE',
            data: this.dataValMSE,
            borderWidth: 1
          }
          ]
        },
        options: {
          responsive: true,
          maintainAspectRatio: true,

          plugins: {
            legend: {
                labels: {
                    // This more specific font property overrides the global property
                    color:'white',
                    font: {
                      size: 10
                  }
                }
            }
          },
          scales: {
            x: {
              ticks: {
                color: 'white'
              },
              grid: {
                color: "rgba(0, 99, 171, 0.5)"
              },
              title: {
                display: true,
                text: 'Epoha',
                color:"white"
              }
            },
            y: {
              beginAtZero: true,
              ticks: {
                color: 'white'
              
              },
              grid: {
                color: "rgba(0, 99, 171, 0.5)"
              },
              title: {
                display: true,
                text: 'Vrednost',
                color:"white"
              }
            }

          },
          animation: {
            duration: 0
          }

        }
      },
      
    );
    this.myChartMae = new Chart("myChartmae",
      {
        type: 'line',
        data: {
          labels: this.dataEpoch,
          datasets: [
          {
            label: 'MAE',
            data: this.dataMAE,
            borderWidth: 1
          },
          {
            label: 'Val_MAE',
            data: this.dataValMAE,
            borderWidth: 1
          },
          ]
        },
        options: {
          responsive: true,
          maintainAspectRatio: true,

          plugins: {
            legend: {
                labels: {
                    // This more specific font property overrides the global property
                    color:'white',
                    font: {
                      size: 10
                  }
                }
            }
          },
          scales: {
            x: {
              ticks: {
                color: 'white'
              },
              grid: {
                color: "rgba(0, 99, 171, 0.5)"
              },
              title: {
                display: true,
                text: 'Epoha',
                color:"white"
              }
            },
            y: {
              beginAtZero: true,
              ticks: {
                color: 'white'
              
              },
              grid: {
                color: "rgba(0, 99, 171, 0.5)"
              },
              title: {
                display: true,
                text: 'Vrednost',
                color:"white"
              }
            }

          },
          animation: {
            duration: 0
          }

        }
      },
      
    );
    
  }
}

