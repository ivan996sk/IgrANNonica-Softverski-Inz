import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Configuration } from './configuration.service';
import Predictor from '../_data/Predictor';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class PredictorsService {
  constructor(private http: HttpClient, private authService: AuthService) { }

  getPublicPredictors(): Observable<Predictor[]> {
    return this.http.get<Predictor[]>(`${Configuration.settings.apiURL}/predictor/publicpredictors`, { headers: this.authService.authHeader() });
  }
  getPredictor(id: String): Observable<Predictor> {
    return this.http.get<Predictor>(`${Configuration.settings.apiURL}/predictor/getpredictor/` + id, { headers: this.authService.authHeader() });
  }

  usePredictor(predictor: Predictor, inputs: Column[]) {
    return this.http.post(`${Configuration.settings.apiURL}/predictor/usepredictor/` + predictor._id, inputs, { headers: this.authService.authHeader() });
  }

  deletePredictor(predictor: Predictor) {
    return this.http.delete(`${Configuration.settings.apiURL}/predictor/` + predictor._id, { headers: this.authService.authHeader(), responseType: "text" });
  }

  getMyPredictors(): Observable<Predictor[]> {
    return this.http.get<Predictor[]>(`${Configuration.settings.apiURL}/predictor/mypredictors`, { headers: this.authService.authHeader() });
  }
}

export class Column {
  constructor(
    public name: string,
    public value: (number | string)) {
  }
}