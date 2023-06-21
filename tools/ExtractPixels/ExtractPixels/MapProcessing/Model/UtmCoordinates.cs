using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtractPixels.MapProcessing.Model;

public class UtmCoordinates
{
    public decimal Easting { get; set; }
    public decimal Northing { get; set; }
    public int ZoneNum { get; set; }
    public string? ZoneLetter { get; set; }
}
