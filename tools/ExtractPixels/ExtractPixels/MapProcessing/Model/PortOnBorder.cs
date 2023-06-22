using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtractPixels.MapProcessing.Model;

public class PortOnBorder
{
    public Port Port { get; set; }
    /// <summary>
    /// OriginalLocation projected on the lands border
    /// </summary>
    public WalkingPoint WalkingPoint { get; set; }
    /// <summary>
    /// location based on GPS coordinates, cn be in the sea or the lands
    /// </summary>
    public MapPoint OriginalLocation { get; set; }


    public decimal DistanceToBorder { get; set; }
}
