import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Configuration } from './configuration.service';
import Dataset from '../_data/Dataset';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class DatasetsService {

  constructor(private http: HttpClient, private authService: AuthService) { }

  getPublicDatasets(): Observable<Dataset[]> {
    return this.http.get<Dataset[]>(`${Configuration.settings.apiURL}/dataset/publicdatasets`, { headers: this.authService.authHeader() });
  }

  getMyDatasets(): Observable<Dataset[]> {
    return this.http.get<Dataset[]>(`${Configuration.settings.apiURL}/dataset/mydatasets`, { headers: this.authService.authHeader() });
  }

  addDataset(dataset: Dataset): Observable<any> {
    return this.http.post(`${Configuration.settings.apiURL}/dataset/add`, dataset, { headers: this.authService.authHeader() });
  }

  stealDataset(dataset: Dataset): Observable<any> {
    return this.http.post(`${Configuration.settings.apiURL}/dataset/stealDs`, dataset, { headers: this.authService.authHeader() });
  }

  getDatasetFile(fileId: any): any {
    return this.http.get(`${Configuration.settings.apiURL}/file/csvRead/${fileId}/-1/11`, { headers: this.authService.authHeader(), responseType: 'text' });
  }
  getDatasetFilePaging(fileId:any,begin:any,end:any){
    return this.http.get(`${Configuration.settings.apiURL}/file/csvRead/${fileId}/${begin}/${end}`, { headers: this.authService.authHeader(), responseType: 'text' });
  }
  getDatasetHeader(fileId:any){
    return this.http.get(`${Configuration.settings.apiURL}/file/csvRead/${fileId}/-1/1`, { headers: this.authService.authHeader(), responseType: 'text' });
  }
  getDatasetFilePartial(fileId: any, startRow: number, rowNum: number): Observable<any> {
    return this.http.get(`${Configuration.settings.apiURL}/file/csvRead/${fileId}/${startRow}/${rowNum}`, { headers: this.authService.authHeader(), responseType: 'text' });
  }
  getDatasetById(datasetId: string): Observable<Dataset> {
    return this.http.get<Dataset>(`${Configuration.settings.apiURL}/dataset/get/${datasetId}`, { headers: this.authService.authHeader() });
  }

  editDataset(dataset: Dataset){
    return this.http.put(`${Configuration.settings.apiURL}/dataset/` + dataset._id, dataset, { headers: this.authService.authHeader() ,responseType:'text'});
  }

  deleteDataset(dataset: Dataset) {
    return this.http.delete(`${Configuration.settings.apiURL}/dataset/` + dataset._id, { headers: this.authService.authHeader(), responseType: "text" });
  }

  downloadFile(id:string):Observable<Blob>{
    return this.http.get(`${Configuration.settings.apiURL}/file/Download?id=`+id, { headers: this.authService.authHeader(), responseType: 'blob' });
  }
}
