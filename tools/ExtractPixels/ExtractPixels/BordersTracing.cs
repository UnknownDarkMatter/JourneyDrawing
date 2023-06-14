﻿using System.Drawing;
using System.Globalization;
using System.Text;

namespace ExtractPixels;

public class BordersTracing
{
    public const int MaxDepth = 5;
    public static Color BorderColor = Color.Blue;
    public const int IgnoreWhenNumberOfAside = 2;
    private enum PixelPosition
    {
        Both = 0,
        Cross = 1,
        Aside = 2
    }
    private IEnumerable<IPixelHandler> _pixelHandlers;
    public BordersTracing(IEnumerable<IPixelHandler> pixelHandlers)
    {
        _pixelHandlers = pixelHandlers;
    }

    public void DoTraceBorders(int xStart, int yStart, Bitmap outputMap, int continentNumber, ref int s)
    {
        string inputPath = Path.Combine(Environment.CurrentDirectory, "image.png");
        string outputPath = Path.Combine(Environment.CurrentDirectory, "image_borders.png");
        using (var image = new Bitmap(Image.FromFile(inputPath)))
        {
            int width = image.Width;
            int height = image.Height;

            int x1 = xStart;
            int y1 = yStart;
            var foundPixels = new List<Tuple<int, int>>();
            image.SetPixel(x1, y1, BorderColor);
            ProcessPixel(x1, y1, continentNumber, ref s);

            while (TryGetNextBorderPixel(xStart, yStart, x1, y1, foundPixels, image, out int x2, out int y2) > 0)
            {
                image.SetPixel(x2, y2, BorderColor);
                image.Save(outputPath);

                outputMap.SetPixel(x2, y2, Color.Black);

                ProcessPixel(x1, y1, continentNumber, ref s);

                foundPixels.Add(new Tuple<int, int>(x2, y2));
                x1 = x2;
                y1 = y2;
            }
        }
    }

    private void ProcessPixel(int x, int y, int continentNumber, ref int s)
    {
        foreach(var handler in _pixelHandlers)
        {
            handler.AddPixel(x, y, continentNumber, ref s);
        }
    }

    private static bool HasEmptyPixelsInNeighbourhood(int x, int y, Bitmap image)
    {
        var pixels = GetNeighbourPixels(x, y, image, PixelPosition.Both);
        var neighBourPixel = pixels
            .FirstOrDefault((tuple) => image.GetPixel(tuple.Item1, tuple.Item2).Name == "0"
            || image.GetPixel(tuple.Item1, tuple.Item2).Name == "ffffffff"
            );
        return neighBourPixel != null;
    }

    private static List<Tuple<int, int>> GetNeighbourPixels(int x, int y, Bitmap image, PixelPosition pixelPosition)
    {
        var result = new List<Tuple<int, int>>();
        if (pixelPosition == PixelPosition.Both || pixelPosition == PixelPosition.Cross)
        {
            result.Add(new Tuple<int, int>(x - 1, y + 1));
            result.Add(new Tuple<int, int>(x - 1, y - 1));
            result.Add(new Tuple<int, int>(x + 1, y + 1));
            result.Add(new Tuple<int, int>(x + 1, y - 1));
        }
        if (pixelPosition == PixelPosition.Both || pixelPosition == PixelPosition.Aside)
        {
            result.Add(new Tuple<int, int>(x, y + 1));
            result.Add(new Tuple<int, int>(x, y - 1));
            result.Add(new Tuple<int, int>(x + 1, y));
            result.Add(new Tuple<int, int>(x - 1, y));
        }
        result = result.Where((tuple) => 
            tuple.Item1 < image.Width 
            && tuple.Item1 >= 0
            && tuple.Item2 < image.Height
            && tuple.Item2 >= 0
        ).ToList();
        return result;
    }

    private static int TryGetNextBorderPixel(int x0, int y0, int x1, int y1, List<Tuple<int, int>> foundPixels,
        Bitmap image, out int x2, out int y2)
    {
        int depth = 0;
        int maxDepth = MaxDepth;
        int x1ToTest = 0;
        int y1ToTest = 0;

        var count = TryGetNextBorderPixelRecursive(x0, y0, x1, y1, ref x1ToTest, ref y1ToTest, foundPixels, image, depth, 
            maxDepth, out x2, out y2, out int foundDepth);
        x2 = x1ToTest;
        y2 = y1ToTest;
        return count;
    }

