using ExtractPixels.MapProcessing.Model;
using ExtractPixels.MapProcessing.MultiThreading;
using ExtractPixels.MapProcessing.MultiThreading.Business;
using System.ComponentModel.DataAnnotations;
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
    /// buggué, acces concurrenciel
    /// </summary>
    private const bool UseMultiThreading = false;

    /// <summary>
    /// [S sart, S end, SeaTrip]
    /// </summary>
    public Dictionary<int, Dictionary<int, SeaTrip>> SeaTrips;

    public TripGenerator()
    {
        SeaTrips = new Dictionary<int, Dictionary<int, SeaTrip>>();
    }

    public void CalculateAllTrips(WalkingPointCollection borderWalkingPoints,
        decimal width, decimal height, string imageFilePath, Bitmap image, List<PortOnBorder> ports)
    {
        if (UseMultiThreading)
        {
            Console.WriteLine($"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} : Preparing multithreading computing for generation of trips ...");
        }
        else
        {
            Console.WriteLine($"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} : Starting generation of trips...");
        }


        var scheduler = new WorkScheduler<MyData, MyWorker>();
        var myDatas = new List<MyData>();

        var filteredBorderWalkingPoints = borderWalkingPoints.WalkingPoints.Where(m => ports == null || ports.Any(p => p.WalkingPoint.S == m.Value.S)); ;
        MaxCount = filteredBorderWalkingPoints.Count() * filteredBorderWalkingPoints.Count();
        ComputationCount = 0;

        foreach (var sStart in filteredBorderWalkingPoints)
        {
            var fromStartDestinations = new Dictionary<int, SeaTrip>();
            SeaTrips.Add(sStart.Key, fromStartDestinations);

            foreach (var sEnd in filteredBorderWalkingPoints)
            {
                if (sStart.Key == sEnd.Key) { continue; }

                if (UseMultiThreading)
                {
                    var mydata = new MyData(sStart.Key, sEnd.Key, borderWalkingPoints, width, height, imageFilePath, image);
                    myDatas.Add(mydata);
                }
                else
                {
                    //this code is moved into the scheduler for multithreading
                    var seaTrip = CalculateSingleTrip(sStart.Key, sEnd.Key, borderWalkingPoints, width, height, imageFilePath, image);
                    fromStartDestinations.Add(sEnd.Key, seaTrip);
                    ComputationCount++;
                }

                var rest = (int)(ComputationCount % (MaxCount * 0.1M));
                if (rest == 0 || rest == (MaxCount * 0.1M))
                {
                    Console.WriteLine($"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} : DONE {(int)(100 * (((decimal)ComputationCount / (decimal)MaxCount)))}%");
                }

            }
        }

        if (UseMultiThreading)
        {
            Console.WriteLine($"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} : Starting generation of trips...");
            ComputationCount = 0;
            scheduler.Run(myDatas);
            foreach (var myData in myDatas)
            {
                SeaTrips[myData.sStart].Add(myData.sEnd, myData.SeaTrip);
            }
        }
    }

    public SeaTrip CalculateSingleTrip(int sStart, int sEnd, WalkingPointCollection borderWalkingPoints,
        decimal width, decimal height, string imageFilePath, Bitmap image)
    {
        var pStartOnEarth = borderWalkingPoints.WalkingPoints[sStart];
        var pEndOnEarth = borderWalkingPoints.WalkingPoints[sEnd];
        int sLine = 1;
        var line = MapUtils.GetLine(pStartOnEarth.Point, pEndOnEarth.Point, width, height, 0, ref sLine);
        string debugImagePath = imageFilePath + "_debug.png";
        var imageDebug = MapUtils.Clone(image);

        //back and forth : d'un côté à l'autre
        WalkingPoint pForthOnLine = line.GetClosest(pStartOnEarth);
        WalkingPoint pForthOnEarthWay1 = pStartOnEarth;
        WalkingPoint pForthOnEarthWay2 = pStartOnEarth;
        WalkingPoint pBackOnLine = line.GetClosest(pStartOnEarth);
        WalkingPoint pBackOnEarthWay1 = pStartOnEarth;
        WalkingPoint pBackOnEarthWay2 = pStartOnEarth;
        bool pForthOnIsOnLine = true;
        bool pBackOnIsOnLine = true;
        var listFoundForth = new List<WalkingPoint>();
        var listFoundBack = new List<WalkingPoint>();
        var listForthOnLine = new List<WalkingPoint>();
        var listForthOnEarthWay1 = new List<WalkingPoint>();
        var listForthOnEarthWay2 = new List<WalkingPoint>();
        var listBackOnLine = new List<WalkingPoint>();
        var listBackOnEarthWay1 = new List<WalkingPoint>();
        var listBackOnEarthWay2 = new List<WalkingPoint>();
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
                if (Constants.IsDebugImage)
                {
                    imageDebug.SetPixel(pForthOnLine.X, pForthOnLine.Y, Color.Pink);
                    imageDebug.Save(debugImagePath);
                }

                pForthOnLine = line.WalkingPoints[pForthOnLine.SPlus1];
            }
            else
            {
                if (Constants.IsDebugImage)
                {
                    imageDebug.SetPixel(pForthOnEarthWay1.X, pForthOnEarthWay1.Y, Color.Red);
                    imageDebug.SetPixel(pForthOnEarthWay2.X, pForthOnEarthWay2.Y, Color.Green);
                    imageDebug.Save(debugImagePath);
                }

                pForthOnEarthWay1 = borderWalkingPoints.WalkingPoints[pForthOnEarthWay1.SPlus1];
                pForthOnEarthWay2 = borderWalkingPoints.WalkingPoints[pForthOnEarthWay2.SMinus1];
            }
            if (pBackOnIsOnLine)
            {
                if (Constants.IsDebugImage)
                {
                    imageDebug.SetPixel(pForthOnLine.X, pForthOnLine.Y, Color.Violet);
                    imageDebug.Save(debugImagePath);
                }

                pBackOnLine = line.WalkingPoints[pBackOnLine.SMinus1];
            }
            else
            {
                if (Constants.IsDebugImage)
                {
                    imageDebug.SetPixel(pForthOnEarthWay1.X, pForthOnEarthWay1.Y, Color.Red);
                    imageDebug.SetPixel(pForthOnEarthWay2.X, pForthOnEarthWay2.Y, Color.Green);
                    imageDebug.Save(debugImagePath);
                }

                pBackOnEarthWay1 = borderWalkingPoints.WalkingPoints[pBackOnEarthWay1.SPlus1];
                pBackOnEarthWay2 = borderWalkingPoints.WalkingPoints[pBackOnEarthWay2.SMinus1];
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
                    listForthOnEarthWay1 = new List<WalkingPoint>();
                    listForthOnEarthWay2 = new List<WalkingPoint>();

                }
            }
            else
            {
                if (IsPointOnLine(pForthOnEarthWay1, line, listFoundForth, image, pForth_CountBeforeTellingItIsALine))
                {
                    pForthOnIsOnLine = true;
                    pForthOnLine = line.GetClosest(pForthOnEarthWay1);
                    listFoundForth.AddRange(listForthOnEarthWay1);
                    listForthOnLine = new List<WalkingPoint>();
                }
                else if (IsPointOnLine(pForthOnEarthWay2, line, listFoundForth, image, pForth_CountBeforeTellingItIsALine))
                {
                    pForthOnIsOnLine = true;
                    pForthOnLine = line.GetClosest(pForthOnEarthWay2);
                    listFoundForth.AddRange(listForthOnEarthWay2);
                    listForthOnLine = new List<WalkingPoint>();
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
                    listBackOnEarthWay1 = new List<WalkingPoint>();
                    listBackOnEarthWay2 = new List<WalkingPoint>();
                }
            }
            else
            {
                if (IsPointOnLine(pBackOnEarthWay1, line, listFoundBack, image, pBack_CountBeforeTellingItIsALine))
                {
                    pBackOnIsOnLine = true;
                    pBackOnLine = line.GetClosest(pBackOnEarthWay1);
                    listFoundBack.AddRange(listBackOnEarthWay1);
                    listBackOnLine = new List<WalkingPoint>();
                }
                else if (IsPointOnLine(pBackOnEarthWay2, line, listFoundBack, image, pBack_CountBeforeTellingItIsALine))
                {
                    pBackOnIsOnLine = true;
                    pBackOnLine = line.GetClosest(pBackOnEarthWay2);
                    listFoundBack.AddRange(listBackOnEarthWay2);
                    listBackOnLine = new List<WalkingPoint>();
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

    private bool IsPointOnEarth(WalkingPoint point, Bitmap image)
    {
        var isPointInSea = IsPointInSea(point, image);
        //var isBorderPoint = IsBorderPoint(point, image);
        return !isPointInSea;
    }

    private bool IsPointInSea(WalkingPoint point, Bitmap image)
    {
        return image.GetPixel(point.X, point.Y).ToArgb() == BorderWalkingPointExtractor.SeaColor.ToArgb();
    }

    private bool IsPointOnLine(WalkingPoint point, WalkingPointCollection line,
        IEnumerable<WalkingPoint> foundPoints, Bitmap image, int countBeforeTellingItIsALine)
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


    private bool IsBorderPoint(WalkingPoint pCurrent, Bitmap imageSource)
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
