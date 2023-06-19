using ExtractPixels.MapProcessing.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtractPixels.MapProcessing;

/// <summary>
/// Extract the borders of a map where the continent are filled with color.
/// Seas have white color.
/// </summary>
public class BorderWalkingPointExtractor
{

    private IEnumerable<IMapPointHandler> _mapPointHandlers;

    public static Color ImageBorderColor = Color.Black;
    public static Color WorkBorderColor = Color.Blue;
    public static Color SeaColor = Color.White;
    public static Color OutMapBorderColor = Color.FromArgb(119, 255, 118);

    public const int MaxDepth = 5;

    public BorderWalkingPointExtractor(IEnumerable<IMapPointHandler> mapPointHandlers)
    {
        _mapPointHandlers = mapPointHandlers;
    }

    public void ExtractBorder(int xStart, int yStart, int continentNumber, ref int s,
        Bitmap imageSource, Bitmap imageWork, string workFilePath, Bitmap imageOutput, string outputMapPath)
    {
        var pStart = new MapPoint(xStart, yStart);
        var p1 = new MapPoint(xStart, yStart);
        var foundPixels = new List<MapPoint>();
        imageWork.SetPixel(pStart.X, pStart.Y, WorkBorderColor);
        imageWork.Save(workFilePath);

        imageOutput.SetPixel(pStart.X, pStart.Y, OutMapBorderColor);
        imageOutput.Save(outputMapPath);


        ProcessMapPoint(pStart, continentNumber, ref s);

        while (TryGetNextBorderPoint(pStart, p1, foundPixels, imageSource, imageWork, out MapPoint p2) > 0)
        {
            imageWork.SetPixel(p2.X, p2.Y, WorkBorderColor);
            imageWork.Save(workFilePath);

            imageOutput.SetPixel(p2.X, p2.Y, OutMapBorderColor);
            imageOutput.Save(outputMapPath);

            ProcessMapPoint(p1, continentNumber, ref s);

            foundPixels.Add(p2);
            p1 = p2;
        }
    }

    private int TryGetNextBorderPoint(MapPoint pStart, MapPoint p1, List<MapPoint> foundPixels,
        Bitmap imageSource, Bitmap imageWork, out MapPoint p2)
    {
        int depth = 0;
        MapPoint pointToTest = null;

        var count = TryGetNextBorderPointRecursive(pStart, p1, foundPixels, imageSource, imageWork,
            depth, MaxDepth, out p2, out int foundDepth, ref pointToTest);

        p2 = pointToTest;
        return count;
    }

    private int TryGetNextBorderPointRecursive(MapPoint pStart, MapPoint p1, List<MapPoint> foundPixels,
        Bitmap imageSource, Bitmap imageWork, int depth, int maxDepth, 
        out MapPoint p2, out int foundDepth, ref MapPoint pointToTest)
    {
        if (depth > maxDepth)
        {
            p2 = pointToTest;
            foundDepth = depth;
            return 1;
        }
        if (depth == 1)
        {
            pointToTest = p1;
        }
        var foundPixelsTmp = foundPixels.Select(m => m).ToList();
        var points = TryGetNextBorderPointNonRecursive(pStart, p1, foundPixelsTmp, imageSource, imageWork);
        if (!points.Any())
        {
            p2 = null;
            foundDepth = depth;
            return 0;
        }

        //[p2, childFoundDepth, pointToTest]
        var childResults = new List<Tuple<MapPoint, int, MapPoint>>();
        foreach (var childPoint in points)
        {
            TryGetNextBorderPointRecursive(pStart, childPoint, foundPixelsTmp, imageSource, imageWork,
                depth + 1, MaxDepth, out p2, out int childFoundDepth, ref pointToTest);
            childResults.Add(new Tuple<MapPoint, int, MapPoint>(p2, childFoundDepth, pointToTest));
        }

        var bestChild = childResults.OrderByDescending(t => t.Item2).First();
        p2 = bestChild.Item1;
        foundDepth = bestChild.Item2;
        if (foundDepth >= maxDepth)
        {
            pointToTest = bestChild.Item3;
        }
        return 1;
    }

    private IEnumerable<MapPoint> TryGetNextBorderPointNonRecursive(MapPoint pStart, MapPoint p1, List<MapPoint> foundPixels,
        Bitmap imageSource, Bitmap imageWork)
    {
        var nextBorderPoints = GetNeighbourPixels(p1, imageSource);
        nextBorderPoints = nextBorderPoints.Where(p => IsNextBorderPoint(p, p1, foundPixels, imageSource, imageWork));

        if (!nextBorderPoints.Any())
        {
            return new List<MapPoint>();
        }

        return nextBorderPoints;
    }

    public static IEnumerable<MapPoint> GetNeighbourPixels(MapPoint p, Bitmap image)
    {
        var result = new List<MapPoint>();

        //cross pixels
        result.Add(new MapPoint(p.X - 1, p.Y + 1));
        result.Add(new MapPoint(p.X - 1, p.Y - 1));
        result.Add(new MapPoint(p.X + 1, p.Y + 1));
        result.Add(new MapPoint(p.X + 1, p.Y - 1));

        //aside pixels
        result.Add(new MapPoint(p.X, p.Y + 1));
        result.Add(new MapPoint(p.X, p.Y - 1));
        result.Add(new MapPoint(p.X + 1, p.Y));
        result.Add(new MapPoint(p.X - 1, p.Y));

        result = result.Where((point) =>
            point.X < image.Width
            && point.X >= 0
            && point.Y < image.Height
            && point.Y >= 0
        ).ToList();

        return result;
    }

    public bool IsNextBorderPoint(MapPoint pCurrent, MapPoint pLastFound, List<MapPoint> foundPixels, 
        Bitmap imageSource, Bitmap imageWork)
    {
        var color = imageSource.GetPixel(pCurrent.X, pCurrent.Y).ToArgb();
        var currentIsBorder = color != WorkBorderColor.ToArgb() && color != SeaColor.ToArgb();
        var lastFoundIsBorder = imageWork.GetPixel(pLastFound.X, pLastFound.Y).ToArgb() == WorkBorderColor.ToArgb();

        var pointNeighbours = GetNeighbourPixels(pCurrent, imageSource);
        var currentIsCloseToSea = pointNeighbours
            .Any(p => imageSource.GetPixel(p.X, p.Y).ToArgb() == SeaColor.ToArgb());

        var alreadyFound = foundPixels.Any(p => p.X == pCurrent.X && p.Y == pCurrent.Y);

        return currentIsBorder && lastFoundIsBorder && currentIsCloseToSea && !alreadyFound;
    }


    private void ProcessMapPoint(MapPoint point, int continentNumber, ref int s)
    {
        foreach (var handler in _mapPointHandlers)
        {
            handler.AddMapPoint(point, continentNumber, ref s);
        }
    }

}