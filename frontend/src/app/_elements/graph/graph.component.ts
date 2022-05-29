import { AfterViewInit, Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { RgbColor } from '@syncfusion/ej2-angular-heatmap';
import Dataset, { ColumnInfo } from 'src/app/_data/Dataset';
import Model, { Layer } from 'src/app/_data/Model';

@Component({
  selector: 'app-graph',
  templateUrl: './graph.component.html',
  styleUrls: ['./graph.component.css']
})
export class GraphComponent implements AfterViewInit {

  @ViewChild('graphWrapper')
  wrapper!: ElementRef;
  @ViewChild('graphCanvas')
  canvas!: ElementRef;

  @Input() model?: Model;
  //@Input() inputCols: number = 1;

  @Input() lineThickness: number = 2;
  @Input() nodeRadius: number = 15;
  @Input() lineColor1: RgbColor = new RgbColor(0, 168, 232);
  @Input() lineColor2: RgbColor = new RgbColor(0, 70, 151);
  @Input() nodeColor: string = '#222277';
  @Input() borderColor: string = '#00a8e8';
  @Input() inputNodeColor: string = '#00a8e8';
  @Input() outputNodeColor: string = '#dfd7d7';

  private ctx!: CanvasRenderingContext2D;
  @Input() inputColumns?: string[];

  constructor() { }

  ngAfterViewInit(): void {
    const ctx = this.canvas.nativeElement.getContext('2d');
    if (ctx) {
      this.ctx = ctx;
    } else {
      console.warn('Could not get canvas context!');
    }

    window.addEventListener('resize', () => { this.resize() });
    this.update();
    this.resize();
  }

  layers: Node[][] = [];

  update() {
    this.layers.length = 0;

    let inputNodeIndex = 0;
    const inputLayer: Node[] = [];
    while (this.inputColumns && inputNodeIndex < this.inputColumns.length) {
      const x = 0.5 / (this.model!.hiddenLayers + 2);
      const y = (inputNodeIndex + 0.5) / this.inputColumns.length;
      const node = new Node(x, y, this.inputNodeColor);
      inputLayer.push(node);
      inputNodeIndex += 1;
    }
    this.layers.push(inputLayer);

    let layerIndex = 1;
    while (layerIndex < this.model!.hiddenLayers + 1) {
      const newLayer: Node[] = [];
      let nodeIndex = 0;
      while (nodeIndex < this.model!.layers[layerIndex - 1].neurons) {
        const x = (layerIndex + 0.5) / (this.model!.hiddenLayers + 2);
        const y = (nodeIndex + 0.5) / this.model!.layers[layerIndex - 1].neurons;
        const node = new Node(x, y, this.nodeColor);
        newLayer.push(node);
        nodeIndex += 1;
      }
      this.layers.push(newLayer);
      layerIndex += 1;
    }

    const outX = 1 - (0.5 / (this.model!.hiddenLayers + 2));
    const outY = 0.5;
    this.layers.push([new Node(outX, outY, this.outputNodeColor)])
    this.draw();
  }

  draw() {
    this.ctx.clearRect(0, 0, this.canvas.nativeElement.width, this.canvas.nativeElement.height);

    let index = 0;
    while (index < this.layers!.length - 1) {
      for (let node1 of this.layers![index]) {
        for (let node2 of this.layers![index + 1]) {
          this.drawLine(node1, node2);
        }
      }
      index += 1;
    }

    for (let layer of this.layers!) {
      layer.forEach((node, index) => {
        this.drawNode(node, 0.5 / layer.length + 0.5);
      });
    }
  }

  bezierOffset = 5;

  drawLine(node1: Node, node2: Node) {
    const lineColor: RgbColor = this.lerpColor(this.lineColor1, this.lineColor2, node1.y);
    this.ctx.strokeStyle = `rgb(${lineColor.R}, ${lineColor.G}, ${lineColor.B})`;
    this.ctx.lineWidth = this.lineThickness;
    this.ctx.beginPath();
    this.ctx.moveTo(node1.x * this.width, node1.y * this.height);
    //this.ctx.lineTo(node2.x * this.width, node2.y * this.height);
    const middle = (node1.x + (node2.x - node1.x) / 2) * this.width;
    this.ctx.bezierCurveTo(
      middle, node1.y * this.height,
      middle, node2.y * this.height,
      node2.x * this.width, node2.y * this.height);
    this.ctx.stroke();
  }

  drawNode(node: Node, sizeMult: number) {
    const lineColor: RgbColor = this.lerpColor(this.lineColor1, this.lineColor2, node.y);
    this.ctx.strokeStyle = `rgb(${lineColor.R}, ${lineColor.G}, ${lineColor.B})`;
    this.ctx.fillStyle = node.color;
    this.ctx.lineWidth = this.lineThickness;
    this.ctx.beginPath();
    this.ctx.arc(node.x * this.width, node.y * this.height, this.nodeRadius * sizeMult, 0, 2 * Math.PI);
    this.ctx.fill();
    this.ctx.stroke();
  }

  width = 200;
  height = 200;
  ratio = 1;

  resize() {
    this.width = this.wrapper.nativeElement.offsetWidth;
    this.height = this.wrapper.nativeElement.offsetHeight;
    this.ratio = this.width / this.height;

    if (this.canvas) {
      this.canvas.nativeElement.width = this.width;
      this.canvas.nativeElement.height = this.height;
    }

    this.draw();
  }

  lerpColor(value1: RgbColor, value2: RgbColor, amount: number): RgbColor {
    const newColor = new RgbColor(0, 0, 0);
    amount = amount < 0 ? 0 : amount;
    amount = amount > 1 ? 1 : amount;
    newColor.R = value1.R + (value2.R - value1.R) * amount;
    newColor.G = value1.G + (value2.G - value1.G) * amount;
    newColor.B = value1.B + (value2.B - value1.B) * amount;
    return newColor;
  };
}

class Node {
  constructor(
    public x: number,
    public y: number,
    public color: string
  ) { }
}
