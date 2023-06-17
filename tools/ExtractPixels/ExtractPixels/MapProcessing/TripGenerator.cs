using ExtractPixels.MapProcessing.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtractPixels.MapProcessing;

public class TripGenerator
{
    /// <summary>
    /// [S sart, S end, SeaTrip]
    /// </summary>
    public Dictionary<int, Dictionary<int, SeaTrip>> SeaTrips;

    public TripGenerator()
    {
        SeaTrips = new Dictionary<int,Dictionary<int, SeaTrip>>();
    }

    public void CalculateAllTrips(BorderPointCollection borderWalkingPoints, decimal width, decimal height)
    {
        long maxCount = borderWalkingPoints.BorderWalkingPoints.Keys.Count 
            * borderWalkingPoints.BorderWalkingPoints.Keys.Count;
        long count = 0;
        foreach (var sStart in borderWalkingPoints.BorderWalkingPoints.Keys)
        {
            var fromStartDestinations = new Dictionary<int, SeaTrip>();
            SeaTrips.Add(sStart, fromStartDestinations);

            foreach (var sEnd in borderWalkingPoints.BorderWalkingPoints.Keys)
            {
                if(sStart == sEnd) { continue; }

                var seaTrip = CalculateSingleTrip(sStart, sEnd, borderWalkingPoints, width, height);
                fromStartDestinations.Add(sEnd, seaTrip);
                count++;
            }
        }
    }

    private SeaTrip CalculateSingleTrip(int sStart, int sEnd, BorderPointCollection borderWalkingPoints, 
        decimal width, decimal height)
    {
        var pStart = borderWalkingPoints.BorderWalkingPoints[sStart];
        var pEnd = borderWalkingPoints.BorderWalkingPoints[sEnd];
        int sLine = 1;
        var line = MapUtils.GetLine(pStart.Point, pEnd.Point, width, height, 0, ref sLine);

        BorderWalkingPoint pPlus1Line = line.GetClosest(pStart);
        BorderWalkingPoint pMinus1Line = line.GetClosest(pStart);
        BorderWalkingPoint pEndLine = line.GetClosest(pEnd);
        var foundPoints = new List<BorderWalkingPoint>();
        var pointsListPlus1 = new List<BorderWalkingPoint>();
        var pointsListMinus1 = new List<BorderWalkingPoint>();

        bool found = false;
        int count = 0;
        while (!found)
        {
            pPlus1Line = line.BorderWalkingPoints[pPlus1Line.SPlus1];
            pointsListPlus1.Add(pPlus1Line);
            pMinus1Line = line.BorderWalkingPoints[pMinus1Line.SMinus1];
            pointsListMinus1.Add(pMinus1Line);

            if (pPlus1Line.S == pEndLine.S)
            {
                foundPoints = pointsListPlus1;
                found = true;
            }
            if (pMinus1Line.S == pEndLine.S)
            {
                foundPoints = pointsListMinus1;
                found = true;
            }
            count++;
            if(count> line.BorderWalkingPoints.Count + 20)
            {

            }
        }

        var seaTrip = new SeaTrip(pStart, pEnd, foundPoints);
        return seaTrip;
    }
}
