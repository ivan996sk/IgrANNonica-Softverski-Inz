import { Component, OnInit } from '@angular/core';
import { MatSliderChange } from '@angular/material/slider';
import { CookieService } from 'ngx-cookie-service';

@Component({
  selector: 'app-playground',
  templateUrl: './playground.component.html',
  styleUrls: ['./playground.component.css']
})
export class PlaygroundComponent implements OnInit {

  animateBackground = true;
  backgroundFill = 1.0;

  constructor(private cookie: CookieService) { }

  updateFillPref(event: MatSliderChange) {
    this.backgroundFill = event.value!;
    this.cookie.set('backgroundFill', "" + this.backgroundFill);
  }

  updateAnimPref() {
    this.cookie.set('animateBackground', "" + this.animateBackground);
  }

  ngOnInit(): void {
    if (this.cookie.check('animateBackground')) {
      this.animateBackground = this.cookie.get('animateBackground') == 'true';
    }
    if (this.cookie.check('backgroundFill')) {
      this.backgroundFill = parseFloat(this.cookie.get('backgroundFill'));
    }
  }

}