    private static int TryGetNextBorderPixelRecursive(int x0, int y0, int x1, int y1, ref int x1ToTest, ref int y1ToTest, 
        List<Tuple<int, int>> foundPixels,
        Bitmap image, int depth, int maxDepth, out int x2, out int y2, out int foundDepth)
    {
        if (depth > maxDepth)
        {
            x2 = x1ToTest;
            y2 = y1ToTest;
            foundDepth = depth;
            return 1;
        }

        if(depth == 1)
        {
            x1ToTest = x1;
            y1ToTest = y1;
        }

        var foundPixelsTmp = foundPixels.Select(m=>m).ToList();
        var tuples = TryGetNextBorderPixelNonRecursive(x0 ,x0, x1, y1, foundPixelsTmp, image, out int x2Tmp, out int y2Tmp);
        if (!tuples.Any())
        {
            x2 = 0;
            y2 = 0;
            foundDepth = depth;
            return 0;
        }
        
        var childResults = new List<Tuple<int, int, int, int, int>>();
        foreach (var childTuple in tuples)
        {
            int count = TryGetNextBorderPixelRecursive(x0, y0, childTuple.Item1, childTuple.Item2, ref x1ToTest, ref y1ToTest,
                foundPixelsTmp, image,
                depth + 1, maxDepth, out int x2Child, out int y2Child, out int childFoundDepth);
            childResults.Add(new Tuple<int, int, int, int, int>(x2Child, y2Child, childFoundDepth, x1ToTest, y1ToTest));
        }

        var bestChild = childResults.OrderByDescending(t => t.Item3).First();
        x2 = bestChild.Item1;
        y2 = bestChild.Item2;
        foundDepth = bestChild.Item3;
        if (foundDepth >= maxDepth)
        {
            x1ToTest = bestChild.Item4;
            y1ToTest = bestChild.Item5;
        }
        return 1;
    }

    private static List<Tuple<int, int>> TryGetNextBorderPixelNonRecursive(int x0, int y0, int x1, int y1, List<Tuple<int, int>> foundPixels, 
        Bitmap image, out int x2, out int y2)
    {
        var neighBourPixel = GetNeighbourPixels(x1, y1, image, PixelPosition.Both)
            .Where((t) => 
                HasEmptyPixelsInNeighbourhood(t.Item1, t.Item2, image)
                && image.GetPixel(t.Item1, t.Item2).Name != "0"
                && image.GetPixel(t.Item1, t.Item2).Name != "ffffffff"
                && !foundPixels.Any(t2 => t.Item1 == t2.Item1 && t.Item2 == t2.Item2)
            );

        //A TESTER
        var startBourPixel = new List<Tuple<int, int>>();
        foreach(var t in neighBourPixel)
        {
            var neighBourTmp = GetNeighbourPixels(t.Item1, t.Item2, image, PixelPosition.Both);
            startBourPixel.AddRange(neighBourTmp.Where(t1 => image.GetPixel(t1.Item1, t1.Item2).ToArgb() == BorderColor.ToArgb()));
        }


        if (startBourPixel.Any())
        {
            x2 = neighBourPixel.First().Item1;
            y2 = neighBourPixel.First().Item2;
            return neighBourPixel.ToList();
        }

        var crossNeighBourHoodPixels = neighBourPixel
            .Where(t =>
                        //IgnoreWhenNumberOfAside > GetNeighbourPixels(t.Item1, t.Item2, image, PixelPosition.Aside)
                        //    .Count(t4 => image.GetPixel(t4.Item1, t4.Item2).ToArgb() == BorderColor.ToArgb())
                        //&&
                        GetNeighbourPixels(x1, y1, image, PixelPosition.Cross)
                            .Any(t2 =>
                                HasEmptyPixelsInNeighbourhood(t2.Item1, t2.Item2, image)
                                && image.GetPixel(t2.Item1, t2.Item2).Name != "0"
                                && image.GetPixel(t2.Item1, t2.Item2).Name != "ffffffff"
                                && !foundPixels.Any(t3 => t2.Item1 == t3.Item1 && t2.Item2 == t3.Item2
                                //&& IgnoreWhenNumberOfAside > GetNeighbourPixels(t2.Item1, t2.Item2, image, PixelPosition.Aside)
                                //    .Count(t4 => image.GetPixel(t4.Item1, t4.Item2).ToArgb() == BorderColor.ToArgb())
                                )
                           )
                    ).ToList();

        neighBourPixel = neighBourPixel
            .Where(t =>
                crossNeighBourHoodPixels.Count == 0
                ||
                !foundPixels.Any(t2 =>
                    GetNeighbourPixels(t.Item1, t.Item2, image, PixelPosition.Aside)
                    .Any(t3 => t3.Item1 == t2.Item1 && t3.Item2 == t2.Item2)
                )
            );

        if (neighBourPixel.Count() == 0 
            && 1 == crossNeighBourHoodPixels.Count(t=>
                IgnoreWhenNumberOfAside > GetNeighbourPixels(t.Item1, t.Item2, image, PixelPosition.Aside)
                        .Count(t4 => image.GetPixel(t4.Item1, t4.Item2).ToArgb() == BorderColor.ToArgb())))
        {
            neighBourPixel = crossNeighBourHoodPixels
                .Where(t =>
                    2 == GetNeighbourPixels(t.Item1, t.Item2, image, PixelPosition.Aside)
                        .Count(t2 =>
                            image.GetPixel(t2.Item1, t2.Item2).Name == "0"
                            || image.GetPixel(t2.Item1, t2.Item2).Name == "ffffffff"
                        )
                );
        }

        if (!neighBourPixel.Any()) { 
            x2 = 0; 
            y2 = 0; 
            return new List<Tuple<int, int>>();
        }

        x2 = neighBourPixel.First().Item1;
        y2 = neighBourPixel.First().Item2;
        return neighBourPixel.ToList();
    }
}
