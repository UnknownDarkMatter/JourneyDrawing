using ExtractPixels.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    public MapPreprocessingGenerator()
    {
        _borderWalkingPoints = new Dictionary<int, BorderWalkingPoint>();
        _borderWalkingPointsFromXAndY = new Dictionary<int, Dictionary<int, BorderWalkingPoint>>();
    }

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
        s++;
    }

    /// <summary>
    /// génère une variable s qui permet de parcourit un continent donné
    /// </summary>
    public void Step1GenerateBorderWalkingVariable()
    {

    }

}
