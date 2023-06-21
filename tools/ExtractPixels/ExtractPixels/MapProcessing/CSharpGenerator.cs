using ExtractPixels.MapProcessing.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtractPixels.MapProcessing;

public class CSharpGenerator
{
    public void GenerateSCharp(Dictionary<int, Dictionary<int, SeaTrip>> seaTrips, string filePath)
    {
        var sb = new StringBuilder();
        sb.Append(@"
using ExtractPixels.MapProcessing.Model;

namespace ExtractPixels.MapProcessing;

public class SeaTripsData
{
    public Dictionary<int, Dictionary<int, SeaTrip>> SeaTrips;

    public SeaTripsData(){
        SeaTrips = new Dictionary<int, Dictionary<int, SeaTrip>>();
        var list = new List<BorderWalkingPoint>();

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

        sb.AppendLine("    }");
        sb.AppendLine("}");

        File.WriteAllText(filePath, sb.ToString());
    }
}
