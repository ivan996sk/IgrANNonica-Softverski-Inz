import { Component, OnInit } from '@angular/core';
import Dataset from 'src/app/_data/Dataset';
import { TabType } from 'src/app/_elements/folder/folder.component';
import { DatasetsService } from 'src/app/_services/datasets.service';

@Component({
  selector: 'app-archive',
  templateUrl: './archive.component.html',
  styleUrls: ['./archive.component.css']
})
export class ArchiveComponent implements OnInit {

  publicDatasets: Dataset[] = [];
  TabType = TabType;

  constructor(private datasetsService: DatasetsService) { }

  ngOnInit(): void {
    this.datasetsService.getPublicDatasets().subscribe((datasets) => {
      this.publicDatasets = datasets;
      this.publicDatasets.forEach((element, index) => {
        this.publicDatasets[index] = (<Dataset>element);
      })
    });
  }

}
