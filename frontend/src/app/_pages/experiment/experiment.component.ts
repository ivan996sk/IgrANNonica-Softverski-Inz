import { AfterViewInit, Component, ElementRef, ViewChild, ViewChildren, Input, OnInit } from '@angular/core';
import { StepperSelectionEvent } from '@angular/cdk/stepper';
import { MatStepper } from '@angular/material/stepper';
import Shared from 'src/app/Shared';
import { FolderFile, FolderType } from 'src/app/_data/FolderFile';
import { FolderComponent, TabType } from 'src/app/_elements/folder/folder.component';
import Experiment from 'src/app/_data/Experiment';
import { ExperimentsService } from 'src/app/_services/experiments.service';
import { ModelsService } from 'src/app/_services/models.service';
import Model from 'src/app/_data/Model';
import Dataset from 'src/app/_data/Dataset';
import { ColumnTableComponent } from 'src/app/_elements/column-table/column-table.component';
import { SignalRService } from 'src/app/_services/signal-r.service';
import { MetricViewComponent } from 'src/app/_elements/metric-view/metric-view.component';
import { ActivatedRoute, Router } from '@angular/router';
import { DatasetsService } from 'src/app/_services/datasets.service';
import { PredictorsService } from 'src/app/_services/predictors.service';
import { LineChartComponent } from 'src/app/_elements/_charts/line-chart/line-chart.component';

@Component({
  selector: 'app-experiment',
  templateUrl: './experiment.component.html',
  styleUrls: ['./experiment.component.css']
})
export class ExperimentComponent implements AfterViewInit {

  @ViewChild(MatStepper) stepper!: MatStepper;
  @ViewChild('stepsContainer') stepsContainer!: ElementRef;
  @ViewChildren('steps') steps!: ElementRef[];

  event: number = 0;
  experiment: Experiment;
  dataset?: Dataset;
  @ViewChild("folderDataset") folderDataset!: FolderComponent;
  @ViewChild(ColumnTableComponent) columnTable!: ColumnTableComponent;
  @ViewChild("folderModel") folderModel!: FolderComponent;
  @ViewChild(LineChartComponent) linechartComponent!: LineChartComponent;

  step1: boolean = false;
  step2: boolean = false;
  step3: boolean = false;
  step4: boolean = false;

  constructor(private experimentsService: ExperimentsService, private modelsService: ModelsService, private datasetsService: DatasetsService, private predictorsService: PredictorsService, private signalRService: SignalRService, private route: ActivatedRoute) {
    this.experiment = new Experiment("exp1");
  }

  /*updateExperiment(){

  }*/

  addNewExperiment() {
    this.experimentsService.addExperiment(this.experiment).subscribe(() => { console.log("new Experiment") });
  }

  trainModel() {
    if (!this.modelToTrain) {
      Shared.openDialog('Greška', 'Morate odabrati konfiguraciju neuronske mreže');
    } else {
      this.modelsService.trainModel(this.modelToTrain._id, this.experiment._id).subscribe(() => { console.log("pocelo treniranje") });
      this.step4 = true;
    }
  }

  stepHeight = this.calcStepHeight();

  calcStepHeight() {
    return window.innerHeight - 64;
  }

