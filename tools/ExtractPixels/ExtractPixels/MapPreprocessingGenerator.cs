using ExtractPixels.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ExtractPixels;

public class MapPreprocessingGenerator : IPixelHandler
{
    /// <summary>
    /// [S, BorderWalkingPoint]
    /// </summary>
    private Dictionary<int, BorderWalkingPoint> _borderWalkingPoints;

    /// <summary>
    /// [X, [Y, BorderWalkingPoint]]
    /// </summary>
    private Dictionary<int, Dictionary<int, BorderWalkingPoint>> _borderWalkingPointsFromXAndY;

    private BorderWalkingPoint _firstBorderWalkingPoint = null;
    private BorderWalkingPoint _previousBorderWalkingPoint = null;

    public MapPreprocessingGenerator()
    {
        _borderWalkingPoints = new Dictionary<int, BorderWalkingPoint>();
        _borderWalkingPointsFromXAndY = new Dictionary<int, Dictionary<int, BorderWalkingPoint>>();
    }

    /// <summary>
    /// génère une variable s qui permet de parcourit un continent donné
    /// </summary>
    public void AddPixel(int x, int y, int continentNumber, ref int s)
    {
        if (_borderWalkingPointsFromXAndY.ContainsKey(x) && _borderWalkingPointsFromXAndY[x].ContainsKey(y)) { return; }

        var borderWalkingPoint = new BorderWalkingPoint(s, x, y, continentNumber);
        _borderWalkingPoints.Add(s, borderWalkingPoint);
        Dictionary<int, BorderWalkingPoint> borderWalkingPointsFromY = _borderWalkingPointsFromXAndY.ContainsKey(x) ? _borderWalkingPointsFromXAndY[x] : null;
        if (borderWalkingPointsFromY == null)
        {
            borderWalkingPointsFromY = new Dictionary<int, BorderWalkingPoint>();
            _borderWalkingPointsFromXAndY.Add(x, borderWalkingPointsFromY);
        }
        borderWalkingPointsFromY.Add(y, borderWalkingPoint);

        if (_previousBorderWalkingPoint != null)
        {
            _previousBorderWalkingPoint.SPlus1 = borderWalkingPoint.S;
            borderWalkingPoint.SMinus1 = _previousBorderWalkingPoint.S;
        }

        LinkFirstAndLastOfContinent(borderWalkingPoint);
        _previousBorderWalkingPoint = borderWalkingPoint;
        s++;
    }

    /// <summary>
    /// Link the first borderWalkingPoint du continent avec le dernier en utilisant les variables SPlus1 et SMinus1
    /// </summary>
    /// <param name="borderWalkingPoint"></param>
    public void LinkFirstAndLastOfContinent(BorderWalkingPoint borderWalkingPoint)
    {

        if (_previousBorderWalkingPoint != null && borderWalkingPoint != null
            && borderWalkingPoint.ContinentNumber != _previousBorderWalkingPoint.ContinentNumber)
        {
            _firstBorderWalkingPoint.SMinus1 = _previousBorderWalkingPoint.S;
            _previousBorderWalkingPoint.SPlus1 = _firstBorderWalkingPoint.S;
        }


        //setup _firstBorderWalkingPoint
        if (_firstBorderWalkingPoint == null)
        {
            _firstBorderWalkingPoint = borderWalkingPoint;
        }
        if (_previousBorderWalkingPoint == null
            || (borderWalkingPoint != null
                && _previousBorderWalkingPoint.ContinentNumber != borderWalkingPoint.ContinentNumber))
        {
            _firstBorderWalkingPoint = borderWalkingPoint;
        }

        if (_previousBorderWalkingPoint == null)
        {
            return;
        }

        if (borderWalkingPoint == null)
        {
            _firstBorderWalkingPoint.SMinus1 = _previousBorderWalkingPoint.S;
            _previousBorderWalkingPoint.SPlus1 = _firstBorderWalkingPoint.S;
        }
    }


    public List<List<Tuple<int, int>>> GetLineForthAndBack(int x1, int y1, int x2, int y2, int width, int height)
    {
        var lines = new List<List<Tuple<int, int>>>();
        var line = GetLineDirect(x1, y1, x2, y2);
        lines.Add(line);
        line = GetLineIndirect(x1, y1, x2, y2, width, height);
        if (line.Any())
        {
            lines.Add(line);
        }
        return lines;
    }

