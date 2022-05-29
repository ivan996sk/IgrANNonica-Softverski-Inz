import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import Experiment from 'src/app/_data/Experiment';
import { ExperimentsService } from 'src/app/_services/experiments.service';

interface DialogData {
  experiment: Experiment;
  selectedOption: number;
}

@Component({
  selector: 'app-update-experiment-dialog',
  templateUrl: './update-experiment-dialog.component.html',
  styleUrls: ['./update-experiment-dialog.component.css']
})
export class UpdateExperimentDialogComponent implements OnInit {

  selectedOption: number = 1;
  selectedName: string = '';
  wrongAlreadyExists: boolean = false;
  wrongEmptyName: boolean = false;

  constructor(public dialogRef: MatDialogRef<UpdateExperimentDialogComponent>, @Inject(MAT_DIALOG_DATA) public data: DialogData, private experimentService: ExperimentsService) { 
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
    if (this.selectedOption == 1) { //update
      this.experimentService.updateExperiment(this.data.experiment).subscribe((response) => { 
        this.data.experiment = response;
        this.dialogRef.close(this.data.experiment);
      });
    }
    else { //save new
      if (this.selectedName == "") {
        this.wrongEmptyName = true;
        return;
      }
      this.wrongEmptyName = false;

      const newExperiment = new Experiment();
      Object.assign(newExperiment, this.data.experiment);
      newExperiment.name = this.selectedName;
      newExperiment._id = '';
      this.experimentService.addExperiment(newExperiment!).subscribe((response) => {
        this.wrongAlreadyExists = false;
        this.dialogRef.close(response);
      }, (error) => {
        if (error.error == "Experiment with this name exists") {
          this.wrongAlreadyExists = true;
        }
      });
    }
  }
}
