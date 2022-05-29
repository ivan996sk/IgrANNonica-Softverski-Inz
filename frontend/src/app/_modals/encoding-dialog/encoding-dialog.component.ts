import { Component, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ColumnType, Encoding } from 'src/app/_data/Experiment';
import Experiment from 'src/app/_data/Experiment';
import { ExperimentsService } from 'src/app/_services/experiments.service';
import { Inject} from '@angular/core';
import Dataset from 'src/app/_data/Dataset';

@Component({
  selector: 'app-encoding-dialog',
  templateUrl: './encoding-dialog.component.html',
  styleUrls: ['./encoding-dialog.component.css']
})
export class EncodingDialogComponent implements OnInit {

  selectedEncodingType?: Encoding;
  Encoding = Encoding;
  Object = Object;
  categoricalColumnExists: boolean = true;

  constructor(public dialogRef: MatDialogRef<EncodingDialogComponent>) 
  { 
    this.selectedEncodingType = Encoding.Label;
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
    this.dialogRef.close(this.selectedEncodingType);
  }
} 
