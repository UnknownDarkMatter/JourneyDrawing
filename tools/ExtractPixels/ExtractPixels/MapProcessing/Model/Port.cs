using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtractPixels.MapProcessing.Model;

public class Port
{
    public string Name { get; set; }
    /// <summary>
    /// Latitude in GPS coordinates (WGS94)
    /// </summary>
    public decimal LatitudeWGS84 { get; set; }
    /// <summary>
    /// Longitude in GPS coordinates (WGS94)
    /// </summary>
    public decimal LongitudeWGS84 { get; set; }



}
