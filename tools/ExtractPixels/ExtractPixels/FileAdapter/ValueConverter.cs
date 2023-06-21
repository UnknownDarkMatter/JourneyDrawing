using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtractPixels.FileAdapter;

public class ValueConverter
{
    private readonly CsvConversionParameters _csvConversionParameters;

    public ValueConverter(CsvConversionParameters csvConversionParameters)
    {
        _csvConversionParameters = csvConversionParameters;
    }

    public decimal? StringToNumber(string valueAsString, bool isNullable = false)
    {
        valueAsString = valueAsString.Replace(",", ".");
        if (!decimal.TryParse(valueAsString, NumberStyles.Number, new CultureInfo("en-GB"), out decimal value))
        {
            return null;
        }
        return value;
    }

    public string NumberToString(decimal? valueAsDecimal)
    {
        if(valueAsDecimal == null) { return null; }
        return valueAsDecimal.ToString().Replace(",", ".");
    }

    public bool? StringToBoolean(string valueAsString)
    {
        valueAsString = (valueAsString ?? "").ToUpper().Trim();
        if (string.IsNullOrEmpty(valueAsString)) { return null; }

        switch (valueAsString)
        {
            case "1":
            case "TRUE":
            case "VRAI":
                { return (bool?)true; }
            case "0":
            case "FALSE":
            case "FAUX":
                { return (bool?)false; }
            default:
                {
                    throw new NotImplementedException($"Cannot convert '{valueAsString}' to boolean.");
                }
        }

    }

    public string BooleanToString(bool? valueAsBoolean)
    {
        if (valueAsBoolean == null) { return null; }
        return valueAsBoolean.Value ? "True" : "False";
    }

    public string DateToString(DateTime? valueAsDate, string csvDateFormatName)
    {
        if (valueAsDate == null) { return null; }
        string dateFormat = _csvConversionParameters.CsvDateFormats.GetFormat(csvDateFormatName);
        if (dateFormat == null)
        {
            throw new Exception($"CsvDateFormats has no value for '{csvDateFormatName}' in appSettings.");
        }
        return valueAsDate.Value.ToString(dateFormat);
    }

    public DateTime? StringToDate(string valueAsString, string csvDateFormatName)
    {
        if (string.IsNullOrWhiteSpace(valueAsString)) { return null; }
        if (DateTime.TryParseExact(valueAsString, csvDateFormatName, new CultureInfo("en-GB"), DateTimeStyles.None, out DateTime valueAsDate))
        {
            return (DateTime?)valueAsDate;
        }
        return null;
    }

}
