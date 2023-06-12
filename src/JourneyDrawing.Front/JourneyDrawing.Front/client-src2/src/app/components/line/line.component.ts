import { Component, Input } from '@angular/core';
import { DrawingPointsMultiplication, ImageOriginalSize, ImageReducedSize } from 'src/app/constants';
import { Point } from 'src/app/model/point';
import { Segment } from 'src/app/model/segment';


@Component({
  selector: 'app-line',
  templateUrl: './line.component.html',
  styleUrls: ['./line.component.scss']
})
export class LineComponent {
  @Input() public segment!:Segment;

  public getDots():Point[]{
    let points:Point[] = [];
    let p1 = this.segment.segmentStart.x <  this.segment.segmentEnd.x 
      ? this.segment.segmentStart 
      : this.segment.segmentEnd;
    let p2 = this.segment.segmentStart.x <  this.segment.segmentEnd.x 
      ? this.segment.segmentEnd 
      : this.segment.segmentStart;

    let a = (p1.y - p2.y) / (p1.x - p2.x);
    let b = p1.y - (a * p1.x );
    console.log(`a:${a}, b:${b}`);

    // console.log({x:this.segment.segmentStart.x, y:a*this.segment.segmentStart.x + b});
    // console.log({x:this.segment.segmentEnd.x, y:a*this.segment.segmentEnd.x + b});

    for(var x = p1.x ; x<=p2.x ; x++){
      for(var i = 1;i<= DrawingPointsMultiplication;i++){
        let newX = p1.x + (((p2.x - p1.x) * i)/DrawingPointsMultiplication);
        let y = (a * newX) + b;
        console.log({x:newX, y:y});
        points.push(this.scalePoint({x:newX, y:y}));
        }
    }

    return points;
  }

  private scalePoint(p:Point):Point{
    return {
      x:(p.x*(ImageReducedSize.w / ImageOriginalSize.w)), 
      y:(p.y*(ImageReducedSize.h / ImageOriginalSize.h))
    } as Point
  }
}
