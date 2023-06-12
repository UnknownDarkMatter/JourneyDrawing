import { Point } from "../model/point";
import { Segment } from "../model/segment";

export function projectPointOnSegment(segment:Segment, point:Point):Segment {
    let projection = projecPointOnLine(segment.segmentStart, segment.segmentEnd, point);
    return {
        segmentStart:point,
        segmentEnd:projection
    } as Segment;
}

function sqr(x:number):number { return x * x }

function addPoints(v1:Point,v2:Point):Point{
    return {
        x : v1.x + v2.x,
        y : v1.y + v2.y
    } as Point;
}
function substractPoints(v1:Point,v2:Point):Point{
    return {
        x : -v1.x + v2.x,
        y : -v1.y + v2.y
    } as Point;
}

function dotProduct(v1:Point,v2:Point):number{
    return (v1.x * v2.x) + (v1.y * v2.y);
}

function length_squared(v1:Point,v2:Point):number{
    let v = substractPoints(v1, v2);
    let vDiff = addPoints(v2, v);
    return sqr(vDiff.x) + sqr(vDiff.y);
}

function projecPointOnLine(v:Point, w:Point, p:Point):Point {
    // Return minimum distance between line segment vw and point p
    const l2 = length_squared(v, w);  // i.e. |w-v|^2 -  avoid a sqrt
    // Consider the line extending the segment, parameterized as v + t (w - v).
    // We find projection of point p onto the line. 
    // It falls where t = [(p-v) . (w-v)] / |w-v|^2
    const  t = dotProduct(substractPoints(v , p), substractPoints(v, w)) / l2;
    let vTmp = substractPoints(v, w);
    vTmp.x = vTmp.x * t;
    vTmp.y = vTmp.y * t;
    let projection = addPoints(v, vTmp);
    return projection;
  }