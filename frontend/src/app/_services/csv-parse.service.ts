import { Injectable } from "@angular/core";
import * as FileSaver from 'file-saver';
import * as XLSX from 'xlsx';

const EXCEL_TYPE = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8';
const EXCEL_EXTENSION = '.xlsx';

@Injectable({ providedIn: 'root' })
export class CsvParseService {

  csvToArray(strData: string, strDelimiter: string): (string | null)[][] {
    strDelimiter = (strDelimiter || ",");

    let objPattern = new RegExp(
      (
        // Delimiters.
        "(\\" + strDelimiter + "|\\r?\\n|\\r|^)" +

        // Quoted fields.
        "(?:\"([^\"]*(?:\"\"[^\"]*)*)\"|" +

        // Standard fields.
        "([^\"\\" + strDelimiter + "\\r\\n]*))"
      ),
      "gi"
    );

    let arrData: (string | null)[][] = [[]];

    let arrMatches = null;

    while (arrMatches = objPattern.exec(strData)) {

      let strMatchedDelimiter = arrMatches[1];

      if (
        strMatchedDelimiter.length &&
        strMatchedDelimiter !== strDelimiter
      ) {
        arrData.push([]);
      }

      let strMatchedValue;

      if (arrMatches[2]) {
        strMatchedValue = arrMatches[2].replace(
          new RegExp("\"\"", "g"),
          "\""
        );
      } else {
        strMatchedValue = arrMatches[3];
      }

      if (strMatchedValue.length > 0)
        arrData[arrData.length - 1].push(strMatchedValue);
      else
        arrData[arrData.length - 1].push(null);
    }

    return (arrData);
  }

  ConvertJSONToCSV(objArray: string, headerList: { [x: string]: any; }) {
    let array = typeof objArray != 'object' ? JSON.parse(objArray) : objArray;
    let str = '';
    let row = 'S.No,';
    for (let index in headerList) {
      row += headerList[index] + ',';
    }
    row = row.slice(0, -1);
    str += row + '\r\n';
    for (let i = 0; i < array.length; i++) {
      let line = (i + 1) + '';
      for (let index in headerList) {
        let head = headerList[index];
        line += ',' + array[i][head];
      }
      str += line + '\r\n';
    }
    return str;
  }

  public exportAsExcelFile(json: any[], excelFileName: string): void {
    const worksheet: XLSX.WorkSheet = XLSX.utils.json_to_sheet(json);
    const workbook: XLSX.WorkBook = { Sheets: { 'data': worksheet }, SheetNames: ['data'] };
    const excelBuffer: any = XLSX.write(workbook, { bookType: 'xlsx', type: 'array' });
    this.saveAsExcelFile(excelBuffer, excelFileName);
  }

  private saveAsExcelFile(buffer: any, fileName: string): void {
    const data: Blob = new Blob([buffer], { type: EXCEL_TYPE });
    FileSaver.saveAs(data, fileName + '_export_' + new Date().getTime() + EXCEL_EXTENSION);
  }
}