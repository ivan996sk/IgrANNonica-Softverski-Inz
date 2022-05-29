import { Component, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import Experiment from 'src/app/_data/Experiment';
import { ExperimentsService } from 'src/app/_services/experiments.service';
import { Inject} from '@angular/core';

interface DialogData {
  experiment: Experiment;
}

@Component({
  selector: 'app-save-experiment-dialog',
  templateUrl: './save-experiment-dialog.component.html',
  styleUrls: ['./save-experiment-dialog.component.css']
})
export class SaveExperimentDialogComponent implements OnInit {

  selectedName: string = '';
  wrongAlreadyExists: boolean = false;
  wrongEmptyName: boolean = false;

  constructor(public dialogRef: MatDialogRef<SaveExperimentDialogComponent>, @Inject(MAT_DIALOG_DATA) public data: DialogData, private experimentService: ExperimentsService) { 
    this.wrongAlreadyExists = false;
    this.wrongEmptyName = false;
  }

  ngOnInit(): void {
  }

  onNoClick() {
    this.dialogRef.close();
  }

  saveWithEnterKey(keyboardEvent: KeyboardEvent) {
    if (keyboardEvent.code == "Enter" || keyboardEvent.code == "NumpadEnter") 
      this.onYesClick();
  }
  onYesClick() {
    if (this.selectedName == "") {
      this.wrongEmptyName = true;
      return;
    }
    this.wrongEmptyName = false;
    
    this.data.experiment.name = this.selectedName;
    this.experimentService.addExperiment(this.data.experiment).subscribe((response) => {
      this.wrongAlreadyExists = false;
      this.data.experiment = response;
      this.dialogRef.close(this.data.experiment);
    }, (error) => {
      if (error.error == "Experiment with this name exists") {
        this.wrongAlreadyExists = true;
      }
    });
  }
}
