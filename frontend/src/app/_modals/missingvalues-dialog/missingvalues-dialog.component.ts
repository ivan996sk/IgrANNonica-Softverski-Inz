import { Component, OnInit } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { NullValueOptions } from 'src/app/_data/Experiment';

@Component({
  selector: 'app-missingvalues-dialog',
  templateUrl: './missingvalues-dialog.component.html',
  styleUrls: ['./missingvalues-dialog.component.css']
})
export class MissingvaluesDialogComponent implements OnInit {

  selectedMissingValuesOption?: NullValueOptions;

  NullValueOptions = NullValueOptions;

  constructor(public dialogRef: MatDialogRef<MissingvaluesDialogComponent>) 
  { 
    this.selectedMissingValuesOption = NullValueOptions.DeleteColumns;
  }

  ngOnInit(): void {
  }

  onNoClick() {
    this.dialogRef.close();
  }

  withEnterKey(keyboardEvent: KeyboardEvent) {
    if (keyboardEvent.code == "Enter" || keyboardEvent.code == "NumpadEnter") 
      this.onYesClick();
  }
  onYesClick() {
    this.dialogRef.close(this.selectedMissingValuesOption);
  }

}
