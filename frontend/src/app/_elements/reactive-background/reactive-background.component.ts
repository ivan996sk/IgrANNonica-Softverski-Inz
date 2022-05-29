import { AfterViewInit, Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';
import Shared from 'src/app/Shared';

@Component({
  selector: 'app-reactive-background',
  templateUrl: './reactive-background.component.html',
  styleUrls: ['./reactive-background.component.css']
})
export class ReactiveBackgroundComponent implements AfterViewInit {

  @ViewChild('bgCanvas') canvasRef!: ElementRef;

  @Input() numPoints: number = 450;
  @Input() speed: number = 0.001; // 0-1
  @Input() scrollSpeed: number = 1;
  @Input() maxSize: number = 6;

  @Input() minDistance: number = 0.07; //0-1
  @Input() cursorDistance: number = 0.07;

  @Input() bgColor: string = '#222277';
  @Input() lineColor: string = '#ffffff';
  @Input() pointColor: string = '#ffffff';
  @Input() cursorLineColor: string = '#ff0000';

  @Input() animate: boolean = true;
  @Input() fill: number = 1.0;

  private fleeSpeed = 0.005;

  private points: Point[] = [];

  private width = 200;
  private height = 200;
  private ratio = 1;

  private canvas!: HTMLCanvasElement;
  private ctx!: CanvasRenderingContext2D;

  private time: number = 0;

  constructor(private cookie: CookieService) { }

  private mouseX = 0;
  private mouseY = 0;

  ngAfterViewInit(): void {
    document.addEventListener('mousemove', (e) => {
      this.mouseX = e.clientX / this.width;
      this.mouseY = e.clientY / this.height;
    })

    document.addEventListener('mouseleave', _ => {
      this.mouseX = -1;
      this.mouseY = -1;
    })

    document.addEventListener('scroll', (e) => {
      this.scrollBackground(e);
    })

    this.canvas = (<HTMLCanvasElement>this.canvasRef.nativeElement);
    const ctx = this.canvas.getContext('2d');
    if (ctx) {
      this.ctx = ctx;
    } else {
      console.warn('Could not get canvas context!');
    }

    let i = 0;
    while (i < this.numPoints) {
      const x = Math.random();
      const y = Math.random();
      const size = (Math.random() * 0.8 + 0.2) * this.maxSize;
      const direction = Math.random() * 360;
      this.points.push(new Point(x, y, size, direction));
      i++;
    }

    window.addEventListener('resize', () => { this.resize() });
    this.resize();

    setInterval(() => {
      if (this.cookie.check('animateBackground')) {
        this.animate = this.cookie.get('animateBackground') == 'true';
      }
      if (this.cookie.check('backgroundFill')) {
        this.fill = parseFloat(this.cookie.get('backgroundFill'));
      }

      this.drawBackground();
    }, 1000 / 60);

    Shared.bgScroll.subscribe((amount) => {
      this.scrollBackgroundFromSharedEvent(amount);
    })
  }

  private lastScrollY: number = 0;

  scrollBackgroundFromSharedEvent(amount: number) {
    const scrolledAmount = amount - this.lastScrollY;
    this.scrollPoints(scrolledAmount);
    this.lastScrollY = amount;
  }

  scrollBackground(e: Event) {
    const scrolledAmount = window.scrollY - this.lastScrollY;
    this.scrollPoints(scrolledAmount);
    this.lastScrollY = window.scrollY;
  }

  scrollPoints(amount: number) {
    this.points.forEach((point, index) => {
      if (index > this.numPoints * this.fill) return;
      point.y = point.y - (amount / this.height) * this.scrollSpeed;
      this.keepPointWithinBounds(point);
    })
  }

  drawBackground() {
    if (!this.ctx || !this.canvas) return;
    this.ctx.clearRect(0, 0, this.canvas.width, this.canvas.height);

    //this.ctx.fillStyle = this.bgColor;
    //this.ctx.fillRect(0, 0, this.width, this.height);

    this.points.forEach((point, index) => {
      if (index > this.numPoints * this.fill) return;

      this.drawLines(point, index);
      this.drawPoint(point);

      if (this.animate)
        this.updatePoint(point);
    });

    //this.drawPoint(new Point(this.mouseX, this.mouseY, 12, 0));
  }

  drawLines(p: Point, index: number) {
    let i = index + 1;
    while (i < this.points.length * this.fill) {
      const otherPoint = this.points[i];

      const dist = this.distance(p.x, p.y, otherPoint.x, otherPoint.y);
      if (dist < this.minDistance) {
        const h = HEX[Math.round((1 - dist / this.minDistance) * 16)]
        this.ctx.strokeStyle = this.lineColor + h;
        this.ctx.lineWidth = this.maxSize / 2;
        this.ctx.beginPath();
        this.ctx.moveTo(p.x * this.width, p.y * this.height);
        this.ctx.lineTo(otherPoint.x * this.width, otherPoint.y * this.height);
        this.ctx.stroke();
      }

      i++;
    }
  }

  drawPoint(p: Point) {
    this.ctx.fillStyle = this.pointColor;
    this.ctx.beginPath();
    this.ctx.arc(p.x * this.width, p.y * this.height, p.size * this.screenDepth(p.x), 0, 2 * Math.PI);
    this.ctx.fill();
  }

  resize() {
    this.width = window.innerWidth;
    this.height = window.innerHeight;
    this.ratio = this.width / this.height;

    if (this.canvas) {
      this.canvas.width = this.width;
      this.canvas.height = this.height;
    }

    if (this.cookie.check('animateBackground')) {
      this.animate = this.cookie.get('animateBackground') == 'true';
    }
    if (this.cookie.check('backgroundFill')) {
      this.fill = parseFloat(this.cookie.get('backgroundFill'));
    }

    this.drawBackground();
  }

  updatePoint(p: Point) {
    const mx = this.mouseX;
    const my = this.mouseY;
    const distToCursor = this.distance(p.x, p.y, mx, my);

    if (distToCursor < this.cursorDistance) {

      const t = (distToCursor / this.cursorDistance);
      p.x -= ((mx - p.x) / distToCursor) * this.speed * (1 + t * 2);
      p.y -= ((my - p.y) / distToCursor) * this.speed * (1 + t * 2);

      p.direction = this.lerp(p.direction, Math.atan2(my - p.y, mx - p.x) * 180 / Math.PI, t);

      const grd = this.ctx.createLinearGradient(p.x * this.width, p.y * this.height, mx * this.width, my * this.height);
      const alpha = HEX[Math.round(p.size / this.maxSize * (HEX.length - 1))];
      grd.addColorStop(0, this.cursorLineColor + alpha);
      grd.addColorStop(0.5, this.cursorLineColor + '00');
      this.ctx.strokeStyle = grd;
      this.ctx.beginPath();
      this.ctx.moveTo(p.x * this.width, p.y * this.height);
      this.ctx.lineTo(mx * this.width, my * this.height);
      this.ctx.stroke();
    }

    const vx = Math.sin(p.direction);
    const vy = Math.cos(p.direction);

    p.x = p.x + vx * this.speed;
    p.y = p.y + vy * this.speed;

    this.keepPointWithinBounds(p);
  }

  lerp(start: number, end: number, amt: number) {
    return (1 - amt) * start + amt * end
  }

  keepPointWithinBounds(p: Point) {
    p.x = p.x % 1.0;
    p.y = p.y % 1.0;
    p.x = ((1 - Math.sign(p.x)) / 2) + p.x;
    p.y = ((1 - Math.sign(p.y)) / 2) + p.y;
  }

  distance(x1: number, y1: number, x2: number, y2: number): number {
    return Math.sqrt(((x2 - x1) ** 2) + ((y2 / this.ratio - y1 / this.ratio) ** 2) / this.screenDepth(x1)) * this.ratio;
  }

  screenDepth(x: number): number {
    return (1.5 - Math.sin(x * Math.PI));
  }
}

class Point {
  constructor(
    public x: number,
    public y: number,
    public size: number,
    public direction: number
  ) { }
}

const HEX = ['00', '11', '22', '33', '44', '55', '66', '77', '88', '99', 'aa', 'bb', 'cc', 'dd', 'ee', 'ff'];