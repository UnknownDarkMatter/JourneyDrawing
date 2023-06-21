
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ExtractPixels.FileAdapter;

public class CsvSerializer : ICsvSerializer
{

    private readonly CsvConversionParameters _csvConversionParameters;
    private readonly CsvSerializationSettings _csvSerializationSettings;
    private readonly ValueConverter _valueConverter;


    public CsvSerializer(CsvConversionParameters csvConversionParameters,
        CsvSerializationSettings csvColumn2ConvertGenerator,
        ValueConverter valueConverter)
    {
        _csvConversionParameters = csvConversionParameters ?? throw new ArgumentNullException(nameof(csvConversionParameters));
        _csvSerializationSettings = csvColumn2ConvertGenerator ?? throw new ArgumentNullException(nameof(csvColumn2ConvertGenerator));
        _valueConverter = valueConverter ?? throw new ArgumentNullException(nameof(valueConverter));
    }

    public IEnumerable<T> DeserializeFromCsv<T>(CsvFile csvFile, string listName)
    {
        List<T> result = new List<T>();
        bool isfirstLine = true;
        Dictionary<string, int> columnsNumber = null;
        foreach (string line in (csvFile.Content.Split("\n")))
        {
            var cells = SplitLine(line, csvFile.ColumnSeparator);
            if (isfirstLine)
            {
                columnsNumber = GetColumnsNumber(cells);
                isfirstLine = false;
                continue;
            }
            if(cells.Count> 0 && !string.IsNullOrEmpty(cells[0])){
                AddEntity(cells, columnsNumber, listName, result);
            }
        }
        return result;
    }


    public CsvFile SerializeToCsv<T>(T[] objectsList, string listName)
        where T : new()
    {
        objectsList = objectsList ?? new T[0];

        var csvFile = new CsvFile();
        csvFile.Name = listName;
        csvFile.ColumnSeparator = _csvConversionParameters.CsvColumnSeparator.ToString();

        var sb = new StringBuilder();
        int lineNumber = 0;
        foreach (var objectItem in objectsList)
        {
            var line = SerializeToCsvLine(objectItem, listName, lineNumber);
            sb.Append(line);
            lineNumber++;
            if (lineNumber < objectsList.Length)
            {
                sb.Append("\r\n");
            }
        }

        csvFile.Content = sb.ToString();
        return csvFile;
    }



    private List<string> SplitLine(string line, string columnSeparator)
    {
        var cells = new List<string>();
        int cursor = 0;
        while (cursor >= 0)
        {
            if(cursor + 1 >= line.Length)
            {
                break;
            }
            int nextCursor = line.IndexOf(columnSeparator, cursor + 1);
            int nextCursorWithEspace = line.IndexOf(_csvConversionParameters.CsvColumnSeparatorEscape + columnSeparator, cursor + 1);

            string cellValue;
            //Fix : escape columnSeparator with " : cellValue =  "['2587', '2588', '2595', '2590', '2616']" and columnSeparator = , there is an escape with "
            string nextCar = line.Substring(cursor + 1, 1);
            if (nextCar == _csvConversionParameters.CsvColumnSeparatorEscape && nextCursorWithEspace >= 0)
            {
                var start = cursor == 0 ? 1 : cursor + 2;
                var length = cursor == 0 ? nextCursorWithEspace - cursor - 1 : nextCursorWithEspace - cursor - 2;
                if (length < 0)
                {
                    break;
                }
                cellValue = line.Substring(start, length);
                cursor = nextCursorWithEspace + 1;
            }
            else
            {
                if (nextCursor >= 0)
                {
                    cellValue = line.Substring(cursor == 0 ? 0 : cursor + 1, cursor == 0 ? nextCursor - cursor : nextCursor - cursor - 1);
                    if(cellValue == columnSeparator)
                    {
                        cellValue = "";
                    }
                }
                else
                {
                    cellValue = line.Substring(cursor == 0 ? 0 : cursor + 1);
                }
                cursor = nextCursor;
            }

            cells.Add(cellValue.Replace("\r", "").Replace("\n", ""));
        }
        return cells;
    }

