using ExtractPixels.FileAdapter;
using ExtractPixels.MapProcessing.Coordinates;
using ExtractPixels.MapProcessing.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ExtractPixels.MapProcessing;

public class PortsLoader
{
    private ICsvSerializer _csvSerializer;
    private CsvConversionParameters _csvConversionParameters;
    private string _portsFilePath;
    private BorderPointCollection _borderPointCollection;

    public PortsLoader(string portsFilePath, BorderPointCollection borderPointCollection)
    {
        _portsFilePath = portsFilePath;
        _borderPointCollection = borderPointCollection;
        _csvConversionParameters = new CsvConversionParameters()
        {
            CsvDateFormats = new CsvDateFormats(new List<CsvDateFormat>()),
            CsvColumnSeparator = ';',
            CsvDecimalCulture = "en-GB" // separateur decimal = . et pas ,
        };
        var valueConverter = new ValueConverter(_csvConversionParameters);
        var csvSerializationSettings = new CsvSerializationSettings(valueConverter);
        _csvSerializer = new CsvSerializer(_csvConversionParameters, csvSerializationSettings, valueConverter);
    }

    public List<PortOnBorder> LoadPorts(MapSize size)
    {
        string listName = "Ports";
        string fileContent = File.ReadAllText(_portsFilePath);
        var csvFile = new CsvFile()
        {
            Name= listName,
            ColumnSeparator= _csvConversionParameters.CsvColumnSeparator.ToString(),
            Content = fileContent
        };
        var ports = _csvSerializer.DeserializeFromCsv<Port>(csvFile, listName).ToList();

        var result = new List<PortOnBorder>();

        foreach (var port in ports)
        {
            var utmCoordinates = UtmToLatLongConverter.fromLatLon(port.LatitudeWGS84, port.LongitudeWGS84);
            var pixelCoordinates = UtmToPixelsConverter.UtmToPixels(utmCoordinates, size);
            var portOnBorder = new PortOnBorder()
            {
                BorderWalkingPoint = null,
                DistanceToBorder = decimal.MaxValue,
                OriginalLocation = pixelCoordinates,
                Port = port
            };
            result.Add(portOnBorder);
        }

        if (Constants.IsDebug)
        {
            result = new List<PortOnBorder>();
            var portOnBorder = new PortOnBorder()
            {
                BorderWalkingPoint = null,
                DistanceToBorder = decimal.MaxValue,
                OriginalLocation = new MapPoint(31, 288),
                Port = new Port() { Name="Port1" }
            };
            result.Add(portOnBorder);

            portOnBorder = new PortOnBorder()
            {
                BorderWalkingPoint = null,
                DistanceToBorder = decimal.MaxValue,
                OriginalLocation = new MapPoint(189, 304),
                Port = new Port() { Name = "Port2" }
            };
            result.Add(portOnBorder);
        }

        foreach (var borderPoint in _borderPointCollection.BorderWalkingPoints)
        {
            foreach(var portOnBorder in result)
            {
                var distance = MapUtils.GetDistance(portOnBorder.OriginalLocation, borderPoint.Value.Point);
                if(distance< portOnBorder.DistanceToBorder)
                {
                    portOnBorder.DistanceToBorder = distance;
                    portOnBorder.BorderWalkingPoint = borderPoint.Value;
                }
            }
        }


        return result;
    }

}
