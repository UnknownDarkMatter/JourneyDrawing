using ExtractPixels.MapProcessing.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtractPixels.MapProcessing;

public class TripGenerator
{
    public const int NbPixelsPointsEqual = 2;

    /// <summary>
    /// [S sart, S end, SeaTrip]
    /// </summary>
    public Dictionary<int, Dictionary<int, SeaTrip>> SeaTrips;

    public TripGenerator()
    {
        SeaTrips = new Dictionary<int, Dictionary<int, SeaTrip>>();
    }

    public void CalculateAllTrips(BorderPointCollection borderWalkingPoints,
        decimal width, decimal height, Bitmap image, int filterS1, int filterS2)
    {
        long maxCount = borderWalkingPoints.BorderWalkingPoints.Keys.Count
            * borderWalkingPoints.BorderWalkingPoints.Keys.Count;
        long count = 0;
        foreach (var sStart in borderWalkingPoints.BorderWalkingPoints.Keys.Where(m=>m == filterS1))
        {
            var fromStartDestinations = new Dictionary<int, SeaTrip>();
            SeaTrips.Add(sStart, fromStartDestinations);

            foreach (var sEnd in borderWalkingPoints.BorderWalkingPoints.Keys.Where(m => m == filterS2))
            {
                if (sStart == sEnd) { continue; }

                var seaTrip = CalculateSingleTrip(sStart, sEnd, borderWalkingPoints, width, height, image);
                fromStartDestinations.Add(sEnd, seaTrip);
                count++;
            }
        }
    }

