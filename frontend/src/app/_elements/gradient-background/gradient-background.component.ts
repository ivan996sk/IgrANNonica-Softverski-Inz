import { AfterViewInit, Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';

@Component({
  selector: 'app-gradient-background',
  templateUrl: './gradient-background.component.html',
  styleUrls: ['./gradient-background.component.css']
})
export class GradientBackgroundComponent implements AfterViewInit {

  @ViewChild('holder') holderRef!: ElementRef;
  private holder!: HTMLDivElement;


  @Input() colorHorizontal1 = 'rgba(0, 8, 45, 0.5)';
  @Input() colorHorizontal2 = 'rgba(0, 52, 89, 0.5)';

  @Input() colorVertical1 = 'rgba(0, 52, 89, 0.5)';
  @Input() colorVertical2 = 'rgba(0, 152, 189, 0.5)';

  constructor() { }

  color: string = this.gradientHorizontal();

  private width = 0;
  private height = 0;

  gradientHorizontal(): string {
    return `linear-gradient(90deg, ${this.colorHorizontal1} 0%, ${this.colorHorizontal2} 50%, ${this.colorHorizontal1} 100%), linear-gradient(0deg, ${this.colorVertical1} 0%, ${this.colorVertical2} 100%)`;
  }

  resize() {
    this.width = window.innerWidth;
    this.height = window.innerHeight;

    this.holder.style.width = this.width + 'px';
    this.holder.style.height = this.height + 'px';
  }

  ngAfterViewInit(): void {
    this.holder = <HTMLDivElement>this.holderRef.nativeElement;

    window.addEventListener('resize', () => this.resize());
    this.resize();

  }

}
