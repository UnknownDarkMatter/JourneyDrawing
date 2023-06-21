using ExtractPixels.MapProcessing.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ExtractPixels.FileAdapter;

public class CsvSerializationSettings
{
    private readonly ValueConverter _valueConverter;

    public CsvSerializationSettings(ValueConverter valueConverter)
    {
        _valueConverter = valueConverter ?? throw new ArgumentNullException(nameof(valueConverter));
    }

    public List<CsvSerializationSetting> GetSettings(Port object2Convert)
    {
        return new List<CsvSerializationSetting>()
            {
                new CsvSerializationSetting()
                {
                   ColumnName = "Name",
                   PropertyName = "Name",
                   DataType = DataType.StringValue,
                   ValueGet = (element) => ((Port)element).Name
                },
                new CsvSerializationSetting()
                {
                    ColumnName = "LatitudeWGS84",
                    PropertyName = "LatitudeWGS84",
                    DataType = DataType.NumericValue,
                    ValueGet = (element) => ((Port)element).LatitudeWGS84
                },
                new CsvSerializationSetting()
                {
                    ColumnName = "LongitudeWGS84",
                    PropertyName = "LongitudeWGS84",
                    DataType = DataType.NumericValue,
                    ValueGet = (element) => ((Port)element).LongitudeWGS84
                },
            };
    }



}
