using ExtractPixels.MapProcessing.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtractPixels.MapProcessing;

public class CSharpGenerator
{
    public void GenerateSCharp(Dictionary<int, Dictionary<int, SeaTrip>> seaTrips, BorderPointCollection borderPointsCollection, string filePath)
    {
        var sb = new StringBuilder();
        sb.Append(@"
using ExtractPixels.MapProcessing.Model;

namespace ExtractPixels.MapProcessing;

public class SeaTripsData
{
    /// <summary>
    /// [S, SeaTrip]
    /// </summary>
    public Dictionary<int, Dictionary<int, SeaTrip>> SeaTrips;

    /// <summary>
    /// [S, BorderWalkingPoint]
    /// </summary>
    public Dictionary<int, BorderWalkingPoint> BorderWalkingPoints;


    public SeaTripsData(){
        SeaTrips = new Dictionary<int, Dictionary<int, SeaTrip>>();
        BorderWalkingPoints = new Dictionary<int, BorderWalkingPoint>();

        var list = new List<BorderWalkingPoint>();

//////////////////// SeaTrips ////////////////////
");
        foreach(var seaTrip in seaTrips)
        {
            sb.AppendLine($"        SeaTrips.Add({seaTrip.Key}, new Dictionary<int, SeaTrip>());");
            foreach(var destination in seaTrip.Value)
            {
                sb.AppendLine("        list = new List<BorderWalkingPoint>();");
                foreach (var tripPoint in seaTrips[seaTrip.Key][destination.Key].TripPoints)
                {
                    sb.AppendLine($"        list.Add(new BorderWalkingPoint({tripPoint.S},{tripPoint.X},{tripPoint.Y},{tripPoint.ContinentNumber}));");
                }
                
                sb.AppendLine(@$"        SeaTrips[{seaTrip.Key}].Add({destination.Key}, new SeaTrip(
            new BorderWalkingPoint({destination.Value.StartPoint.S},{destination.Value.StartPoint.X},{destination.Value.StartPoint.Y},{destination.Value.StartPoint.ContinentNumber}), 
            new BorderWalkingPoint({destination.Value.EndPoint.S},{destination.Value.EndPoint.X},{destination.Value.EndPoint.Y},{destination.Value.EndPoint.ContinentNumber}), 
            list));
");
            }

        }
        sb.AppendLine("//////////////////// BorderWalkingPoints ////////////////////");
        foreach(var borderPoint in borderPointsCollection.BorderWalkingPoints.Values)
        {
            sb.AppendLine($"        BorderWalkingPoints.Add({borderPoint.S}, new BorderWalkingPoint({borderPoint.S},{borderPoint.X},{borderPoint.Y},{borderPoint.ContinentNumber}));");
        }

        sb.AppendLine("    }");
        sb.AppendLine("}");

        File.WriteAllText(filePath, sb.ToString());
    }
}
