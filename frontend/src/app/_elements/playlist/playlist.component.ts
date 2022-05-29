import { Component, Input, OnInit } from '@angular/core';
import Dataset from 'src/app/_data/Dataset';
import { TableData } from 'src/app/_elements/datatable/datatable.component';
import { CsvParseService } from 'src/app/_services/csv-parse.service';
import { DatasetsService } from 'src/app/_services/datasets.service';

@Component({
  selector: 'app-playlist',
  templateUrl: './playlist.component.html',
  styleUrls: ['./playlist.component.css']
})
export class PlaylistComponent implements OnInit {

  selectedId: string = "0";

  @Input() datasets!: Dataset[];

  tableDatas?: TableData[];

  constructor(private datasetService: DatasetsService, private csv: CsvParseService) {

  }

  getIndex(str: string) {
    return parseInt(str);
  }

  ngOnInit(): void {
    this.tableDatas = [];

    this.datasets.forEach((dataset, index) => {
      if (index < 3) {
        this.datasetService.getDatasetFile(dataset.fileId).subscribe((file: string | undefined) => {
          if (file) {
            const tableData = new TableData();
            tableData.hasInput = true;
            tableData.loaded = true;
            tableData.numRows = dataset.rowCount;
            tableData.numCols = dataset.columnInfo.length;
            tableData.data = this.csv.csvToArray(file, (dataset.delimiter == "razmak") ? " " : (dataset.delimiter.toString() == "") ? "," : dataset.delimiter);
            this.tableDatas!.push(tableData);
          }
        });
      }
    });

  }
}
