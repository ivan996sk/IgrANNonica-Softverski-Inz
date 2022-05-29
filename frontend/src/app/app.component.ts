import { AfterViewInit, Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { Router, NavigationEnd, ActivatedRoute } from '@angular/router';
import { filter, map } from 'rxjs';
import { AuthService } from './_services/auth.service';
import { SignalRService } from './_services/signal-r.service';
import { HttpClient } from '@angular/common/http';
import Shared from './Shared';
import { Chart } from 'chart.js';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit, AfterViewInit {
  constructor(private router: Router, private titleService: Title, private authService: AuthService, private signalRService: SignalRService, private http: HttpClient) {

    const getOrCreateTooltip = (chart: { canvas: { parentNode: { querySelector: (arg0: string) => any; appendChild: (arg0: any) => void; }; }; }) => {
      let tooltipEl = chart.canvas.parentNode.querySelector('div');

      if (!tooltipEl) {
        tooltipEl = document.createElement('div');
        tooltipEl.style.background = 'rgba(0, 0, 0, 0.7)';
        tooltipEl.style.borderRadius = '3px';
        tooltipEl.style.color = 'white';
        tooltipEl.style.opacity = 1;
        tooltipEl.style.pointerEvents = 'none';
        tooltipEl.style.position = 'absolute';
        tooltipEl.style.transform = 'translate(-50%, 0)';
        tooltipEl.style.transition = 'all .1s ease';
        tooltipEl.style.zIndex = 9000;

        tooltipEl.classList.add("clearfix");

        const table = document.createElement('table');
        table.style.margin = '0px';

        tooltipEl.appendChild(table);
        chart.canvas.parentNode.appendChild(tooltipEl);
      }

      return tooltipEl;
    };

    const externalTooltipHandler = (context: { chart: any; tooltip: any; }) => {
      // Tooltip Element
      const { chart, tooltip } = context;
      const tooltipEl = getOrCreateTooltip(chart);

      // Hide if no tooltip
      if (tooltip.opacity === 0) {
        tooltipEl.style.opacity = 0;
        return;
      }

      // Set Text
      if (tooltip.body) {
        const titleLines = tooltip.title || [];
        const bodyLines = tooltip.body.map((b: { lines: any; }) => b.lines);

        const tableHead = document.createElement('thead');

        titleLines.forEach((title: string) => {
          const tr = document.createElement('tr');
          tr.style.borderWidth = '' + 0;

          const th = document.createElement('th');
          th.style.borderWidth = '' + 0;
          const text = document.createTextNode(title);

          th.appendChild(text);
          tr.appendChild(th);
          tableHead.appendChild(tr);
        });

        const tableBody = document.createElement('tbody');
        bodyLines.forEach((body: string, i: string | number) => {
          const colors = tooltip.labelColors[i];

          const span = document.createElement('span');
          span.style.background = colors.backgroundColor;
          span.style.borderColor = colors.borderColor;
          span.style.borderWidth = '2px';
          span.style.marginRight = '10px';
          span.style.height = '10px';
          span.style.width = '10px';
          span.style.display = 'inline-block';

          const tr = document.createElement('tr');
          tr.style.backgroundColor = 'inherit';
          tr.style.borderWidth = '' + 0;

          const td = document.createElement('td');
          td.style.borderWidth = '' + 0;

          const text = document.createTextNode(body);

          td.appendChild(span);
          td.appendChild(text);
          tr.appendChild(td);
          tableBody.appendChild(tr);
        });

        const tableRoot = tooltipEl.querySelector('table');

        // Remove old children
        while (tableRoot.firstChild) {
          tableRoot.firstChild.remove();
        }

        // Add new children
        tableRoot.appendChild(tableHead);
        tableRoot.appendChild(tableBody);
      }

      const { offsetLeft: positionX, offsetTop: positionY } = chart.canvas;

      // Display, position, and set styles for font
      tooltipEl.style.opacity = 1;
      tooltipEl.style.left = positionX + tooltip.caretX + 'px';
      tooltipEl.style.top = positionY + tooltip.caretY + 'px';
      tooltipEl.style.font = tooltip.options.bodyFont.string;
      tooltipEl.style.padding = tooltip.options.padding + 'px ' + tooltip.options.padding + 'px';
    };

    Chart.defaults.plugins.tooltip.enabled = false;
    Chart.defaults.plugins.tooltip.position = 'nearest';
    Chart.defaults.plugins.tooltip.external = externalTooltipHandler;
  }
  ngAfterViewInit(): void {
  }

  ngOnInit() {
    this.router.events
      .pipe(
        filter((event) => event instanceof NavigationEnd),
        map(() => {
          let route: ActivatedRoute = this.router.routerState.root;
          let routeTitle = '';
          while (route!.firstChild) {
            route = route.firstChild;
          }
          if (route.snapshot.data['title']) {
            routeTitle = route!.snapshot.data['title'];
          }
          return routeTitle;
        })
      )
      .subscribe((title: string) => {
        if (title) {
          this.titleService.setTitle(`${title} - Igrannonica`);
        }
      });
    if (!this.authService.isAuthenticated()) {
      if (!this.authService.alreadyGuest())
        this.authService.addGuestToken();
    }
    this.signalRService.startConnection();
    //this.startHttpRequest();
  }
}