    public List<Tuple<int, int>> GetLineDirect(decimal x1, decimal y1, decimal x2, decimal y2)
    {
        var resultPixels = new List<Tuple<int, int>>();

        if (x1 > x2)
        {
            var xTmp = x2;
            var yTmp = y2;
            x2 = x1;
            y2 = y1;
            x1 = xTmp;
            y1 = yTmp;
        }

        var a = (y1 - y2) / (x1 - x2);
        var b = y1 - (a * x1);

        decimal previousY = -1;
        for (var x = x1; x <= x2; x++)
        {
            var y = (a * x) + b;
            resultPixels.Add(new Tuple<int, int>((int)x, (int)y));

            if (x != x1)
            {
                if (y > previousY)
                {
                    for (decimal yTmp = previousY; yTmp <= y; yTmp++)
                    {
                        resultPixels.Add(new Tuple<int, int>((int)x, (int)yTmp));
                    }
                }
                else
                {
                    for (decimal yTmp = y; yTmp <= previousY; yTmp++)
                    {
                        resultPixels.Add(new Tuple<int, int>((int)x, (int)yTmp));
                    }
                }
            }
            resultPixels.Add(new Tuple<int, int>((int)x, (int)y));
            previousY = y;
        }

        return resultPixels;
    }

    public List<Tuple<int, int>> GetLineIndirect(decimal x1, decimal y1, decimal x2, decimal y2, decimal width, decimal height)
    {
        var resultPixels = new List<Tuple<int, int>>();
        if (y1 == y2) { return resultPixels; }

        if (x1 > x2)
        {
            var xTmp = x2;
            var yTmp = y2;
            x2 = x1;
            y2 = y1;
            x1 = xTmp;
            y1 = yTmp;
        }

        decimal previousY = y1;
        y1 = height - 1 - y1;
        y2 = height - 1 - y2;
        var heightInPixels = 1;

        var a = (y2 - y1) / (x2 - x1);//y=a*x+b; (y-b)=a*x; x=(y-b)/a;
        var b = y1 - (a * x1);

        var xMinNegativeGradient = (height -1 - b) / a;//pente diminue y qd x diminue (a<0), x<xMin => y<0,
                                          //y passe a height -1, x passe a (height - 1-b)/a
        xMinNegativeGradient = xMinNegativeGradient <= 0 ? 1 : xMinNegativeGradient;
        xMinNegativeGradient = xMinNegativeGradient >= width ? (width - 1) : xMinNegativeGradient;

        var xMinPositiveGradient = (heightInPixels - b) / a; //pente diminue y qd x diminue (a>0), x<xMin => y>=height,  
                                                         //y passe a 0, x passe a (1 -b)/ a
        xMinPositiveGradient = xMinPositiveGradient <= 0 ? 1 : xMinPositiveGradient;
        xMinPositiveGradient = xMinPositiveGradient >= width ? (width - 1) : xMinPositiveGradient;

        decimal x = x1 - 1;
        decimal y = y2;
        var inversionDone = false;
        while (!inversionDone || x > x2)
        {
            y = (a * x) + b;
            y = height - 1 - y;
            y = y <= 0 ? 1 : y;
            var inversionDoneInThisIteration = false;

            if (a > 0)
            {
                if (x < xMinPositiveGradient)
                {
                    inversionDone = true;
                    inversionDoneInThisIteration = true;

                    var yBeforeInversion = height - 1 - ((a * x) + b);

                    y = 1;

                    if(x > xMinPositiveGradient || yBeforeInversion <= 1)
                    {
                        if (y > previousY)
                        {
                            for (decimal yTmp = previousY; yTmp <= y; yTmp++)
                            {
                                resultPixels.Add(new Tuple<int, int>((int)x, (int)yTmp));
                            }
                        }
                        else
                        {
                            for (decimal yTmp = y; yTmp <= previousY; yTmp++)
                            {
                                resultPixels.Add(new Tuple<int, int>((int)x, (int)yTmp));
                            }
                        }
                    }

                    x = (1 - b) / a;
                    if(x < 0)
                    {
                        x = width - 1;
                    }

                    var yInversion = (a * x) + b;
                    y = height - 1 - yInversion;
                    if(y < 0)
                    {
                        x = (height - b)/a;//x=(y-b)/a
                        y = 1;
                        yInversion = y;
                    }
                    yInversion = yInversion <= 0 ? 1 : yInversion;
                    previousY = yInversion;
                }
            }
            else
            {
                if (x < xMinNegativeGradient)
                {
                    inversionDone = true;
                    inversionDoneInThisIteration = true;

                    var yBeforeInversion = height - 1 - ((a * x) + b);
                    y = heightInPixels - 1;
                    y = y <= 0 ? 1 : y;

                    if(x > xMinNegativeGradient || yBeforeInversion <= 1)
                    {
                        if (y > previousY)
                        {
                            for (decimal yTmp = previousY; yTmp <= y; yTmp++)
                            {
                                resultPixels.Add(new Tuple<int, int>((int)x, (int)yTmp));
                            }
                        }
                        else
                        {
                            for (decimal yTmp = y; yTmp <= previousY; yTmp++)
                            {
                                resultPixels.Add(new Tuple<int, int>((int)x, (int)yTmp));
                            }
                        }
                    }

                    x = (heightInPixels - 1 - b) / a;
                    if (x < 0)
                    {
                        x = width - 1;
                    }

                    var yInversion = (a * x) + b;
                    y = height - 1 - yInversion;
                    if (y < 0)
                    {
                        x = (height - b) / a;//x=(y-b)/a
                        y = 1;
                        yInversion = y;
                    }
                    yInversion = yInversion <= 0 ? 1 : yInversion;
                    previousY = yInversion;

                }
            }

            if (!inversionDoneInThisIteration)
            {
                if (y > previousY)
                {
                    for (decimal yTmp = previousY; yTmp <= y; yTmp++)
                    {
                        resultPixels.Add(new Tuple<int, int>((int)x, (int)yTmp));
                    }
                }
                else
                {
                    for (decimal yTmp = y; yTmp <= previousY; yTmp++)
                    {
                        resultPixels.Add(new Tuple<int, int>((int)x, (int)yTmp));
                    }
                }
            }


            resultPixels.Add(new Tuple<int, int>((int)x, (int)y));
            previousY = y;
            x--;
        }

        previousY = height - 1 - y2;
        if (y > previousY)
        {
            for (decimal yTmp = previousY; yTmp <= y; yTmp++)
            {
                resultPixels.Add(new Tuple<int, int>((int)x, (int)yTmp));
            }
        }
        else
        {
            for (decimal yTmp = y; yTmp <= previousY; yTmp++)
            {
                resultPixels.Add(new Tuple<int, int>((int)x, (int)yTmp));
            }
        }


        return resultPixels;
    }


