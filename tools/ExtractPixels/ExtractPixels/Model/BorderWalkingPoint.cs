using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtractPixels.Model;

public class BorderWalkingPoint
{
    /// <summary>
    /// variable indicating progress when walking along the continents
    /// </summary>
    public int S { get; set; }
    public int X { get; set; }
    public int Y { get; set; }

    public int ContinentNumber { get; set; }

    public int SPlus1 { get; set; }
    public int SMinus1 { get; set; }

    public BorderWalkingPoint(int s, int x, int y, int continentNumber)
    {
        S = s;
        X = x;
        Y = y;
        ContinentNumber = continentNumber;
    }
}
