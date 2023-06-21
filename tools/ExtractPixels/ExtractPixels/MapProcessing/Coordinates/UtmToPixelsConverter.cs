using ExtractPixels.MapProcessing.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtractPixels.MapProcessing.Coordinates;

/// <summary>
/// Cponverts back and forth Pixels coordinates to UTM coordinates
/// </summary>
public static class UtmToPixelsConverter
{
    private const decimal EarthDiameterEquatorialMeters = 40075017;
    private const decimal EarthDiameterMeridionalMeters = 40007860;


    public static MapPoint UtmToPixels(UtmCoordinates utmCoordinates, MapSize mapSizePixels)
    {
        decimal topPixels = 0;
        decimal leftPixels = 0;

        var zoneLetters = new List<string>() { "C", "D", "E", "F", "G", "H", "J", "K", "L", "M", "N", "P", "Q", "R", "S", "T", "U", "V", "W", "X" };
        var zoneLetterAsNumArray = zoneLetters.Select((value, index) => {
            if (utmCoordinates.ZoneLetter != value) { return 0; }
            return index - 10;
        });

        var zoneLetterAsNum = 0;
        foreach (var zoneArg in zoneLetterAsNumArray)
        {
            zoneLetterAsNum += zoneArg;
        }

        var refPointXPixels = ((decimal)mapSizePixels.Width / 2);
        var refPointYPixels = ((decimal)mapSizePixels.Height / 2);
        var squareWidthKm = EarthDiameterEquatorialMeters / 60000;
        var squareHeightKm = EarthDiameterMeridionalMeters / (zoneLetters.Count * 1000);
        var squareWidthPixels = (decimal)mapSizePixels.Width / 60;
        var squareHeightPixels = (decimal)mapSizePixels.Height / zoneLetters.Count;

        topPixels = (decimal)zoneLetterAsNum * (decimal)squareHeightPixels;
        topPixels = (decimal)refPointYPixels - (decimal)topPixels;
        leftPixels = (decimal)utmCoordinates.ZoneNum * (decimal)squareWidthPixels;

        return new MapPoint()
        {
            Y = (int)topPixels,
            X = (int)leftPixels
        };

    }
}