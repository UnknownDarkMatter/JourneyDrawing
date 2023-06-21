using ExtractPixels.MapProcessing.Model;
using ExtractPixels.MapProcessing.MultiThreading;
using ExtractPixels.MapProcessing.MultiThreading.Business;
using System.Drawing;

namespace ExtractPixels.MapProcessing;

public class TripGenerator
{
    public const int NbPixelsPointsEqual = 0;
    public const int NbPixelsNeighBours = 1;
    public const int CountBeforeTellingItIsALine = 4;

    public static long MaxCount;
    public static long ComputationCount;

    /// <summary>
    /// [S sart, S end, SeaTrip]
    /// </summary>
    public Dictionary<int, Dictionary<int, SeaTrip>> SeaTrips;

    public TripGenerator()
    {
        SeaTrips = new Dictionary<int, Dictionary<int, SeaTrip>>();
    }

    public void CalculateAllTrips(BorderPointCollection borderWalkingPoints,
        decimal width, decimal height, string imageFilePath, Bitmap image, int? filterS1, int? filterS2)
    {
        Console.WriteLine($"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} : Preparing multithreading computing for generation of trips ...");

        MaxCount = borderWalkingPoints.BorderWalkingPoints.Keys.Count * borderWalkingPoints.BorderWalkingPoints.Keys.Count;
        ComputationCount = 0;

        var scheduler = new WorkScheduler<MyData, MyWorker>();
        var myDatas = new List<MyData>();

        foreach (var sStart in borderWalkingPoints.BorderWalkingPoints.Keys.Where(m=> filterS1 == null || m == filterS1))
        {
            var fromStartDestinations = new Dictionary<int, SeaTrip>();
            SeaTrips.Add(sStart, fromStartDestinations);

            foreach (var sEnd in borderWalkingPoints.BorderWalkingPoints.Keys.Where(m => filterS2 == null || m == filterS2))
            {
                if (sStart == sEnd) { continue; }

                var mydata = new MyData(sStart, sEnd, borderWalkingPoints, width, height, imageFilePath, image);
                myDatas.Add(mydata);


                //this code is moved into the scheduler for multithreading
                //var seaTrip = CalculateSingleTrip(sStart, sEnd, borderWalkingPoints, width, height, imageFilePath, image);
                //fromStartDestinations.Add(sEnd, seaTrip);


                //ComputationCount++;
                var rest = (int)(ComputationCount % (MaxCount * 0.1M));
                if (rest == 0 || rest == (MaxCount * 0.1M))
                {
                    Console.WriteLine($"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} : DONE {(int)(100 * (((decimal)ComputationCount / (decimal)MaxCount)))}%");
                }

            }
        }

        Console.WriteLine($"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} : Starting generation of trips...");
        ComputationCount = 0;
        scheduler.Run(myDatas);

        foreach(var myData in myDatas)
        {
            SeaTrips[myData.sStart].Add(myData.sEnd, myData.SeaTrip);
        }
    }

    public SeaTrip CalculateSingleTrip(int sStart, int sEnd, BorderPointCollection borderWalkingPoints,
        decimal width, decimal height, string imageFilePath, Bitmap image)
    {
        var pStartOnEarth = borderWalkingPoints.BorderWalkingPoints[sStart];
        var pEndOnEarth = borderWalkingPoints.BorderWalkingPoints[sEnd];
        int sLine = 1;
        var line = MapUtils.GetLine(pStartOnEarth.Point, pEndOnEarth.Point, width, height, 0, ref sLine);
        string debugImagePath = imageFilePath + "_debug.png";
        var imageDebug = MapUtils.Clone(image);

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
        var pForth_CountBeforeTellingItIsALine = 0;
        var pBack_CountBeforeTellingItIsALine = 0;

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
                if (Constants.IsDebug)
                {
                    imageDebug.SetPixel(pForthOnLine.X, pForthOnLine.Y, Color.Pink);
                    imageDebug.Save(debugImagePath);
                }

                pForthOnLine = line.BorderWalkingPoints[pForthOnLine.SPlus1];
            }
            else
            {
                if (Constants.IsDebug)
                {
                    imageDebug.SetPixel(pForthOnEarthWay1.X, pForthOnEarthWay1.Y, Color.Red);
                    imageDebug.SetPixel(pForthOnEarthWay2.X, pForthOnEarthWay2.Y, Color.Green);
                    imageDebug.Save(debugImagePath);
                }

                pForthOnEarthWay1 = borderWalkingPoints.BorderWalkingPoints[pForthOnEarthWay1.SPlus1];
                pForthOnEarthWay2 = borderWalkingPoints.BorderWalkingPoints[pForthOnEarthWay2.SMinus1];
            }
            if (pBackOnIsOnLine)
            {
                if (Constants.IsDebug)
                {
                    imageDebug.SetPixel(pForthOnLine.X, pForthOnLine.Y, Color.Violet);
                    imageDebug.Save(debugImagePath);
                }

                pBackOnLine = line.BorderWalkingPoints[pBackOnLine.SMinus1];
            }
            else
            {
                if (Constants.IsDebug)
                {
                    imageDebug.SetPixel(pForthOnEarthWay1.X, pForthOnEarthWay1.Y, Color.Red);
                    imageDebug.SetPixel(pForthOnEarthWay2.X, pForthOnEarthWay2.Y, Color.Green);
                    imageDebug.Save(debugImagePath);
                }

                pBackOnEarthWay1 = borderWalkingPoints.BorderWalkingPoints[pBackOnEarthWay1.SPlus1];
                pBackOnEarthWay2 = borderWalkingPoints.BorderWalkingPoints[pBackOnEarthWay2.SMinus1];
            }

