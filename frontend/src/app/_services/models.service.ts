import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import Model from '../_data/Model';
import { AuthService } from './auth.service';
import { Observable } from 'rxjs';
import Dataset from '../_data/Dataset';
import { Configuration } from '../_services/configuration.service';

@Injectable({
  providedIn: 'root'
})
export class ModelsService {

  constructor(private http: HttpClient, private authService: AuthService) { }

  uploadData(file: File): Observable<any> {
    let formData = new FormData();
    formData.append('file', file, file.name);

    let params = new HttpParams();

    const options = {
      params: params,
      reportProgress: false,
      headers: this.authService.authHeader()
    };

    return this.http.post(`${Configuration.settings.apiURL}/file/csv`, formData, options);
  }

  addModel(model: Model): Observable<any> {
    return this.http.post(`${Configuration.settings.apiURL}/model/add`, model, { headers: this.authService.authHeader() });
  }

  stealModel(model: Model): Observable<any> {
    return this.http.post(`${Configuration.settings.apiURL}/model/stealModel`, model, { headers: this.authService.authHeader() });
  }
  addDataset(dataset: Dataset): Observable<any> {
    return this.http.post(`${Configuration.settings.apiURL}/dataset/add`, dataset, { headers: this.authService.authHeader() });
  }
  trainModel(modelId: string, experimentId: string): Observable<any> {
    return this.http.post(`${Configuration.settings.apiURL}/model/trainmodel`, { ModelId: modelId, ExperimentId: experimentId }, { headers: this.authService.authHeader(), responseType: 'text' });
  }

  getMyDatasets(): Observable<Dataset[]> {
    return this.http.get<Dataset[]>(`${Configuration.settings.apiURL}/dataset/mydatasets`, { headers: this.authService.authHeader() });
  }

  getMyModels(): Observable<Model[]> {
    return this.http.get<Model[]>(`${Configuration.settings.apiURL}/model/mymodels`, { headers: this.authService.authHeader() });
  }

  editModel(model: Model): Observable<Model> {
    return this.http.put<Model>(`${Configuration.settings.apiURL}/model/` + model.name, model, { headers: this.authService.authHeader() });
  }

  deleteModel(model: Model) {
    return this.http.delete(`${Configuration.settings.apiURL}/model/` + model.name, { headers: this.authService.authHeader(), responseType: "text" });
  }

  getPublicModels(): Observable<Model[]> {
    return this.http.get<Model[]>(`${Configuration.settings.apiURL}/model/publicmodels`, { headers: this.authService.authHeader() });
  }

  getModelById(modelId: string): Observable<Model> {
    return this.http.get<Model>(`${Configuration.settings.apiURL}/model/byid/${modelId}`, { headers: this.authService.authHeader() });
  }

}
