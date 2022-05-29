import { AfterViewInit, Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import Dataset from 'src/app/_data/Dataset';
import { FolderFile, FolderType } from 'src/app/_data/FolderFile';
import Model, { ProblemType } from 'src/app/_data/Model';
import { DatasetsService } from 'src/app/_services/datasets.service';
import Shared from 'src/app/Shared';
import { ModelsService } from 'src/app/_services/models.service';
import { FormDatasetComponent } from '../form-dataset/form-dataset.component';
import Experiment from 'src/app/_data/Experiment';
import { ExperimentsService } from 'src/app/_services/experiments.service';
import { PredictorsService } from 'src/app/_services/predictors.service';
import { SignalRService } from 'src/app/_services/signal-r.service';
import { FormModelComponent } from '../form-model/form-model.component';
import { ActivatedRoute, Router } from '@angular/router';
import Predictor from 'src/app/_data/Predictor';
import FileSaver from 'file-saver';
import isEqual from 'lodash.isequal';

@Component({
  selector: 'app-folder',
  templateUrl: './folder.component.html',
  styleUrls: ['./folder.component.css']
})
export class FolderComponent implements AfterViewInit {

  @ViewChild('selectedDataset') formDataset!: FormDatasetComponent;
  @ViewChild('selectedModel') formModel!: FormModelComponent;
  @ViewChild('newDataset') formNewDataset!: FormDatasetComponent;
  @ViewChild('newModel') formNewModel!: FormModelComponent;

  @Input() folderName: string = 'Moji podaci';
  @Input() files!: FolderFile[]

  newFile?: Dataset | Model;

  @Input() type: FolderType = FolderType.Dataset;
  @Input() forExperiment!: Experiment;
  @Input() startingTab!: TabType;
  @Input() archive: boolean = false;
  newFileSelected: boolean = true;

  selectedFileIndex: number = -1;
  selectedFile?: FolderFile;
  hoveringOverFileIndex: number = -1;
  saveDisabled: boolean = false;

  @Output() selectedFileChanged: EventEmitter<FolderFile> = new EventEmitter();
  @Output() fileFromRoute: EventEmitter<FolderFile> = new EventEmitter();
  @Output() okPressed: EventEmitter<string> = new EventEmitter();

  searchTerm: string = '';

  constructor(private datasetsService: DatasetsService, private experimentsService: ExperimentsService, private modelsService: ModelsService, private predictorsService: PredictorsService, private signalRService: SignalRService, private router: Router, private route: ActivatedRoute) {
    this.tabsToShow.forEach(tab => this.folders[tab] = []);
  }

  ngAfterViewInit(): void {
    this.refreshFiles(null);

    if (this.signalRService.hubConnection) {
      this.signalRService.hubConnection.on("NotifyDataset", (dName: string, dId: string) => {
        if (this.type == FolderType.Dataset) {
          this.refreshFiles(dId);
        }
      });
    } else {
      console.warn("Dataset-Load: No connection!");
    }
  }

  displayFile() {
    if (this.newFileSelected) {
      if (this.type == FolderType.Dataset) {
        this.formNewDataset.dataset = <Dataset>this.newFile;
        this.formNewDataset.existingFlag = false;
      }
      else if (this.type == FolderType.Model)
        this.formNewModel.newModel = <Model>this.newFile;
    } else {
      if (this.type == FolderType.Dataset) {
        this.formDataset.dataset = <Dataset>this.selectedFile;
        this.formDataset.existingFlag = false;
      }
      else if (this.type == FolderType.Model)
        this.formModel.newModel = <Model>this.selectedFile;
    }
  }

  selectNewFile() {
    if (!this.newFile) {
      this.createNewFile();
    }
    this.newFileSelected = true;
    this.listView = false;
    this.displayFile();

    this.selectedTab = TabType.NewFile;

    if (this.type == FolderType.Dataset) {
      this.formNewDataset.clear();
    }
  }

  selectFile(file?: FolderFile) {
    this.formDataset.resetPagging();
    this.selectedFile = file;
    this.updateLastFileData(file);
    if (this.type == FolderType.Experiment && file) {
      this.router.navigate(['/experiment/' + file._id]);
    }
    this.newFileSelected = false;
    this.listView = false;
    this.selectedFileChanged.emit(this.selectedFile);
    this.displayFile();

    this.selectedTab = TabType.File;

    if (this.type == FolderType.Dataset)
      this.formDataset.loadExisting();
  }

  updateLastFileData(file: FolderFile | undefined) {
    if (!file) return;

    Object.assign(this.lastFileData, file);
    if (this.type == FolderType.Model) {
      const lastModel = (<Model>this.lastFileData)
      lastModel.layers = [];
      (<Model>file).layers.forEach(layer => {
        const clone = Object.assign({}, layer);
        lastModel.layers.push(clone);
      })
    }
  }

  goToExperimentPageWithPredictor(file: FolderFile, predictor: Predictor) {
    this.router.navigate(['/experiment/p/' + predictor._id]);
  }

  createNewFile() {
    if (this.type == FolderType.Dataset) {
      this.newFile = new Dataset();
    } else if (this.type == FolderType.Model) {
      this.newFile = new Model();
    }
  }

  ok() {
    this.okPressed.emit();
  }

  _initialized: boolean = false;

  refreshFiles(selectedDatasetId: string | null = null, selectedModelId: string | null = null) {

    this.tabsToShow.forEach(tab => {
      this.folders[tab] = [];
    });

    if (this.archive) {
      this.refreshDatasets(selectedDatasetId);
      this.refreshModels(selectedModelId);
      this.refreshExperiments();
    } else {
      switch (this.type) {
        case FolderType.Dataset:
          this.refreshDatasets(selectedDatasetId);
          break;

        case FolderType.Model:
          this.refreshModels(selectedModelId);
          break;

        case FolderType.Experiment:
          this.refreshExperiments();
          break;

        default:
          console.error("Bad folder type.");
          break;
      }
    }

    if (!this._initialized) {
      this.files = this.folders[this.startingTab];
      this.filteredFiles = [];
      setTimeout(() => {
        this.selectTab(this.startingTab);
      });
      this._initialized = true;
    }
  }

  refreshModels(selectedModelId: string | null) {
    this.modelsService.getMyModels().subscribe((models) => {
      this.folders[TabType.MyModels] = models;
      if (selectedModelId) {
        this.selectFile(models.filter(x => x._id == selectedModelId)[0]);
        setTimeout(() => {
          this.okPressed.emit();
        });
      }
      this.searchTermsChanged();
    });
    /*this.modelsService.getMyModels().subscribe((models) => {
      this.folders[TabType.PublicModels] = models;
      this.searchTermsChanged();
    });*/

    this.modelsService.getPublicModels().subscribe((models) => {
      this.folders[TabType.PublicModels] = models;
      this.searchTermsChanged();
    });
    //this.folders[TabType.PublicModels] = [];
  }

  refreshDatasets(selectedDatasetId: string | null) {
    this.datasetsService.getMyDatasets().subscribe((datasets) => {
      this.folders[TabType.MyDatasets] = datasets;
      if (selectedDatasetId) {
        this.selectFile(datasets.filter(x => x._id == selectedDatasetId)[0]);
      }
      this.searchTermsChanged();
    });
    this.datasetsService.getPublicDatasets().subscribe((datasets) => {
      this.folders[TabType.PublicDatasets] = datasets;
      this.searchTermsChanged();
    });
  }

  refreshExperiments() {
    this.experimentsService.getMyExperiments().subscribe((experiments) => {
      this.folders[TabType.MyExperiments] = experiments;
      this.predictorsService.getMyPredictors().subscribe((predictors) => {
        this.predictorsForExp = {};
        experiments.forEach(exp => {
          this.predictorsForExp[exp._id] = predictors.filter(pred => pred.experimentId == exp._id);
          /* TODO IZMENI OVO DA SE SETUJE NA BACKU AUTOMATSKI */
          this.predictorsForExp[exp._id].forEach(pred => {
            const model = this.folders[TabType.MyModels].find(model => model._id == pred.modelId);
            pred.name = model?.name!;
            //pred.lastUpdated = model?.lastUpdated!;
          })
          /* ------------------------------------------------ */
          this.searchTermsChanged();
          if (this.selectedTab == TabType.MyExperiments)
            this.selectTab(TabType.MyExperiments);
        })
      });
    });
  }

  saveNewFile() {
    this.saveDisabled = true;
    this.loadingAction = true;
    switch (this.type) {
      case FolderType.Dataset:
        this.formNewDataset!.uploadDataset((dataset: Dataset) => {
          this.newFile = undefined;
          this.loadingAction = false;
          this.okPressed.emit();
          //Shared.openDialog("Obaveštenje", "Uspešno ste dodali novi izvor podataka u kolekciju. Molimo sačekajte par trenutaka da se obradi.");
          this.refreshFiles();
        },
          () => {
            Shared.openDialog("Neuspeo pokušaj!", "Izvor podataka sa unetim nazivom već postoji u Vašoj kolekciji. Izmenite naziv ili iskoristite postojeći dataset.");
          });
        break;
      case FolderType.Model:
        this.formNewModel.newModel.type = this.formModel.forProblemType;
        this.modelsService.addModel(this.formModel.newModel).subscribe(model => {
          this.newFile = undefined;
          this.loadingAction = false;
          //Shared.openDialog("Obaveštenje", "Uspešno ste dodali novu konfiguraciju neuronske mreže u kolekciju.");
          this.refreshFiles(null, model._id); // todo select model
        }, (err) => {
          Shared.openDialog("Neuspeo pokušaj!", "Konfiguracija neuronske mreže sa unetim nazivom već postoji u Vašoj kolekciji. Izmenite naziv ili iskoristite postojeću konfiguraciju.");
        });
        break;
    }
    this.saveDisabled = false;
  }

  predictorsForExp: { [expId: string]: Predictor[] } = {}

  clearSearchTerm() {
    this.searchTerm = '';
    this.searchTermsChanged();
  }

  filteredFiles: FolderFile[] = [];

  searchTermsChanged() {
    this.filteredFiles.length = 0;
    if (!this.files) return;
    this.filteredFiles.push(...this.files.filter((file) => {
      return (file.name.toLowerCase().includes(this.searchTerm.toLowerCase())
        && (!this.forExperiment
          || this.type != FolderType.Model
          || (this.type == FolderType.Model && (<Model>file).type == this.forExperiment.type)))
    }));
    /*if (this.selectedFile) {
      if (!this.filteredFiles.includes(this.selectedFile)) {
        if (this.hoverTab === TabType.None && this.getFolderType(this.selectedTab) === this.type) {
          this.selectFile(undefined);
        }
      } else {
        //this.selectedFileIndex = this.filteredFiles.indexOf(this.selectedFile);
      }
    }*/
  }

  listView: boolean = true;

  loadingAction = false;
  selectedFileHasChanges = false;
  lastFileData = {};

  onFileChange() {
    setTimeout(() => {
      this.selectedFileHasChanges = !((this.selectedTab == TabType.NewFile) || this.checkFileDataEqualToLastFileData());
    });
  }

  checkFileDataEqualToLastFileData() {
    if (this.type == FolderType.Model) {
      let layersEqual = true;
      const oldModel = (<Model>this.lastFileData);
      const selectedModel = (<Model>this.selectedFile)
      const oldLayers = oldModel.layers;
      oldModel.layers = selectedModel.layers;
      const objEqual = isEqual(this.selectedFile, oldModel);
      oldLayers.forEach((layer, index) => {
        if (!isEqual(layer, selectedModel.layers[index])) {
          layersEqual = false;
        }
      });
      return objEqual && layersEqual;
    } else {
      return isEqual(this.selectedFile, this.lastFileData);
    }
  }

  updateFile() {
    const file = this.selectedFile;
    this.loadingAction = true;
    switch (this.type) {
      case FolderType.Dataset:
        this.datasetsService.editDataset(<Dataset>file).subscribe((response) => {
          this.fileUpdatedSuccess();
        });
        break;
      case FolderType.Model:
        this.modelsService.editModel(<Model>file).subscribe((response) => {
          this.fileUpdatedSuccess();
        });
        break;
    }
  }

  fileUpdatedSuccess() {
    this.loadingAction = false;
    this.selectedFileHasChanges = false;
    this.updateLastFileData(this.selectedFile);
    this.refreshFiles();
    this.selectedFileChanged.emit(this.selectedFile);
  }

  deleteFile(file: FolderFile, event: Event, deletePredictor: boolean = false) {
    event.stopPropagation();

    switch (this.type) {
      case FolderType.Dataset:
        const dataset = <Dataset>file;
        Shared.openYesNoDialog("Obriši izvor podataka", "Eksperimenti i trenirani modeli nad ovim izvorom podataka će takođe biti obrisani, da li ste sigurni da želite da obrišete izvor: " + dataset.name + "?", () => {
          if (this.selectedTab == TabType.MyDatasets) {
            this.filteredFiles.splice(this.filteredFiles.indexOf(file), 1);
            this.files.splice(this.files.indexOf(file), 1);
          }
          this.loadingAction = true;
          this.datasetsService.deleteDataset(dataset).subscribe((response) => {
            this.loadingAction = false;
            if (this.selectedTab == TabType.File) {
              this.refreshDatasets(null);
              this.selectedFile = undefined!;
              setTimeout(() => {
                this.selectTab(TabType.MyDatasets);
              });
            }
            if (this.archive) {
              this.refreshExperiments();
            }

          });
        })
        break;
      case FolderType.Model:
        const model = <Model>file;
        Shared.openYesNoDialog("Obriši konfiguraciju neuronske mreže", "Trenirani modeli za ovu konfiguraciju će takođe biti obrisani, da li ste sigurni da želite da obrišete konfiguraciju: " + model.name + "?", () => {
          if (this.selectedTab == TabType.MyModels) {
            this.filteredFiles.splice(this.filteredFiles.indexOf(file), 1);
            this.files.splice(this.files.indexOf(file), 1);
          }
          this.loadingAction = true;
          this.modelsService.deleteModel(<Model>file).subscribe((response) => {
            this.loadingAction = false;
            if (this.selectedTab == TabType.File) {
              this.refreshModels(null);
              this.selectedFile = undefined!;
              setTimeout(() => {
                this.selectTab(TabType.MyModels);
              });
            }
            if (this.archive) {
              this.refreshExperiments();
            }
          });
        })

        break;
      case FolderType.Experiment:
        if (deletePredictor) {
          const predictor = <Predictor>file;
          Shared.openYesNoDialog("Obriši trenirani model", "Da li ste sigurni da želite da obrišete trenirani model: " + predictor.name + "?", () => {
            this.filteredFiles.splice(this.filteredFiles.indexOf(file), 1);
            this.files.splice(this.files.indexOf(file), 1);
            this.loadingAction = true;
            this.predictorsService.deletePredictor(predictor).subscribe((response) => {
              this.loadingAction = false;
            });
          });
        } else {
          const experiment = <Experiment>file;
          Shared.openYesNoDialog("Obriši eksperiment", "Trenirani modeli za ovaj eksperiment će takođe biti obrisani, da li ste sigurni da želite da obrišete eksperiment: " + experiment.name + "?", () => {
            if (this.selectedTab == TabType.MyExperiments) {
              this.filteredFiles.splice(this.filteredFiles.indexOf(file), 1);
              this.files.splice(this.files.indexOf(file), 1);
            }
            this.loadingAction = true;
            this.experimentsService.deleteExperiment(experiment).subscribe((response) => {
              this.loadingAction = false;
              if (this.selectedTab == TabType.File) {
                this.refreshExperiments();
                this.selectedFile = undefined!;
                setTimeout(() => {
                  this.selectTab(TabType.MyExperiments);
                });
              }
            });
          });
        }
        break;
    }
  }
  downloadFile(file: FolderFile, event: Event) {
    event.stopPropagation();
    if (this.type == FolderType.Dataset) {
      const fileId = (<Dataset>file).fileId;
      const name = (<Dataset>file).name;
      const ext = (<Dataset>file).extension;
      if (fileId != undefined)
        this.datasetsService.downloadFile(fileId).subscribe((response) => {
          FileSaver.saveAs(response, name + ext);

        });

    }
  }

  addFile(file: FolderFile, event: Event) {
    event.stopPropagation();
    switch (this.type) {
      case FolderType.Dataset:
        (<Dataset>file)._id = "";
        (<Dataset>file).isPreProcess = true;
        (<Dataset>file).isPublic = false;
        this.datasetsService.stealDataset(<Dataset>file).subscribe((response) => {
          Shared.openDialog("Obaveštenje", "Uspešno ste dodali javni izvor podataka u vašu kolekciju.");
          this.refreshFiles(null);
        }, (error: any) => {
          if (error.error == "Dataset with this name already exists") {
            Shared.openDialog("Obaveštenje", "Izvor podataka sa ovim imenom postoji u vašoj kolekciji.");
          }
        });
        break;
      case FolderType.Model:
        this.modelsService.stealModel(<Model>file).subscribe((response) => {
          Shared.openDialog("Obaveštenje", "Uspešno ste dodali javnu konfiguraciju neuronske mreže u vašu kolekciju.");
          this.refreshFiles(null);
        }, (error: any) => {
          if (error.error == "Model already exisits or validation size is not between 0-1") {
            Shared.openDialog("Obaveštenje", "Model sa ovim imenom postoji u vašoj kolekciji.");
          }
        });
        break;
      case FolderType.Experiment:
        // this.experimentsService.addExperiment(<Model>file).subscribe((response) => {
        // });
        //todo delete za predictor
        break;
    }
  }

  folders: { [tab: number]: FolderFile[] } = {};

  tabTitles: { [tab: number]: string } = {
    [TabType.File]: 'Fajl',
    [TabType.NewFile]: 'Novi fajl',
    [TabType.MyDatasets]: 'Moji izvori podataka',
    [TabType.PublicDatasets]: 'Javni izvori podataka',
    [TabType.MyModels]: 'Moje konfiguracije neuronske mreže',
    [TabType.PublicModels]: 'Javne konfiguracije neuronske mreže',
    [TabType.MyExperiments]: 'Eksperimenti',
  };

  FolderType = FolderType;
  ProblemType = ProblemType;
  Privacy = Privacy;
  TabType = TabType;

  privacy: Privacy = Privacy.Private;

  @Input() tabsToShow: TabType[] = [
    TabType.MyDatasets,
    TabType.PublicDatasets,
    TabType.MyModels,
    TabType.PublicModels,
    TabType.MyExperiments
  ]

  @Input() selectedTab: TabType = TabType.NewFile;
  hoverTab: TabType = TabType.None;

  selectTab(tab: TabType) {
    if (tab == TabType.NewFile) {
      this.selectNewFile();
    } else if (tab == TabType.File) {
      this.selectFile(this.selectedFile);
    }
    this.listView = this.getListView(tab);
    this.type = this.getFolderType(tab);
    this.privacy = this.getPrivacy(tab);
    this.selectedTab = tab;
    this.files = this.folders[tab];

    if (tab !== TabType.File && tab !== TabType.NewFile)
      this.searchTermsChanged();
  }

  getListView(tab: TabType) {
    switch (tab) {
      case TabType.File:
      case TabType.NewFile:
      case TabType.None:
        return false;
      case TabType.MyExperiments:
      case TabType.MyDatasets:
      case TabType.MyModels:
      case TabType.PublicDatasets:
      case TabType.PublicModels:
        return true;
      default:
        return false;
    }
  }

  getFolderType(tab: TabType) {
    switch (tab) {
      case TabType.MyExperiments:
        return FolderType.Experiment;
      case TabType.MyDatasets:
      case TabType.PublicDatasets:
        return FolderType.Dataset;
      case TabType.MyModels:
      case TabType.PublicModels:
        return FolderType.Model;
      default:
        return this.type;
    }
  }

  getPrivacy(tab: TabType) {
    switch (tab) {
      case TabType.PublicDatasets:
      case TabType.PublicModels:
        return Privacy.Public;
      default:
        return Privacy.Private;
    }
  }

  hoverOverTab(tab: TabType) {
    // this.listView = this.getListView(tab);
    // this.privacy = this.getPrivacy(tab);
    this.hoverTab = tab;
    // if (tab == TabType.None) {
    //   this.listView = this.getListView(this.selectedTab);
    //   this.files = this.folders[this.selectedTab];
    // } else {
    //   this.files = this.folders[tab];
    // }
    // this.searchTermsChanged();
  }

  updateExperiment() {
    if (this.formModel) {
      setTimeout(() => {
        this.formModel.updateGraph();
      });
    }
  }

  newTabTitles: { [tab: number]: string } = {
    [FolderType.Dataset]: 'Novi izvor podataka',
    [FolderType.Model]: 'Nova konfiguracija neuronske mreže',
  };
}

export enum Privacy {
  Private,
  Public
}

export enum TabType {
  NewFile,
  File,
  MyDatasets,
  PublicDatasets,
  MyModels,
  PublicModels,
  MyExperiments,
  None
}