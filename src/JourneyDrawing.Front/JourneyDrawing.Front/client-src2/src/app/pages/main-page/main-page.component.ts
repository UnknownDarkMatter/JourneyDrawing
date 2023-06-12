import { Component, ElementRef, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { Port } from 'src/app/model/port';
import { fromLatLon } from 'src/app/calculations/utm-utils';
import { utmToPixels } from 'src/app/calculations/utm-to-pixels';
import { Size } from 'src/app/model/size';
import { Segment } from 'src/app/model/segment';


@Component({
  selector: 'app-main-page',
  templateUrl: './main-page.component.html',
  styleUrls: ['./main-page.component.scss'],
})
export class MainPageComponent implements OnInit, AfterViewInit {
 
  public ports3: Port[] = [{ name:"Pau", latitudeWGS84: 43.295755, longitudeWGS84: -0.368567 }];
  public ports2: Port[] = [{ name:"NY", latitudeWGS84: 40.78306, longitudeWGS84: -73.971249 }];
  public ports1: Port[] = [{ name:"Quatar", latitudeWGS84: 25.333698, longitudeWGS84: 51.229529 }];
  public ports: Port[] = [
    { name:"Pau", latitudeWGS84: 43.295755, longitudeWGS84: -0.368567 },
    { name:"NY", latitudeWGS84: 40.78306, longitudeWGS84: -73.971249 },
    { name:"Quatar", latitudeWGS84: 25.333698, longitudeWGS84: 51.229529 },
    { name:"Sud argentine", latitudeWGS84: -55.965350, longitudeWGS84: -67.148438 },
  ];
  public screenSize:Size= {
    width:1196,
    height:557,
  };

  public continentSeparators:Segment[] = [
    {
      segmentStart:{x:779,y:233}, 
      segmentEnd:{x:773,y:363}
    }
  ];

  constructor() {
  }
  
  ngOnInit(): void {

  }

  ngAfterViewInit(): void{

  }

  public getPortLeftPixels(port:Port){
    var utmCoordinates = fromLatLon(port.latitudeWGS84!, port.longitudeWGS84!);
    console.log(utmCoordinates);
    var pixelCoordinates = utmToPixels(utmCoordinates, this.screenSize);
    return pixelCoordinates.leftPixels;
  }
  
  public getPortTopPixels(port:Port){
    var utmCoordinates = fromLatLon(port.latitudeWGS84!, port.longitudeWGS84!);
    var pixelCoordinates = utmToPixels(utmCoordinates, this.screenSize);
    return pixelCoordinates.topPixels;
  }

}
