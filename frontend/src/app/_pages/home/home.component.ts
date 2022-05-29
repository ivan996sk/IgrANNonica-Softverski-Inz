import { Component, OnInit } from '@angular/core';
import Dataset from 'src/app/_data/Dataset';
import Predictor from 'src/app/_data/Predictor';
import shared from 'src/app/Shared';
import { DatasetsService } from 'src/app/_services/datasets.service';
import { PredictorsService } from 'src/app/_services/predictors.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  publicDatasets: Dataset[] = [];

  shared = shared;

  constructor(private datasetsService: DatasetsService, private predictorsService: PredictorsService) {
    this.datasetsService.getPublicDatasets().subscribe((datasets) => {
      this.publicDatasets = datasets;
      this.publicDatasets.forEach((element, index) => {
        this.publicDatasets[index] = (<Dataset>element);
      })
    });
  }

  ngOnInit(): void {
  }

}
