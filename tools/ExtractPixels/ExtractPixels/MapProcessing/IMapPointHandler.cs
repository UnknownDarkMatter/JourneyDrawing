using ExtractPixels.MapProcessing.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtractPixels.MapProcessing;

public interface IMapPointHandler
{
    public void AddMapPoint(MapPoint point, int continentNumber, ref int s);
}