  ngAfterViewInit(): void {
    window.addEventListener('resize', () => {
      this.updatePageHeight();
    })
    this.updatePageHeight();

    setInterval(() => {
      this.updatePageIfScrolled();
    }, 100);

    this.stepsContainer.nativeElement.addEventListener('scroll', (event: Event) => {
      Shared.emitBGScrollEvent(this.stepsContainer.nativeElement.scrollTop);
    });

    if (this.signalRService.hubConnection) {
      this.signalRService.hubConnection.on("NotifyEpoch", (mName: string, mId: string, stat: string, totalEpochs: number, currentEpoch: number) => {
        if (currentEpoch == 0) {
          this.history = [];
        }
        if (this.modelToTrain?._id == mId) {
          stat = stat.replace(/'/g, '"');
          this.history.push(JSON.parse(stat));
          this.linechartComponent.updateAll(this.history,this.modelToTrain.epochs);
        }
      });

    }

    this.route.queryParams.subscribe(params => {

      let experimentId = this.route.snapshot.paramMap.get("id");
      let predictorId = this.route.snapshot.paramMap.get("predictorId");
      if (predictorId != null) {
        this.predictorsService.getPredictor(predictorId!).subscribe((response) => {
          let predictor = response;
          this.experimentsService.getExperimentById(predictor.experimentId).subscribe((response) => {
            this.experiment = response;
            this.datasetsService.getDatasetById(this.experiment.datasetId).subscribe((response: Dataset) => {
              this.dataset = response;
              this.folderDataset.forExperiment = this.experiment;
              this.folderDataset.selectFile(this.dataset); //sad 3. i 4. korak da se ucitaju

              this.modelsService.getModelById(predictor.modelId).subscribe((response) => {
                let model = response;
                this.folderModel.formModel.newModel = model;
                this.step3 = true;
                let numOfEpochsArray = Array.from({length: model.epochs}, (_, i) => i + 1);
                setTimeout(() => {
                  this.linechartComponent.update(numOfEpochsArray, predictor.metricsAcc, predictor.metricsLoss, predictor.metricsMae, predictor.metricsMse, predictor.metricsValAcc, predictor.metricsValLoss, predictor.metricsValMae, predictor.metricsValMse);
                })
              });
            });
          });
        });
      }
      else if (experimentId != null) {
        this.experimentsService.getExperimentById(experimentId).subscribe((response) => {
          this.experiment = response;
          this.datasetsService.getDatasetById(this.experiment.datasetId).subscribe((response: Dataset) => {
            this.dataset = response;
            this.folderDataset.forExperiment = this.experiment;
            this.folderDataset.selectFile(this.dataset);
          });
        });
      }

    });
  }

  history: any[] = [];

  updatePageIfScrolled() {
    if (this.scrolling) return;
    const currentPage = Math.round(this.stepsContainer.nativeElement.scrollTop / this.stepHeight)
    this.stepper.selectedIndex = currentPage;
  }

  updatePageHeight() {
    this.stepHeight = this.calcStepHeight();
    const stepHeight = `${this.stepHeight}px`;
    this.stepsContainer.nativeElement.style.minHeight = stepHeight;
    this.steps.forEach(step => {
      step.nativeElement.style.minHeight = stepHeight;
    })
  }

  changePage(event: StepperSelectionEvent) {
    this.updatePage(<number>event.selectedIndex)
  }

  goToPage(pageNum: number) {
    this.stepper.selectedIndex = pageNum;
    this.updatePage(pageNum);
  }

  scrollTimeout: any;

  updatePage(pageNum: number) {
    this.scrolling = true;
    this.event = pageNum;
    let scrollAmount = 0;
    this.steps.forEach((step, index) => {
      if (index == pageNum) {
        scrollAmount = step.nativeElement.offsetTop;
      }
    })
    clearTimeout(this.scrollTimeout);
    this.scrollTimeout = setTimeout(() => {
      this.scrolling = false;
    }, 800);
    this.stepsContainer.nativeElement.scroll({
      top: scrollAmount,
      behavior: 'smooth' //auto, smooth, initial, inherit
    });
  }

  scrolling: boolean = false;

  FolderType = FolderType;

  TabType = TabType;

  columnTableChangedEvent() {
    //sta se desi kad se nesto promeni u column-table komponenti...
  }

  experimentChangedEvent() {
    this.step2 = true;
    setTimeout(() => {
      this.folderModel.updateExperiment();
    });
  }

  setDataset(dataset: FolderFile | null) {
    if (dataset == null) {
      this.columnTable.loaded = false;
      this.dataset = undefined;
      this.experiment.datasetId = '';
      return;
    }
    const d = <Dataset>dataset;
    this.experiment.datasetId = d._id;
    this.dataset = d;

    this.step1 = true;
    setTimeout(() => {
      this.columnTable.loadDataset(d);
    });

    this.folderModel.selectFile(undefined);
    this.folderModel.selectTab(TabType.NewFile);
    // REFRESH GRAFIKA (4. KORAKA) URADITI 
  }

  modelToTrain?: Model;

  setModel(model: FolderFile) {
    const m = <Model>model;
    this.modelToTrain = m;
    this.step3 = true;
  }
}