    public void GenerateJourneys()
    {
        foreach (var sStart in _borderWalkingPoints)
        {
            foreach (var sEnd in _borderWalkingPoints)
            {

            }

        }
    }

    public void Dump(string debugDumpPath)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Continent;S;SPlus1;SMinus1");
        foreach (var p in _borderWalkingPoints.Values)
        {
            sb.AppendLine($"{p.ContinentNumber};{p.S};{p.SPlus1};{p.SMinus1}");
        }
        File.WriteAllText(debugDumpPath, sb.ToString());
    }

    public void DrawLine(int x1, int y1, int x2, int y2, int width, int height, string imageFilePath)
    {
        var lines = GetLineForthAndBack(x1, y1, x2, y2, width, height);
        Bitmap outputMap;
        using (var image = new Bitmap(System.Drawing.Image.FromFile(imageFilePath)))
        {
            width = image.Width;
            height = image.Height;
            outputMap = image.Clone() as Bitmap;

            var firstLine = true;
            foreach (var line in lines)
            {
                foreach (var pixel in line)
                {
                    outputMap.SetPixel(pixel.Item1, pixel.Item2, firstLine ? Color.Blue : Color.Green);
                }
                firstLine = false;
            }
            outputMap.Save(imageFilePath + "_lines.png");

        }
    }
}
