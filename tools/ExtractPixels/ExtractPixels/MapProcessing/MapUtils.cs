using ExtractPixels.MapProcessing.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtractPixels.MapProcessing;

public static class MapUtils
{


    /// <summary>
    /// génère une ligne qui traverse toute la carte et où le dernier point est lié au premier et inversement
    /// </summary>
    /// <returns></returns>
    public static WalkingPointCollection GetLine(MapPoint p1, MapPoint p2, decimal width, decimal height, 
        int continentNumber, ref int s)
    {
        var collection = new WalkingPointCollection();

        if(p1.X == p2.X)
        {
            for (decimal yTmp = 0; yTmp < height - 1; yTmp++)
            {
                collection.AddMapPoint(new MapPoint((int)p1.X, (int)yTmp), continentNumber, ref s);
            }
            collection.LinkFirstAndLastOfContinent(null);
            return collection;
        }

        if (p1.Y == p2.Y)
        {
            for (decimal xTmp2 = 0; xTmp2 < width - 1; xTmp2++)
            {
                collection.AddMapPoint(new MapPoint((int)xTmp2, (int)p1.Y), continentNumber, ref s);
            }
            collection.LinkFirstAndLastOfContinent(null);
            return collection;
        }

        //if (p1.X > p2.X)
        //{
        //    var pTmp = p2.Clone();
        //    p2 = p1;
        //    p1 = pTmp;
        //}

        //equation formula
        var y1 = (decimal)p1.Y;
        var y2 = (decimal)p2.Y;
        //y1 = height - 1 - y1;//y mirror
        //y2 = height - 1 - y2;//y mirror
        var a = (y2 - y1) / ((decimal)p2.X - (decimal)p1.X);//y=a*x+b; (y-b)=a*x; x=(y-b)/a;
        var b = y1 - (a * (decimal)p1.X);

        var debugY =  (p1.X * a + b);//y mirror
        debugY =   (p2.X * a + b);//y mirror

        //get boundaries
        decimal x1 = 0;
        decimal x2 = width - 1;
        LimitXToBoundaries(a, b, width, height, ref x1);
        LimitXToBoundaries(a, b, width, height, ref x2);

        //generate MapPoints beetween boundaries
        decimal previousY = -1;
        decimal previousX = -1;
        decimal y = 0;
        for (var x = x1; x <= x2; x++)
        {
            y = (a * x) + b;
            //y = height - 1 - y;//y mirror
            collection.AddMapPoint(new MapPoint((int)x, (int) y), continentNumber, ref s);

            if (x != x1)
            {
                if (y > previousY)
                {
                    for (decimal yTmp = previousY; yTmp < y; yTmp++)
                    {
                        collection.AddMapPoint(new MapPoint((int)x, (int)yTmp), continentNumber, ref s);
                    }
                }
                else
                {
                    for (decimal yTmp = y; yTmp < previousY; yTmp++)
                    {
                        collection.AddMapPoint(new MapPoint((int)x, (int)yTmp), continentNumber, ref s);
                    }
                }
            }
            previousY = y;
            previousX = x;
        }

        var xTmp = previousX + 1;
        xTmp = xTmp > width - 1 ? width - 1 : xTmp;

        var yProjection = (a * (previousX + 1)) + b;
        //yProjection = (int)(height - 1 - yProjection);//y mirror

        if ((int)y > 0 && (int)xTmp < width -1)
        {
            if ((int)yProjection >= 0)
            {
                for (decimal yTmp = yProjection; yTmp <= height - 1; yTmp++)
                {
                    collection.AddMapPoint(new MapPoint((int)xTmp, (int)yTmp), continentNumber, ref s);
                    previousY = yTmp;
                }
            }
        }

        if (y > 0 && (int)previousY < height-1 && xTmp < width - 1)
        {
            if(yProjection > height - 1)
            {
                for (decimal yTmp = previousY; yTmp < height - 1; yTmp++)
                {
                    collection.AddMapPoint(new MapPoint((int)xTmp, (int)yTmp), continentNumber, ref s);
                }
            }
            else
            {
                for (decimal yTmp = 0; yTmp < y; yTmp++)
                {
                    collection.AddMapPoint(new MapPoint((int)xTmp, (int)yTmp), continentNumber, ref s);
                }
            }
        }

        collection.LinkFirstAndLastOfContinent(null);

        return collection;
    }

    public static decimal GetDistance(MapPoint p1, MapPoint p2)
    {
        var x = Math.Pow((double)p2.X - (double)p1.X, (double)2);
        var y = Math.Pow((double)p2.Y - (double)p1.Y, (double)2);
        return (decimal)Math.Sqrt(x + y);
    }

    public static Bitmap Clone(Bitmap image)
    {
        int width = image.Width;
        int height = image.Height;

        var imageWork = image.Clone() as Bitmap;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                imageWork.SetPixel(x, y, image.GetPixel(x, y));
            }
        }
        return imageWork;
    }

    public static void DrawLine(MapPoint p1, MapPoint p2, int width, int height, string imageFilePath)
    {

        //MapUtils.DrawLine(new MapPoint(26, 219), new MapPoint(189, 304), width, height, imageFilePath);//pente negative, inversion y en biais
        //MapUtils.DrawLine(new MapPoint(77, 94), new MapPoint(80, 333), width, height, imageFilePath);//pente negative vertical
        //MapUtils.DrawLine(new MapPoint(124, 285), new MapPoint(212, 281), width, height, imageFilePath);//pente negative horizontal
        //MapUtils.DrawLine(new MapPoint(43, 330), new MapPoint(118, 232), width, height, imageFilePath);//pente positive, inversion y en biais
        //MapUtils.DrawLine(new MapPoint(98, 340), new MapPoint(103, 264), width, height, imageFilePath);//pente positive vertical
        //MapUtils.DrawLine(new MapPoint(254, 459), new MapPoint(350, 450), width, height, imageFilePath);//pente positive horizontal
        //MapUtils.DrawLine(new MapPoint(141, 778), new MapPoint(290, 710), width, height, imageFilePath);//pente positive diagonale
        //MapUtils.DrawLine(new MapPoint(151, 70), new MapPoint(300, 137), width, height, imageFilePath);//pente negative diagonale
        //MapUtils.DrawLine(new MapPoint(151, 70), new MapPoint(300, 120), width, height, imageFilePath);//pente negative diagonale

        int s = 1;
        var line = GetLine(p1, p2, width, height, 1, ref s);
        Bitmap outputMap;
        using (var image = new Bitmap(System.Drawing.Image.FromFile(imageFilePath)))
        {
            width = image.Width;
            height = image.Height;
            outputMap = image.Clone() as Bitmap;

            foreach (var p in line.GetPoints())
            {
                outputMap.SetPixel(p.X, p.Y, Color.Blue);
            }

            outputMap.Save(imageFilePath + "_lines.png");

        }
    }


    private static void LimitXToBoundaries(decimal a, decimal b, decimal width, decimal height, ref decimal x)
    {
        //y=a*x+b; (y-b)=a*x; x=(y-b)/a;
        var y = (a * x) + b;

        if(y < 0)
        {
            x = -b / a;
        }

        if(y >= height)
        {
            x = (height - 1 - b) / a;
        }
    }
}
