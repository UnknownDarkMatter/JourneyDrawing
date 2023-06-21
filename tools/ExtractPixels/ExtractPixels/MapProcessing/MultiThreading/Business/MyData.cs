using ExtractPixels.MapProcessing.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtractPixels.MapProcessing.MultiThreading.Business;

public class MyData : WorkerData
{

    public int sStart { get; set; }

    public int sEnd { get; set; }

    public BorderPointCollection borderWalkingPoints { get; set; }

    public decimal width { get; set; }

    public decimal height { get; set; }

    public string imageFilePath { get; set; }

    public Bitmap image { get; set; }

    public SeaTrip SeaTrip { get; set; }

    public MyData(int sStart, int sEnd, BorderPointCollection borderWalkingPoints,
        decimal width, decimal height, string imageFilePath, Bitmap image) {
        this.sStart = sStart;
        this.sEnd = sEnd;
        this.borderWalkingPoints = borderWalkingPoints;
        this.width = width;
        this.height = height;
        this.imageFilePath = imageFilePath;
        this.image = image;
    }

}
