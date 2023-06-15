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
        if(_borderWalkingPointsFromXAndY.ContainsKey(x) && _borderWalkingPointsFromXAndY[x].ContainsKey(y)) { return; }

        var borderWalkingPoint = new BorderWalkingPoint(s, x, y, continentNumber);
        _borderWalkingPoints.Add(s, borderWalkingPoint);
        Dictionary<int, BorderWalkingPoint> borderWalkingPointsFromY = _borderWalkingPointsFromXAndY.ContainsKey(x) ? _borderWalkingPointsFromXAndY[x] : null;
        if(borderWalkingPointsFromY == null)
        {
            borderWalkingPointsFromY = new Dictionary<int, BorderWalkingPoint>();
            _borderWalkingPointsFromXAndY.Add(x, borderWalkingPointsFromY);
        }
        borderWalkingPointsFromY.Add(y, borderWalkingPoint);

        if(_previousBorderWalkingPoint != null)
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
        if(_previousBorderWalkingPoint == null
            || (borderWalkingPoint != null 
                && _previousBorderWalkingPoint.ContinentNumber != borderWalkingPoint.ContinentNumber))
        {
            _firstBorderWalkingPoint = borderWalkingPoint;
        }

        if(_previousBorderWalkingPoint == null)
        {
            return;
        }

        if (borderWalkingPoint == null)
        {
            _firstBorderWalkingPoint.SMinus1 = _previousBorderWalkingPoint.S;
            _previousBorderWalkingPoint.SPlus1 = _firstBorderWalkingPoint.S;
        } 
    }


    public void Dump(string debugDumpPath)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Continent;S;SPlus1;SMinus1");
        foreach(var p in _borderWalkingPoints.Values)
        {
            sb.AppendLine($"{p.ContinentNumber};{p.S};{p.SPlus1};{p.SMinus1}");
        }
        File.WriteAllText(debugDumpPath, sb.ToString());
    }

    public List<Tuple<int, int>> GetLine(int x1, int y1, int x2, int y2)
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

        int previousY = -1;
        for (var x = x1; x <= x2; x++)
        {
            var y = (a * x) + b;
             resultPixels.Add(new Tuple<int, int>(x, y));

            if (x != x1)
            {
                if(y > previousY)
                {
                    for (int yTmp = previousY; yTmp <= y; yTmp++)
                    {
                        resultPixels.Add(new Tuple<int, int>(x, yTmp));
                    }
                }
                else
                {
                    for (int yTmp = y; yTmp <= previousY; yTmp++)
                    {
                        resultPixels.Add(new Tuple<int, int>(x, yTmp));
                    }
                }
            }
            resultPixels.Add(new Tuple<int, int>(x, y));
            previousY = y;
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


}