            //apres un pas en avant on repere où on est
            if (pForthOnIsOnLine)
            {
                if (!IsPointOnLine(pForthOnLine, line, listFoundForth, image, pForth_CountBeforeTellingItIsALine))
                {
                    pForthOnIsOnLine = false;
                    pForth_CountBeforeTellingItIsALine = CountBeforeTellingItIsALine;
                    pForthOnEarthWay1 = borderWalkingPoints.GetClosest(pForthOnLine);
                    pForthOnEarthWay2 = borderWalkingPoints.GetClosest(pForthOnLine);
                    listFoundForth.AddRange(listForthOnLine);
                    listForthOnEarthWay1 = new List<BorderWalkingPoint>();
                    listForthOnEarthWay2 = new List<BorderWalkingPoint>();

                }
            }
            else
            {
                if (IsPointOnLine(pForthOnEarthWay1, line, listFoundForth, image, pForth_CountBeforeTellingItIsALine))
                {
                    pForthOnIsOnLine = true;
                    pForthOnLine = line.GetClosest(pForthOnEarthWay1);
                    listFoundForth.AddRange(listForthOnEarthWay1);
                    listForthOnLine = new List<BorderWalkingPoint>();
                }
                else if (IsPointOnLine(pForthOnEarthWay2, line, listFoundForth, image, pForth_CountBeforeTellingItIsALine))
                {
                    pForthOnIsOnLine = true;
                    pForthOnLine = line.GetClosest(pForthOnEarthWay2);
                    listFoundForth.AddRange(listForthOnEarthWay2);
                    listForthOnLine = new List<BorderWalkingPoint>();
                }
                pForth_CountBeforeTellingItIsALine--;
            }
            if (pBackOnIsOnLine)
            {

                if (!IsPointOnLine(pBackOnLine, line, listFoundBack, image, pBack_CountBeforeTellingItIsALine))
                {
                    pBackOnIsOnLine = false;
                    pBack_CountBeforeTellingItIsALine = CountBeforeTellingItIsALine;
                    pBackOnEarthWay1 = borderWalkingPoints.GetClosest(pBackOnLine);
                    pBackOnEarthWay2 = borderWalkingPoints.GetClosest(pBackOnLine);
                    listFoundBack.AddRange(listBackOnLine);
                    listBackOnEarthWay1 = new List<BorderWalkingPoint>();
                    listBackOnEarthWay2 = new List<BorderWalkingPoint>();
                }
            }
            else
            {
                if (IsPointOnLine(pBackOnEarthWay1, line, listFoundBack, image, pBack_CountBeforeTellingItIsALine))
                {
                    pBackOnIsOnLine = true;
                    pBackOnLine = line.GetClosest(pBackOnEarthWay1);
                    listFoundBack.AddRange(listBackOnEarthWay1);
                    listBackOnLine = new List<BorderWalkingPoint>();
                }
                else if (IsPointOnLine(pBackOnEarthWay2, line, listFoundBack, image, pBack_CountBeforeTellingItIsALine))
                {
                    pBackOnIsOnLine = true;
                    pBackOnLine = line.GetClosest(pBackOnEarthWay2);
                    listFoundBack.AddRange(listBackOnEarthWay2);
                    listBackOnLine = new List<BorderWalkingPoint>();
                }
                pBack_CountBeforeTellingItIsALine--;
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

    private bool IsPointOnEarth(BorderWalkingPoint point, Bitmap image)
    {
        var isPointInSea = IsPointInSea(point, image);
        //var isBorderPoint = IsBorderPoint(point, image);
        return !isPointInSea;
    }

    private bool IsPointInSea(BorderWalkingPoint point, Bitmap image)
    {
        return image.GetPixel(point.X, point.Y).ToArgb() == BorderWalkingPointExtractor.SeaColor.ToArgb();
    }

    private bool IsPointOnLine(BorderWalkingPoint point, BorderPointCollection line,
        IEnumerable<BorderWalkingPoint> foundPoints, Bitmap image, int countBeforeTellingItIsALine)
    {
        if (foundPoints
            .Any(m => m.Equals(point)
                || MapUtils.GetDistance(m.Point, point.Point) <= NbPixelsPointsEqual))
        {
            return false;
        }
        var pointOnEarth = IsPointOnEarth(point, image);
        var pointOnLine = line.GetPoints().Any(p => MapUtils.GetDistance(point.Point, p.Point) <= NbPixelsPointsEqual);
        var pointOnBorder = IsBorderPoint(point, image);
        var isOkayCountBeforeTellingItIsALine = countBeforeTellingItIsALine <= 0;
        return pointOnLine && (!pointOnEarth || pointOnBorder) && isOkayCountBeforeTellingItIsALine;
    }


    private bool IsBorderPoint(BorderWalkingPoint pCurrent, Bitmap imageSource)
    {
        var neighBourPoints = new List<MapPoint>();
        AddNeighbourPixels(pCurrent.Point, imageSource, neighBourPoints, 1, NbPixelsNeighBours);
        var currentIsCloseToSea = neighBourPoints
            .Any(p => imageSource.GetPixel(p.X, p.Y).ToArgb() == BorderWalkingPointExtractor.SeaColor.ToArgb());

        return currentIsCloseToSea;
    }


    private void AddNeighbourPixels(MapPoint p, Bitmap image, List<MapPoint> neighBourPoints, int depth, int maxDepth)
    {
        if(depth> maxDepth) { return; }
        //cross pixels
        var p1 = new MapPoint(p.X - 1, p.Y + 1);
        if(!neighBourPoints.Any(p => p1.X == p.X && p1.Y == p.Y) && IsValidPoint(p1, image)) { 
            neighBourPoints.Add(p1); 
        }
        var p2 = new MapPoint(p.X - 1, p.Y - 1);
        if (!neighBourPoints.Any(p => p2.X == p.X && p2.Y == p.Y) && IsValidPoint(p2, image)) {
            neighBourPoints.Add(p2); 
        }
        var p3 = new MapPoint(p.X + 1, p.Y + 1);
        if (!neighBourPoints.Any(p => p3.X == p.X && p3.Y == p.Y) && IsValidPoint(p3, image)) {
            neighBourPoints.Add(p3);
        }
        var p4 = new MapPoint(p.X + 1, p.Y - 1);
        if (!neighBourPoints.Any(p => p4.X == p.X && p4.Y == p.Y) && IsValidPoint(p4, image)) { 
            neighBourPoints.Add(p4); 
        }

        //aside pixels
        var p5 = new MapPoint(p.X, p.Y + 1);
        if (!neighBourPoints.Any(p => p5.X == p.X && p5.Y == p.Y) && IsValidPoint(p5, image)) { 
            neighBourPoints.Add(p5);
        }
        var p6 = new MapPoint(p.X, p.Y - 1);
        if (!neighBourPoints.Any(p => p6.X == p.X && p6.Y == p.Y) && IsValidPoint(p6, image)) {
            neighBourPoints.Add(p6);
        }
        var p7 = new MapPoint(p.X + 1, p.Y);
        if (!neighBourPoints.Any(p => p7.X == p.X && p7.Y == p.Y) && IsValidPoint(p7, image)) { 
            neighBourPoints.Add(p7); 
        }
        var p8 = new MapPoint(p.X - 1, p.Y);
        if (!neighBourPoints.Any(p => p8.X == p.X && p8.Y == p.Y) && IsValidPoint(p8, image)) { 
            neighBourPoints.Add(p8); 
        }

        AddNeighbourPixels(p1, image, neighBourPoints, depth + 1, maxDepth);
        AddNeighbourPixels(p2, image, neighBourPoints, depth + 1, maxDepth);
        AddNeighbourPixels(p3, image, neighBourPoints, depth + 1, maxDepth);
        AddNeighbourPixels(p4, image, neighBourPoints, depth + 1, maxDepth);
        AddNeighbourPixels(p5, image, neighBourPoints, depth + 1, maxDepth);
        AddNeighbourPixels(p6, image, neighBourPoints, depth + 1, maxDepth);
        AddNeighbourPixels(p7, image, neighBourPoints, depth + 1, maxDepth);
        AddNeighbourPixels(p8, image, neighBourPoints, depth + 1, maxDepth);
    }

    private bool IsValidPoint(MapPoint point, Bitmap image)
    {
        return point.X < image.Width
            && point.X >= 0
            && point.Y < image.Height
            && point.Y >= 0;
    }

}