    private SeaTrip CalculateSingleTrip(int sStart, int sEnd, BorderPointCollection borderWalkingPoints,
        decimal width, decimal height, Bitmap image)
    {
        var pStartOnEarth = borderWalkingPoints.BorderWalkingPoints[sStart];
        var pEndOnEarth = borderWalkingPoints.BorderWalkingPoints[sEnd];
        int sLine = 1;
        var line = MapUtils.GetLine(pStartOnEarth.Point, pEndOnEarth.Point, width, height, 0, ref sLine);

        //back and forth : d'un côté à l'autre
        BorderWalkingPoint pForthOnLine = line.GetClosest(pStartOnEarth);
        BorderWalkingPoint pForthOnEarthWay1 = pStartOnEarth;
        BorderWalkingPoint pForthOnEarthWay2 = pStartOnEarth;
        BorderWalkingPoint pBackOnLine = line.GetClosest(pStartOnEarth);
        BorderWalkingPoint pBackOnEarthWay1 = pStartOnEarth;
        BorderWalkingPoint pBackOnEarthWay2 = pStartOnEarth;
        bool pForthOnIsOnLine = true;
        bool pBackOnIsOnLine = true;
        var listFoundForth = new List<BorderWalkingPoint>();
        var listFoundBack = new List<BorderWalkingPoint>();
        var listForthOnLine = new List<BorderWalkingPoint>();
        var listForthOnEarthWay1 = new List<BorderWalkingPoint>();
        var listForthOnEarthWay2 = new List<BorderWalkingPoint>();
        var listBackOnLine = new List<BorderWalkingPoint>();
        var listBackOnEarthWay1 = new List<BorderWalkingPoint>();
        var listBackOnEarthWay2 = new List<BorderWalkingPoint>();

        bool foundForth = false;
        bool foundBack = false;
        while (!foundForth && !foundBack)
        {
            //on memorise les points courants (y compris le premier)
            listForthOnLine.Add(pForthOnLine);
            listForthOnEarthWay1.Add(pForthOnEarthWay1);
            listForthOnEarthWay2.Add(pForthOnEarthWay2);
            listBackOnLine.Add(pBackOnLine);
            listBackOnEarthWay1.Add(pBackOnEarthWay1);
            listBackOnEarthWay2.Add(pBackOnEarthWay2);

            //on avance
            if (pForthOnIsOnLine)
            {
                pForthOnLine = line.BorderWalkingPoints[pForthOnLine.SPlus1];
            }
            else
            {
                pForthOnEarthWay1 = borderWalkingPoints.BorderWalkingPoints[pForthOnEarthWay1.SPlus1];
                pForthOnEarthWay2 = borderWalkingPoints.BorderWalkingPoints[pForthOnEarthWay2.SMinus1];
            }
            if (pBackOnIsOnLine)
            {
                pBackOnLine = line.BorderWalkingPoints[pBackOnLine.SMinus1];
            }
            else
            {
                pBackOnEarthWay1 = borderWalkingPoints.BorderWalkingPoints[pBackOnEarthWay1.SPlus1];
                pBackOnEarthWay2 = borderWalkingPoints.BorderWalkingPoints[pBackOnEarthWay2.SMinus1];
            }

            //apres un pas en avant on repere où on est
            if (pForthOnIsOnLine)
            {
                if (!IsPointInSea(pForthOnLine, image))
                {
                    pForthOnIsOnLine = false;
                    pForthOnEarthWay1 = borderWalkingPoints.GetClosest(pForthOnLine);
                    pForthOnEarthWay2 = borderWalkingPoints.GetClosest(pForthOnLine);
                    listFoundForth.AddRange(listForthOnLine);
                    listForthOnEarthWay1 = new List<BorderWalkingPoint>();
                    listForthOnEarthWay2 = new List<BorderWalkingPoint>();
                }
            }
            else
            {
                if (IsPointOnLine(pForthOnEarthWay1, line))
                {
                    pForthOnIsOnLine = true;

                    listFoundForth.AddRange(listForthOnEarthWay1);
                    listForthOnLine = new List<BorderWalkingPoint>();
                }
                else if (IsPointOnLine(pForthOnEarthWay2, line))
                {
                    pForthOnIsOnLine = true;

                    listFoundForth.AddRange(listForthOnEarthWay2);
                    listForthOnLine = new List<BorderWalkingPoint>();
                }
            }
            if (pBackOnIsOnLine)
            {
                if (!IsPointInSea(pBackOnLine, image))
                {
                    pBackOnIsOnLine = false;
                    pBackOnEarthWay1 = borderWalkingPoints.GetClosest(pBackOnLine);
                    pBackOnEarthWay2 = borderWalkingPoints.GetClosest(pBackOnLine);
                    listFoundBack.AddRange(listBackOnLine);
                    listBackOnEarthWay1 = new List<BorderWalkingPoint>();
                    listBackOnEarthWay2 = new List<BorderWalkingPoint>();
                }
            }
            else
            {
                if (IsPointOnLine(pBackOnEarthWay1, line))
                {
                    pBackOnIsOnLine = true;

                    listFoundBack.AddRange(listBackOnEarthWay1);
                    listBackOnLine = new List<BorderWalkingPoint>();
                }
                else if (IsPointOnLine(pBackOnEarthWay2, line))
                {
                    pBackOnIsOnLine = true;

                    listFoundBack.AddRange(listBackOnEarthWay2);
                    listBackOnLine = new List<BorderWalkingPoint>();
                }
            }

            //trouvé ? -> fin
            if (pForthOnIsOnLine)
            {
                if (borderWalkingPoints.GetClosest(pForthOnLine).Equals(pEndOnEarth))
                {
                    foundForth = true;
                    listFoundForth.AddRange(listForthOnLine);
                }
            }
            else
            {
                if (pForthOnEarthWay1.Equals(pEndOnEarth))
                {
                    foundForth = true;
                    listFoundForth.AddRange(listForthOnEarthWay1);
                }
                else if (pForthOnEarthWay2.Equals(pEndOnEarth))
                {
                    foundForth = true;
                    listFoundForth.AddRange(listForthOnEarthWay2);
                }
            }
            if (pBackOnIsOnLine)
            {
                if (borderWalkingPoints.GetClosest(pBackOnLine).Equals(pEndOnEarth))
                {
                    foundBack = true;
                    listFoundBack.AddRange(listBackOnLine);
                }
            }
            else
            {
                if (pBackOnEarthWay1.Equals(pEndOnEarth))
                {
                    foundBack = true;
                    listFoundBack.AddRange(listBackOnEarthWay1);
                }
                else if(pBackOnEarthWay2.Equals(pEndOnEarth))
                {
                    foundBack = true;
                    listFoundBack.AddRange(listBackOnEarthWay2);
                }
            }

        }
        var listFound = foundForth ? listFoundForth : listFoundBack;
        var seaTrip = new SeaTrip(pStartOnEarth, pEndOnEarth, listFound);
        return seaTrip;
    }

    private bool IsPointInSea(BorderWalkingPoint point, Bitmap image)
    {
        return image.GetPixel(point.X, point.Y).ToArgb() == BorderWalkingPointExtractor.SeaColor.ToArgb();
    }
    private bool IsPointOnLine(BorderWalkingPoint point, BorderPointCollection line)
    {
        return line.GetPoints().Any(p => MapUtils.GetDistance(point.Point, p.Point) <= NbPixelsPointsEqual);
    }

}
