using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtractPixels.MapProcessing.Model;

public class SeaTrip
{
    public BorderWalkingPoint StartPoint;
    public BorderWalkingPoint EndPoint;
    public IEnumerable<BorderWalkingPoint> TripPoints;

    public SeaTrip(BorderWalkingPoint startPoint, BorderWalkingPoint endPoint, IEnumerable<BorderWalkingPoint> tripPoints)
    {
        StartPoint = startPoint;
        EndPoint = endPoint;
        TripPoints = tripPoints;
    }
}
