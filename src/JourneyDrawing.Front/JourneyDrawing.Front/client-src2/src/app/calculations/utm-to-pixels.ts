import { PixelCoordinates } from "../model/pixel-coordinates";
import { Size } from "../model/size";
import { UtmCoordinates } from "../model/utm-coordinates";

export const earthDiameterEquatorialMeters = 40075017;
                                           // 4515184
export const earthDiameterMeridionalMeters = 40007860;

export function utmToPixels(utmCoordinates:UtmCoordinates, mapSizePixels:Size):PixelCoordinates{
    let topPixels:number = 0;
    let leftPixels:number = 0;

    let zoneLetters = ['C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X'];
    let zoneLetterAsNumArray = zoneLetters.map((value, index) => {
        if(utmCoordinates.zoneLetter !== value) {return 0;}
        return index-10;
    });
    let zoneLetterAsNum = 0;
    for(let zoneArg of zoneLetterAsNumArray){
        zoneLetterAsNum+=zoneArg;
    }

    let refPointXPixels:number = (mapSizePixels.width/2);
    let refPointYPixels:number = (mapSizePixels.height/2);
    let squareWidthKm = earthDiameterEquatorialMeters/60000;
    let squareHeightKm = earthDiameterMeridionalMeters/(zoneLetters.length * 1000);
    let squareWidthPixels = mapSizePixels.width / 60;
    let squareHeightPixels = mapSizePixels.height / zoneLetters.length;

    topPixels = zoneLetterAsNum * squareHeightPixels;
    topPixels = refPointYPixels - topPixels;
    leftPixels = utmCoordinates.zoneNum * squareWidthPixels;

    return {
        topPixels:topPixels,
        leftPixels:leftPixels
    } as PixelCoordinates;
}