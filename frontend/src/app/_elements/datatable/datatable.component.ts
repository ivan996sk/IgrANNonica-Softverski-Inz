import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-datatable',
  templateUrl: './datatable.component.html',
  styleUrls: ['./datatable.component.css']
})
export class DatatableComponent implements OnInit {

  @Input() tableData!: TableData;

  constructor() { }

  ngOnInit(): void {
  }

}

export class TableData {
  constructor(
    public hasInput = false,
    public loaded = false,
    public numRows = 0,
    public numCols = 0,
    public data?: any[][]
  ) { }
}
