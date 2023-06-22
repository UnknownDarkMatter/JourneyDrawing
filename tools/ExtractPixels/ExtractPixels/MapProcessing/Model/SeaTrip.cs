using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtractPixels.MapProcessing.Model;

public class SeaTrip
{
    public WalkingPoint StartPoint;
    public WalkingPoint EndPoint;
    public IEnumerable<WalkingPoint> TripPoints;

    public SeaTrip(WalkingPoint startPoint, WalkingPoint endPoint, IEnumerable<WalkingPoint> tripPoints)
    {
        StartPoint = startPoint;
        EndPoint = endPoint;
        TripPoints = tripPoints;
    }

    public void DrawTrip(Bitmap image, string outputPath)
    {
        int width = image.Width;
        int height = image.Height;

        var imageOutput = image.Clone() as Bitmap;

        foreach(var tripPoint in TripPoints)
        {
            imageOutput.SetPixel(tripPoint.X, tripPoint.Y, Color.Orange);
        }
        imageOutput.Save(outputPath);
    }
}
