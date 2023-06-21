using ExtractPixels.FileAdapter;
using ExtractPixels.MapProcessing.Coordinates;
using ExtractPixels.MapProcessing.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtractPixels.MapProcessing;

public class PortsLoader
{
    private ICsvSerializer _csvSerializer;
    private CsvConversionParameters _csvConversionParameters;
    private string _portsFilePath;

    public PortsLoader(string portsFilePath)
    {
        _portsFilePath = portsFilePath;
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

    public List<Tuple<Port, MapPoint>> LoadPorts(MapSize size)
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

        var result = new List<Tuple<Port, MapPoint>>();

        foreach(var port in ports)
        {
            var utmCoordinates = UtmToLatLongConverter.fromLatLon(port.LatitudeWGS84, port.LongitudeWGS84);
            var pixelCoordinates = UtmToPixelsConverter.UtmToPixels(utmCoordinates, size);
            result.Add(new Tuple<Port, MapPoint>(port, pixelCoordinates));
        }

        return result;
    }

}
