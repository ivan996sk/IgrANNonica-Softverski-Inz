import { Component, OnInit ,Input,Output, ViewChild} from '@angular/core';
import Predictor from 'src/app/_data/Predictor';
import {PredictorsService } from 'src/app/_services/predictors.service';
import { ActivatedRoute } from '@angular/router';
@Component({
  selector: 'app-form-predictor',
  templateUrl: './form-predictor.component.html',
  styleUrls: ['./form-predictor.component.css']
})
export class FormPredictorComponent implements OnInit {
  inputs : Column[] = [];

  predictor?:Predictor ;
  predictorsForExp: { [expId: string]: Predictor[] } = {}
  predictorsService: any;
  experimentsService: any;
  folders: any;
  constructor(private predictS : PredictorsService, private route: ActivatedRoute) {
   
   }

  ngOnInit(): void {
   /* this.route.params.subscribe(url => {
      console.log("**********************************************");
           this.predictS.getPredictor(url["id"]).subscribe(p => {
      
        this.predictor = p;
        this.predictor.inputs.forEach((p,index)=> this.inputs[index] = new Column(p, ""));
      })
    });
  }

*/
}

}

export class Column {
  constructor(
    public name : string, 
    public value : (number | string)){
  }}
