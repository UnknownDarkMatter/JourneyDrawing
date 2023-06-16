using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtractPixels.MapProcessing.Model;

public class MapPoint 
{
    public int X { get; set; }
    public int Y { get; set; }

    public MapPoint()
    {

    }
    public MapPoint(int x, int y)
    {
        X = x; 
        Y = y; 
    }

    public override string ToString()
    {
        return $"X:{X},Y:{Y}";
    }

    public MapPoint Clone()
    {
        return new MapPoint(X, Y);
    }
}

