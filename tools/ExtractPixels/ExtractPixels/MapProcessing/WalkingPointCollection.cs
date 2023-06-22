using ExtractPixels.MapProcessing.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtractPixels.MapProcessing;

public class WalkingPointCollection : IMapPointHandler
{    
    /// <summary>
     /// [S, WalkingPoint]
     /// </summary>
    private Dictionary<int, WalkingPoint> _borderWalkingPoints;

    public Dictionary<int, WalkingPoint> WalkingPoints
    {
        get { return _borderWalkingPoints; }
    }


    /// <summary>
    /// [X, [Y, WalkingPoint]]
    /// </summary>
    private Dictionary<int, Dictionary<int, WalkingPoint>> _borderWalkingPointsFromXAndY;

    private WalkingPoint _firstBorderWalkingPoint = null;
    private WalkingPoint _previousBorderWalkingPoint = null;

    public WalkingPointCollection()
    {
        _borderWalkingPoints = new Dictionary<int, WalkingPoint>();
        _borderWalkingPointsFromXAndY = new Dictionary<int, Dictionary<int, WalkingPoint>>();
    }

    public void AddMapPoint(MapPoint point, int continentNumber, ref int s)
    {
        if (_borderWalkingPointsFromXAndY.ContainsKey(point.X) && _borderWalkingPointsFromXAndY[point.X].ContainsKey(point.Y)) { return; }

        var borderWalkingPoint = new WalkingPoint(s, point.X, point.Y, continentNumber);
        _borderWalkingPoints.Add(s, borderWalkingPoint);
        Dictionary<int, WalkingPoint> borderWalkingPointsFromY = _borderWalkingPointsFromXAndY.ContainsKey(point.X) ? _borderWalkingPointsFromXAndY[point.X] : null;
        if (borderWalkingPointsFromY == null)
        {
            borderWalkingPointsFromY = new Dictionary<int, WalkingPoint>();
            _borderWalkingPointsFromXAndY.Add(point.X, borderWalkingPointsFromY);
        }
        borderWalkingPointsFromY.Add(point.Y, borderWalkingPoint);

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
    public void LinkFirstAndLastOfContinent(WalkingPoint borderWalkingPoint)
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

    public IEnumerable<WalkingPoint> GetPoints()
    {
        return _borderWalkingPoints.Values;
    }

    public WalkingPoint GetClosest(int x, int y)
    {
        var targetPoint = new MapPoint(x, y);
        var closest = _borderWalkingPoints.Values.FirstOrDefault(p => p.X == x && p.Y == y);
        if(closest == null)
        {
            decimal minDistance = decimal.MaxValue;
            foreach(var point in _borderWalkingPoints.Values)
            {
                var distance = MapUtils.GetDistance(targetPoint, new MapPoint(point.X, point.Y));
                if(distance < minDistance)
                {
                    minDistance = distance;
                    closest = point;
                }
            }
        }
        return closest;
    }

    public WalkingPoint GetClosest(WalkingPoint point)
    {
        return GetClosest(point.X, point.Y);
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
}
