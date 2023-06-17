using System;
using System.Collections.Generic;
using System.Drawing;
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
