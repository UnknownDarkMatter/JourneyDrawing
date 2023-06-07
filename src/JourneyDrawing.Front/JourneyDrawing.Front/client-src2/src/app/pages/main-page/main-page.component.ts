import { Component, OnInit } from '@angular/core';
import { Port } from 'src/app/model/port';

@Component({
  selector: 'app-main-page',
  templateUrl: './main-page.component.html',
  styleUrls: ['./main-page.component.scss'],
})
export class MainPageComponent implements OnInit {
  public ports: Port[] = [
    { leftPixel: 200, topPixel: 200, gpsN: 40.7128, gpsW: 74.006 },
  ];

  ngOnInit(): void {
    for (let port of this.ports) {
    }
  }
}