    private string SerializeToCsvLine(dynamic object2Convert, string listName, int lineNumber, List<CsvSerializationSetting> columns2Convert = null)
    {
        if(columns2Convert == null)
        {
            columns2Convert = _csvSerializationSettings.GetSettings(object2Convert);
        }
        
        var sb = new StringBuilder();
        bool firstColumn = true;
        string cellValue;
        if(lineNumber == 0)
        {
            foreach (var column in columns2Convert)
            {
                if (!firstColumn)
                {
                    sb.Append(_csvConversionParameters.CsvColumnSeparator);
                }
                firstColumn = false;
                sb.Append(column.ColumnName.Replace("\"r", "").Replace("\"n", ""));
            }
            sb.Append("\r\n");
        }

        firstColumn = true;
        foreach (var column in columns2Convert)
        {
            if (!firstColumn)
            {
                sb.Append(_csvConversionParameters.CsvColumnSeparator);
            }
            firstColumn = false;
            cellValue = "";
            switch (column.DataType)
            {
                case DataType.NumericValue:
                    {
                        cellValue = _valueConverter.NumberToString(column.ValueGet(object2Convert)) ?? "";
                        break;
                    }
                case DataType.BooleanValue:
                    {
                        cellValue = _valueConverter.BooleanToString(column.ValueGet(object2Convert)) ?? "";
                        break;
                    }
                case DataType.DateTimeValue:
                    {
                        cellValue = _valueConverter.DateToString(column.ValueGet(object2Convert), listName) ?? "";
                        break;
                    }
                default:
                    {
                        cellValue = column.ValueGet(object2Convert) ?? "";
                        break;
                    }
            }
            if (cellValue.IndexOf(_csvConversionParameters.CsvColumnSeparator) >= 0)
            {
                cellValue = $"{_csvConversionParameters.CsvColumnSeparatorEscape}{cellValue}{_csvConversionParameters.CsvColumnSeparatorEscape}";
            }
            cellValue = cellValue.Replace("\"r", "").Replace("\"n", "");
            sb.Append(cellValue);
        }
        return sb.ToString();
    }

    private void AddEntity<T>(List<string> cells, Dictionary<string, int> columnsNumber, string listName, List<T> result)
    {
        if (cells.Count <= 1) { return; }
        var entitytype = typeof(T);
        var entity = Activator.CreateInstance(entitytype);
        List<CsvSerializationSetting> serializationSettings = _csvSerializationSettings.GetSettings((dynamic)entity);
        for(int columnIndex = 0; columnIndex < cells.Count; columnIndex++)
        {
            var cellAsString = cells[columnIndex];
            var columnNameKvp = columnsNumber.Where(kvp => kvp.Value == columnIndex).FirstOrDefault();
            if(columnNameKvp.Equals(default(KeyValuePair<string, int>))) { continue; }
            var serializationSetting = serializationSettings.FirstOrDefault(m => m.ColumnName.ToLower() == columnNameKvp.Key.ToLower());
            if(serializationSetting is null) { continue; }
            var pi = entitytype.GetProperty(serializationSetting.PropertyName);
            object? cellValue;
            switch (serializationSetting.DataType)
            {
                case DataType.NumericValue:
                    {
                        cellValue = _valueConverter.StringToNumber(cellAsString);
                        break;
                    }
                case DataType.BooleanValue:
                    {
                        cellValue = _valueConverter.StringToBoolean(cellAsString);
                        break;
                    }
                case DataType.DateTimeValue:
                    {
                        var csvFileFormatName = _csvConversionParameters.CsvDateFormats.GetFormat(listName);
                        cellValue = _valueConverter.StringToDate(cellAsString, csvFileFormatName);
                        break;
                    }
                default:
                    {
                        cellValue = cellAsString;
                        break;
                    }
            }
            var t = Nullable.GetUnderlyingType(pi.PropertyType) ?? pi.PropertyType;
            var safeValue = (cellValue == null) ? null : Convert.ChangeType(cellValue, t);
            pi.SetValue(entity, safeValue);
        }
        result.Add((T)entity);
    }


    private Dictionary<string, int> GetColumnsNumber(List<string> firstLineCells)
    {
        var columnNumbers = new Dictionary<string, int>();
        int i = 0;
        foreach (var cell in firstLineCells)
        {
            var cellValue = cell.Replace("\r", "").Replace("\n", "");
            if (!string.IsNullOrWhiteSpace(cellValue))
            {
                if(!columnNumbers.ContainsKey(cellValue))
                {
                    columnNumbers.Add(cellValue, i);
                }
                i++;
            }
        }
        return columnNumbers;
    }

    private IEnumerable<string> GetColumnNames<T>(T[] objectsList)
    {
        var columns = new List<string>();
        foreach (var cell in objectsList)
        {
            var pis = cell.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var pi in pis)
            {
                if (pi.PropertyType == typeof(string))
                {
                    string propertyName = (string)pi.GetValue(cell);
                    columns.Add(propertyName);
                }
            }
        }
        return columns.Distinct();
    }


}
